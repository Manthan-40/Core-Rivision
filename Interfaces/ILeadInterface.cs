using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Models;

namespace RevisioneNew.Interfaces
{
    public interface ILeadInterface
    {
        public Datatable<LeadModel> GetaAllLeades(string Draw, string? sortColumn, PagingInfo pagingInfo, string searchValue = null, string sortOrder = "asc");

        public LeadModel GetLeadById(Guid LeadID);
        public LeadModel GetEmptyModel();

        public void CreateLead(LeadModel model);
        public void UpdateLead(LeadModel model);
        public void DeleteLead(Guid LeadID);
    }
}
