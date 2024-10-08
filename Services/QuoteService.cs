using Microsoft.Crm.Sdk.Messages;
using Microsoft.Graph;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Interfaces;
using RevisioneNew.Models;

namespace RevisioneNew.Services
{
    public class QuoteService : ServiceHelper, IQuoteInterface
    {
        public QuoteService(ServiceClient serviceClient) : base(serviceClient)
        {
        }

        public Datatable<Quote> getQuoteData(string Draw, string? sortColumn, PagingInfo pagingInfo, string searchValue = null, string sortOrder = "asc")
        {
            EntityCollection allQuote = GetAll("quote", new ColumnSet("name", "totalamount", "statecode", "createdon", "quotenumber", "statuscode"), pagingInfo, sortColumn, ["name", "quotenumber"], searchValue, sortOrder);

            List<Quote> quoteList = new List<Quote>();
            foreach (var item in allQuote.Entities)
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
            Datatable<Quote> QuoteTable = new Datatable<Quote>()
            {
                Draw = Draw,
                RecordsTotal = allQuote.TotalRecordCount,
                RecordsFiltered = allQuote.TotalRecordCount,
                Data = quoteList
            };
            return QuoteTable;
        }

        public string RevisedQuoteService(string quote)
        {
            if (quote != null)
            {
                Guid quoteGuid = new Guid(quote);
                ReviseQuoteRequest reviseQuoteRequest = new ReviseQuoteRequest();
                reviseQuoteRequest.QuoteId = quoteGuid;
                reviseQuoteRequest.ColumnSet = new ColumnSet("quotenumber");
                ReviseQuoteResponse resp = (ReviseQuoteResponse)_serviceClient.Execute(reviseQuoteRequest);
                return resp.Entity.GetAttributeValue<string>("quotenumber");
            }
            return null;
        }

    }
}
