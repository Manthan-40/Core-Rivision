using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace RevisioneNew.Interfaces
{
    public interface IOpportunityInterface
    {
        public EntityCollection GetaAllOpportunities(string? sortColumn, PagingInfo pagingInfo, string searchValue = null, string sortOrder = "asc");
    }
}
