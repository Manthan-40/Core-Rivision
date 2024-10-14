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
using RevisioneNew.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RevisioneNew.Controllers
{
    [CustomAuthorize]
    [Authorize]
    public class LeadController : Controller
    {
        //private readonly GraphServiceClient _graphServiceClient;
        private readonly ServiceClient _serviceClient;
        private readonly ILeadInterface _leadService;
        public LeadController(ServiceClient serviceClient, ILeadInterface leadInterface)
        {
            _serviceClient = serviceClient;
            _leadService = leadInterface;
           
        }

        [HttpPost]
        public JsonResult LoadLeadList()
        {
            string a = "1";
            customoptions ca = (customoptions)Convert.ToInt32(a);

            foreach(int i in Enum.GetValues(typeof(customoptions)))
            {
                Console.WriteLine(i.ToString());
                int attributes = (((customoptions)Convert.ToInt32(a)).GetType().GetField(((customoptions)Convert.ToInt32(a)).ToString()).GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[]).First().Order;

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

                PagingInfo pagingInfo = new PagingInfo()
                {
                    Count = pageSize,
                    PageNumber = (skip / pageSize) + 1,
                    ReturnTotalRecordCount = true
                };
                Datatable<LeadModel> leads = _leadService.GetaAllLeades(draw, sortColumn, pagingInfo, searchValue, sortColumnDirection);

                return Json(leads);
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


        public IActionResult Details(string leadId =null)
        {
            try
            {
                if (!string.IsNullOrEmpty(leadId))
                {
                    Guid LeadGUID = new Guid(leadId);
                    LeadModel lead = _leadService.GetLeadById(LeadGUID);                    

                    ViewBag.Lead = "active";        
                    return View(lead);
                }
                return BadRequest("Can't find LeadID '" + leadId +"'");

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IActionResult AddorEditLead(string LeadId = null)
        {
            try
            {
                if (LeadId != null)
                {
                    Guid LeadGUID = new Guid(LeadId);
                    LeadModel lead = _leadService.GetLeadById(LeadGUID);

                    ViewBag.Lead = "active";
                    return View(lead);
                }
                else
                {
                    LeadModel lead = _leadService.GetEmptyModel();

                    ViewBag.Lead = "active";
                    return View(lead);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public IActionResult AddorEditLead(LeadModel lead)
        {
            try
                 {
                    LeadModel leadmodelForList = _leadService.GetEmptyModel();
                    lead.AccountList = leadmodelForList.AccountList;
                    lead.ContactList = leadmodelForList.ContactList;
                if (ModelState.IsValid)
                {
                    if(lead.Id == new Guid())
                    {
                        _leadService.CreateLead(lead);

                    }
                    else
                    {
                        _leadService.UpdateLead(lead);
                        TempData["LeadStatus"] = true.ToString();
                        TempData["LeadData"] = "Lead is updated.";
                    }
                    return RedirectToAction("leades", "Lead");
                }
                return View(lead);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public IActionResult DeleteLead(string leadId)
        {
            try
            {
                if (leadId != null)
                {
                    Guid leadGuid = new Guid(leadId);
                    _leadService.DeleteLead(leadGuid);
                    return Ok("Data deleted successfully.");
                }
                return BadRequest("Quote is not found.");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

    }
}
