using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using RevisioneNew.CustomFilters;
using RevisioneNew.Models;

namespace RevisioneNew.Controllers
{
    public class AccountController : Controller
    {

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
            HttpContext.ChallengeAsync("OpenIdConnect", new AuthenticationProperties { RedirectUri = Url.Action("Index","Home") }).Wait();
        }
    }
}
