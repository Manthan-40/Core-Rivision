using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Models;

namespace RevisioneNew.Interfaces
{
    public interface IOpportunityInterface
    {
        public Datatable<OpportunityModel> GetaAllOpportunities(string Draw, string? sortColumn, PagingInfo pagingInfo, string searchValue = null, string sortOrder = "asc");
    }
}
