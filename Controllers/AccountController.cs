using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Xrm.Sdk;
using Microsoft.PowerPlatform.Dataverse.Client;
using RevisioneNew.CustomFilters;
using RevisioneNew.Interfaces;
using RevisioneNew.Models;
using Microsoft.Xrm.Sdk.Query;

namespace RevisioneNew.Controllers
{
    public class AccountController : Controller
    {
        private readonly ServiceClient _serviceClient;
        private readonly IServiceInterface _service;

        public AccountController( ServiceClient serviceClient, IServiceInterface service)
        {
            _service = service;
            _serviceClient = serviceClient;
        }

        [Route("/Login")]
        public IActionResult Login()
        {
            return View();
        }

        public void LoginCallBack()
        {
            string cookieName = "LogInUserFromPage";
            string cookieValue = "true";

            // Set cookie options (optional)
            CookieOptions options = new CookieOptions
            {
                Secure = true    // Use true if the application is using HTTPS
            };

            // Add the cookie to the response
            Response.Cookies.Append(cookieName, cookieValue, options);
            HttpContext.ChallengeAsync("OpenIdConnect", new AuthenticationProperties { RedirectUri = Url.Action("LogincallBackRedirection", "Account") }).Wait();
        }

        public IActionResult LogincallBackRedirection()
        {
            var userClaims = User.Identity;

            string mail =userClaims.Name;
            Entity user = _service.GetCurrentUser(mail);
            bool IsfirstTimeLogin = user.Contains("md_isfirsttimelogin")? user.GetAttributeValue<bool>("md_isfirsttimelogin") : false;

            if (IsfirstTimeLogin)
            {
                Entity Sender = _service.Get("queue", new ColumnSet("name", "emailaddress"), LogicalOperator.And, [new ConditionExpression("name", ConditionOperator.Equal, "Welcoming Email")]);
                //Entity Receiver = _service.Get("contact", new ColumnSet(false), LogicalOperator.And, [new ConditionExpression("emailaddress1", ConditionOperator.Equal, mail)]);
                Entity EmailTemplate = _service.Get("template", new ColumnSet("title"), LogicalOperator.And, [new ConditionExpression("title", ConditionOperator.Equal, "Welcome Email to User")]);

                bool SendMail = _service.SendMail(Sender, user, EmailTemplate);
                if(SendMail)
                {
                    user["md_isfirsttimelogin"] = false;
                    _service.Update(user);
                   return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Support()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Support(SupportSignIn ssi)
        {
            if (ModelState.IsValid)
            {
                _service.SendSupportMail(ssi);
                TempData["SupportMailSended"] = true;
                //return Ok("You Query is successfully Registered");
                return RedirectToAction("Login", "Account");
            }
            return View(ssi);
        }
    }
}
