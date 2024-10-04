using Microsoft.AspNetCore.Mvc;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Linq.Dynamic.Core;
using RevisioneNew.Models;
using Microsoft.PowerPlatform.Dataverse.Client;
//using Microsoft.Graph;

namespace RevisioneNew.Controllers
{
    public class QuoteController : Controller
    {
        private readonly ServiceClient _serviceClient;
        List<Quote> quoteList = new List<Quote>();

        public QuoteController(ServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
            QueryExpression queryExpression = new QueryExpression("quote")
            {
                ColumnSet = new ColumnSet("name", "totalamount", "statecode", "createdon")
            };
            EntityCollection quotes = _serviceClient.RetrieveMultiple(queryExpression);

            foreach (var item in quotes.Entities)
            {
                quoteList.Add(new Quote
                {
                    QuoteName = item.GetAttributeValue<string>("name"),
                    TotalAmount = item.GetAttributeValue<Money>("totalamount").Value,
                    Status = ((QuoteStateCode)item.GetAttributeValue<OptionSetValue>("statecode").Value).ToString(),
                    CreatedOn = item.GetAttributeValue<DateTime>("createdon"),
                    QuoteId = item.GetAttributeValue<Guid>("quoteid")
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

                var quoteData = quoteList.AsQueryable();
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    quoteData = quoteData.OrderBy(sortColumn + " " + sortColumnDirection);

                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    quoteData = quoteData.Where(m => m.QuoteName.Contains(searchValue));
                }
                recordsTotal = quoteData.Count();
                var data = quoteData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult CloseQuote(string quoteID, bool isWon)
        {
            try
            {
                if (quoteID != null)
                {
                    Guid quoteGuid = new Guid(quoteID);

                    CloseQuoteRequest closeQuoteRequest = new CloseQuoteRequest();
                    closeQuoteRequest.QuoteClose = new Entity("quoteclose")
                    {
                        Attributes =
                    {
                        { "quoteid", new EntityReference("quote", quoteGuid) }
                    }
                    };

                    // Use -1 for default status or specific values for Won/Lost statuses
                    closeQuoteRequest.Status = isWon ? new OptionSetValue(4) : new OptionSetValue(5); // 4: Won, 5: Lost
                    _serviceClient.Execute(closeQuoteRequest);

                    return Ok($"Quote is successfully {(isWon ? "Won" : "Lost")}.");
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

                    Entity closeQuoteUpdate = new("quote")
                    {
                        Id = quoteGuid,
                        Attributes = {
                            // Active
                            { "statecode", new OptionSetValue(1) },
                            // In Progress
                            { "statuscode", new OptionSetValue(2) }
                        }
                    };

                    _serviceClient.Update(closeQuoteUpdate);
                    return Ok("Order successfully created from Quote.");

                    //return BadRequest("Order creation failed.");
                }
                return BadRequest("Quote not found");
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
