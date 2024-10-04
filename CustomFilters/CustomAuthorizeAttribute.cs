using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Graph;

namespace RevisioneNew.CustomFilters
{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Console.WriteLine("Hiii");

            string CookieValue = context.HttpContext.Request.Cookies["LogInUserFromPage"];
            string username = context.HttpContext.User.Identity.Name;
            if (CookieValue == null || username == null) {
                context.Result = new RedirectToActionResult("Login","Account",null);
            }

        }
    }
}
