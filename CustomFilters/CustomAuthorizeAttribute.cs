using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Graph;
using RevisioneNew.Models;

namespace RevisioneNew.CustomFilters
{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        //private readonly SingleSignInModel _sso;

        //public CustomAuthorizeAttribute(SingleSignInModel sso)
        //{
        //    _sso = sso;
        //}
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Console.WriteLine("Hiii");

            string CookieValue = context.HttpContext.Request.Cookies["LogInUserFromPage"];
            if (CookieValue == null) {
                context.Result = new RedirectToActionResult("Login","Account",null);
            }

        }
    }
}
