using Microsoft.AspNetCore.Mvc;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Linq;
using System.Linq.Dynamic.Core;
using RevisioneNew.Models;
using Microsoft.PowerPlatform.Dataverse.Client;
using RevisioneNew.CustomFilters;
    using Microsoft.AspNetCore.Authorization;
using RevisioneNew.Interfaces;
using RevisioneNew.Services;

namespace RevisioneNew.Controllers
{
    [CustomAuthorize]
    [Authorize]
    public class OpportunityController : Controller
    {
        //private readonly GraphServiceClient _graphServiceClient;
        private readonly ServiceClient _serviceClient;
        private readonly IOpportunityInterface _opportunityService;
        public OpportunityController(ServiceClient serviceClient, IOpportunityInterface opportunityService)
        {
            _serviceClient = serviceClient;
            _opportunityService = opportunityService;
        }



        [HttpPost]
        public JsonResult LoadOpportunityList()
        {
            ViewBag.Opportunity = "active";
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                PagingInfo pagingInfo = new PagingInfo()
                {
                    Count = pageSize,
                    PageNumber = (skip/pageSize)+1,
                    ReturnTotalRecordCount =true
                };
                Datatable<OpportunityModel> opportunities = _opportunityService.GetaAllOpportunities(draw, sortColumn, pagingInfo, searchValue, sortColumnDirection);
                
                return Json(opportunities);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("/Opportunities")]
        public IActionResult Opportunities()
        {
            ViewBag.Opportunity = "active";
            return View();
        }

        [HttpPost]
        public IActionResult WonOpporunity(string opportunityID)
        {
            try
            {
                if (opportunityID != null)
                {
                    Guid winopportunityID = new Guid(opportunityID);

                    WinOpportunityRequest winOpportunityRequest = new WinOpportunityRequest();
                    winOpportunityRequest.OpportunityClose = new Entity("opportunityclose")
                    {
                        Attributes =
                        {
                            {"opportunityid",new EntityReference("opportunity",winopportunityID) }
                        },

                    };
                    winOpportunityRequest.Status = new OptionSetValue(-1);
                    WinOpportunityResponse response = (WinOpportunityResponse)_serviceClient.Execute(winOpportunityRequest);
                    return Ok("Opportunity is sucessfully Won.");

                }
                return BadRequest("Opportunity is not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost]
        public IActionResult LostOpportunity(string opportunityID)
         {
            try
            {
                if (opportunityID != null)
                {
                    Guid loseopportunityID = new Guid(opportunityID);

                    LoseOpportunityRequest loseOpportunityRequest = new LoseOpportunityRequest();
                    loseOpportunityRequest.OpportunityClose = new Entity("opportunityclose")
                    {
                        Attributes =
                        {
                            {"opportunityid",new EntityReference("opportunity",loseopportunityID) }
                        },

                    };
                    loseOpportunityRequest.Status = new OptionSetValue(-1);
                    LoseOpportunityResponse response = (LoseOpportunityResponse)_serviceClient.Execute(loseOpportunityRequest);
                    return Ok("Opportunity is sucessfully Lost.");

                }
                return BadRequest("Opportunity is not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // Action to display Opportunity details
        public IActionResult Details(string opportunityId = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(opportunityId))
                {
                    Guid OpportunityGUID = new Guid(opportunityId);
                    OpportunityModel opportunity = _opportunityService.GetOpportunityById(OpportunityGUID);

                    ViewBag.Opportunity = "active";
                    return View(opportunity);
                }
                return BadRequest("Can't find OpportunityID '" + opportunityId + "'");
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Action to add or edit an Opportunity (GET)
        public IActionResult AddOrEditOpportunity(string opportunityId = null)
        {
            try
            {
                if (opportunityId != null)
                {
                    Guid OpportunityGUID = new Guid(opportunityId);
                    OpportunityModel opportunity = _opportunityService.GetOpportunityById(OpportunityGUID);

                    ViewBag.Opportunity = "active";
                    return View(opportunity);
                }
                else
                {
                    OpportunityModel opportunity = _opportunityService.GetEmptyModel();

                    ViewBag.Opportunity = "active";
                    return View(opportunity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Action to handle Opportunity form submission (POST)
        [HttpPost]
        public IActionResult AddOrEditOpportunity(OpportunityModel opportunity)
        {
            try
            {
                OpportunityModel opportunityForList = _opportunityService.GetEmptyModel();
                opportunity.AccountList = opportunityForList.AccountList;
                opportunity.ContactList = opportunityForList.ContactList;
                opportunity.PriceListitems = opportunityForList.PriceListitems;
                opportunity.CurrencyList = opportunityForList.CurrencyList;

                if (ModelState.IsValid)
                {
                    if (opportunity.Id == new Guid())
                    {
                        _opportunityService.CreateOpportunity(opportunity);
                        TempData["OpportunityStatus"] = true.ToString();
                        TempData["OpportunityData"] = "New Opportunity is created.";
                    }
                    else
                    {
                        _opportunityService.UpdateOpportunity(opportunity);
                        TempData["OpportunityStatus"] = true.ToString();
                        TempData["OpportunityData"] = "Opportunity is Updated.";
                    }
                    return RedirectToAction("Opportunities", "Opportunity");
                }
                return View(opportunity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Action to delete an Opportunity
        [HttpPost]
        public IActionResult DeleteOpportunity(string opportunityId)
        {
            try
            {
                if (opportunityId != null)
                {
                    Guid opportunityGuid = new Guid(opportunityId);
                    _opportunityService.DeleteOpportunity(opportunityGuid);
                    return Ok("Data deleted successfully.");
                }
                return BadRequest("Opportunity not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
