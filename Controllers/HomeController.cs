using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevisioneNew.Models;
using System.Diagnostics;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Security.Claims;

namespace RevisioneNew.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //private readonly GraphServiceClient _graphServiceClient;
        private readonly ServiceClient _serviceClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ServiceClient serviceClient)
        {
            _logger = logger;
            //_graphServiceClient = graphServiceClient;
            _serviceClient = serviceClient;
        }

        [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
        public IActionResult Index()
        {
            var userClaims = User.Identity;
            ViewBag.Home = "active";
            return View();
        }

        public IActionResult Privacy()
        {
            ViewBag.Privacy = "active";
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
