using Microsoft.AspNetCore.Mvc;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Linq;
using System.Linq.Dynamic.Core;
using RevisioneNew.Models;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace RevisioneNew.Controllers
{
    public class OpportunityController : Controller
    {
        //private readonly GraphServiceClient _graphServiceClient;
        private readonly ServiceClient _serviceClient;
        List<Opportunity> opportunityList = [];
        public OpportunityController(ServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
            QueryExpression queryExpression = new QueryExpression("opportunity") { ColumnSet = new ColumnSet("name", "description", "statecode", "createdon") };
            EntityCollection leades = _serviceClient.RetrieveMultiple(queryExpression);
            foreach (var item in leades.Entities)
            {
                opportunityList.Add(new Opportunity
                {
                    Topic = item.GetAttributeValue<string>("name"),
                    Description = item.GetAttributeValue<string>("description"),
                    Status = ((OpportunityStateCode)item.GetAttributeValue<OptionSetValue>("statecode").Value).ToString(),
                    CreatedON = item.GetAttributeValue<DateTime>("createdon"),
                    Id = item.GetAttributeValue<Guid>("opportunityid")
                });
            }
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
                int recordsTotal = 0;

                var OpportunityData = opportunityList.AsQueryable();
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    OpportunityData = OpportunityData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    OpportunityData = OpportunityData.Where(m => m.Topic.Contains(searchValue)
                                                || m.Description.Contains(searchValue));
                }
                recordsTotal = OpportunityData.Count();
                var data = OpportunityData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                var data2 = Json(jsonData);
                return Json(jsonData);

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
    }
}
