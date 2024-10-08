using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Models;

namespace RevisioneNew.Interfaces
{
    public interface ILeadInterface
    {
        public Datatable<LeadModel> GetaAllLeades(string Draw, string? sortColumn, PagingInfo pagingInfo, string searchValue = null, string sortOrder = "asc");

        public LeadModel GetLeadById(Guid LeadID);
    }
}
