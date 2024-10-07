using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Models;

namespace RevisioneNew.Interfaces
{
    public interface IQuoteInterface : IServiceInterface
    {
        public string RevisedQuoteService(string quote);

        public Datatable<Quote> getQuoteData(string Draw, string? sortColumn, PagingInfo pagingInfo, string searchValue = null, string sortOrder = "asc");
    }
}
