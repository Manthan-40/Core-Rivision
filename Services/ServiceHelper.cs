using Microsoft.Crm.Sdk.Messages;
using Microsoft.Graph;
using Microsoft.PowerPlatform.Dataverse.Client;
using RevisioneNew.Interfaces;
using RevisioneNew.Models;
using Microsoft.Xrm.Sdk.Query;

namespace RevisioneNew.Services
{
    public class ServiceHelper : IServiceInterface
    {
        private readonly ServiceClient _serviceClient;
        public ServiceHelper(ServiceClient serviceClient) {
            _serviceClient = serviceClient;
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
