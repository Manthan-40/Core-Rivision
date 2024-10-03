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
        }

        public void LoginCallBack()
        {
            HttpContext.ChallengeAsync("OpenIdConnect", new AuthenticationProperties { RedirectUri = Url.Action("Index","Home") }).Wait();
        }

        public IActionResult Logout() {


            //HttpContext.SignOutAsync("OpenIdConnect", new AuthenticationProperties { RedirectUri = Url.Action("Login","Account") }).Wait();
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return SignOut(new AuthenticationProperties { RedirectUri = Url.Action("Login", "Account") }, OpenIdConnectDefaults.AuthenticationScheme);
        }

    }
}
