using Microsoft.AspNetCore.Mvc;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Linq.Dynamic.Core;
using RevisioneNew.Models;
using Microsoft.PowerPlatform.Dataverse.Client;
using RevisioneNew.Interfaces;
using RevisioneNew.Services;

namespace RevisioneNew.Controllers
{
    public class QuoteController : Controller
    {
        private readonly ServiceClient _serviceClient;
        private readonly IServiceInterface _service;
        private readonly IQuoteInterface _quoteService;
            List<Quote> quoteList = new List<Quote>();

        public QuoteController(ServiceClient serviceClient, IServiceInterface service,IQuoteInterface quoteService)
        {
            _service = service;
            _serviceClient = serviceClient;
            _quoteService = quoteService;
            QueryExpression queryExpression = new QueryExpression("quote")
            {
                ColumnSet = new ColumnSet("name", "totalamount", "statecode", "createdon","quotenumber", "statuscode")
            };
            EntityCollection quotes = _serviceClient.RetrieveMultiple(queryExpression);

            foreach (var item in quotes.Entities)
            {
                quoteList.Add(new Quote
                {
                    QuoteNumber = item.GetAttributeValue<string>("quotenumber"),
                    QuoteName = item.GetAttributeValue<string>("name"),
                    TotalAmount = item.GetAttributeValue<Money>("totalamount").Value,
                    Status = ((QuoteStateCode)item.GetAttributeValue<OptionSetValue>("statecode").Value).ToString(),
                    CreatedOn = item.GetAttributeValue<DateTime>("createdon"),
                    QuoteId = item.GetAttributeValue<Guid>("quoteid"),
                    Statuscode = ((QuoteStatusCode)item.GetAttributeValue<OptionSetValue>("statuscode").Value).ToString()
                });
            }
        }

        [HttpPost]
        public JsonResult LoadQuoteList()
        {
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

                PagingInfo pagingInfo = new PagingInfo()
                {
                    Count = pageSize,
                    PageNumber = (skip/pageSize)+1,
                    ReturnTotalRecordCount =true
                };
                Datatable<Quote> QuoteData = _quoteService.getQuoteData(draw, sortColumn, pagingInfo, searchValue,sortColumnDirection); 

                return Json(QuoteData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult CloseQuote(string quoteID, string quoteNumber)
        {
            try
            {  
                if (quoteID != null)
                {
                    Guid quoteGuid = new(quoteID);

                    CloseQuoteRequest closeQuoteRequest = new CloseQuoteRequest();
                    closeQuoteRequest.QuoteClose = new Entity("quoteclose")
                    {
                        Attributes =
                        {
                            {"subject",$"Quote Closed (Lost) - {quoteNumber}" },
                            { "quoteid", new EntityReference("quote", quoteGuid) }
                        }
                    };
                    closeQuoteRequest.Status = new OptionSetValue((int)QuoteStatusCode.Lost);
                    _serviceClient.Execute(closeQuoteRequest);

                    return Ok($"Quote is successfully Lost.");
                }
                return BadRequest("Quote not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult WinQuote (string quoteID, string quoteNumber)
        {
            try
            {
                if (quoteID != null)
                {
                    Guid quoteGuid = new(quoteID);

                    WinQuoteRequest winQuoteRequest = new()
                    {
                        QuoteClose = new Entity("quoteclose")
                        {
                            Attributes =
                            {
                                {"subject", $"Quote Won (Won) - {quoteNumber}" },
                                { "quoteid", new EntityReference("quote", quoteGuid) }
                            }
                        },
                        Status = new OptionSetValue(-1)
                    };
                    _serviceClient.Execute(winQuoteRequest);

                    return Ok($"Quote is successfully Won.");
                }
                return BadRequest("Quote not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult ActiveQuote(string quoteID)
        {
            try
            {
                if (quoteID != null)
                {
                    Guid quoteGuid = new Guid(quoteID);

                    SetStateRequest activateQuote = new SetStateRequest()
                    {
                        EntityMoniker = new EntityReference("quote",quoteGuid),
                        State = new OptionSetValue((int)QuoteStateCode.Active),
                        Status = new OptionSetValue((int)QuoteStatusCode.InProgress_Active) //in progress
                    };
                    _serviceClient.Execute(activateQuote);
                    return Ok("Quote is successfully Activated.");

                    //return BadRequest("Order creation failed.");
                }
                return BadRequest("Quote not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult ReviseQuote(string quoteID)
        {
            try
            {
                string reviseQuoteNumber = _quoteService.RevisedQuoteService(quoteID);
                if(reviseQuoteNumber != null)
                {
                    return Ok($"Revised Quote created with Quote number: {reviseQuoteNumber}");
                }
                return BadRequest("There is some error occured");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult DeleteQuote(string quoteID)
        {
            try
            {
                if(quoteID != null)
                {
                    Guid quoteGuid = new Guid(quoteID);
                    _service.Delete("quote", quoteGuid);
                    return Ok("Data deleted successfully.");
                }
                return BadRequest("Quote is not found.");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        [Route("/Quotes")]
        public IActionResult Quotes()
        {
            ViewBag.Quote = "active";
            return View();
        }
    }
}
