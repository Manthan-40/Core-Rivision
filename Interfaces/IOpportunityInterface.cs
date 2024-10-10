    using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Models;

namespace RevisioneNew.Interfaces
{
    public interface IOpportunityInterface
    {
        public Datatable<OpportunityModel> GetaAllOpportunities(string Draw, string? sortColumn, PagingInfo pagingInfo, string searchValue = null, string sortOrder = "asc");

        // Method to get opportunity details by ID
        public OpportunityModel GetOpportunityById(Guid opportunityId);

        // Method to return an empty OpportunityModel (used for new opportunity creation)
        public OpportunityModel GetEmptyModel();

        // Method to create a new opportunity
        public void CreateOpportunity(OpportunityModel model);

        // Method to update an existing opportunity
        public void UpdateOpportunity(OpportunityModel model);

        // Method to delete an opportunity by ID
        public void DeleteOpportunity(Guid opportunityId);
    }

}
