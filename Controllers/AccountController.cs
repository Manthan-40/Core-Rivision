using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace RevisioneNew.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
            //return SignIn(new AuthenticationProperties { RedirectUri = Url.Action("Login", "Account") }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult LoginCallBack()
        {
            
            //return Redirect("/MicrosoftIdentity/Account/SignIn");
            return RedirectToAction("SignIn", "Account", new { area = "MicrosoftIdentity" });
        }

        public IActionResult Logout() {

            return SignOut(new AuthenticationProperties{ RedirectUri = Url.Action("Login", "Account")},OpenIdConnectDefaults.AuthenticationScheme);
        }

    }
}
