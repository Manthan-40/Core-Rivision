using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json.Linq;
using RevisioneNew.Models;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using RevisioneNew.CustomFilters;
using System.Reflection;
using System.ComponentModel;

namespace RevisioneNew.Controllers
{
    [CustomAuthorize]
    [Authorize]
    public class LeadController : Controller
    {
        //private readonly GraphServiceClient _graphServiceClient;
        private readonly ServiceClient _serviceClient;
        List<LeadModel> leadList = [];
        public LeadController(ServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
            QueryExpression queryExpression = new QueryExpression("lead") { ColumnSet = new ColumnSet("subject", "fullname", "statecode", "createdon") };
            EntityCollection leades = _serviceClient.RetrieveMultiple(queryExpression);
            foreach (var item in leades.Entities)
            {
                leadList.Add(new LeadModel
                {
                    TopicName = item.GetAttributeValue<string>("subject"),
                    FullName = item.GetAttributeValue<string>("fullname"),
                    Status = ((LeadStateCode)item.GetAttributeValue<OptionSetValue>("statecode").Value).ToString(),
                    CreatedON = item.GetAttributeValue<DateTime>("createdon"),
                    Id = item.GetAttributeValue<Guid>("leadid")
                });
            }
        }



        [HttpPost]
        public JsonResult LoadLeadList()
        {
            LeadStatusCode LeadStatusCod = LeadStatusCode.Canceled;
            FieldInfo fi = LeadStatusCod.GetType().GetField(LeadStatusCod.ToString());
            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
               string desiption = attributes.First().Description;
            }

            ViewBag.Lead = "active";
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

                var LeadlistData = leadList.AsQueryable();
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    LeadlistData = LeadlistData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    LeadlistData = LeadlistData.Where(m => m.FullName.Contains(searchValue)
                                                || m.TopicName.Contains(searchValue));
                }
                recordsTotal = LeadlistData.Count();
                var data = LeadlistData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                var data2 = Json(jsonData);
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("/leades")]
        public IActionResult Leades()
        {
            ViewBag.Lead = "active";
            return View();
        }

        [HttpPost]
        public IActionResult LeadQualification(string leadId) {
            try
            {
                if (leadId != null)
                {
                    Guid leadID = new Guid(leadId);

                    QualifyLeadRequest request = new QualifyLeadRequest();
                    request.LeadId = new EntityReference("lead", leadID);
                    request.Status = new OptionSetValue(-1);
                    request.CreateOpportunity = false;
                    request.CreateAccount = false;
                    request.CreateContact = false;

                    QualifyLeadResponse response = (QualifyLeadResponse)_serviceClient.Execute(request);
                    return Ok("Lead is sucessfully qualified.");
                }
                return BadRequest("Lead is not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }


        [HttpPost]
        public IActionResult LeadDisqualification(string leadId)
        {
            try
            {
                if (leadId != null)
                {
                    Guid leadID = new Guid(leadId);

                    SetStateRequest disqualificationRequest = new SetStateRequest();
                    disqualificationRequest.EntityMoniker = new EntityReference("lead", leadID);
                    disqualificationRequest.State = new OptionSetValue(((int)LeadStateCode.Disqualified));
                    disqualificationRequest.Status = new OptionSetValue(((int)LeadStatusCode.Lost));

                    SetStateResponse response = (SetStateResponse)_serviceClient.Execute(disqualificationRequest);
                    return Ok("Lead is sucessfully disqualified.");
                }
                return BadRequest("Lead is not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
