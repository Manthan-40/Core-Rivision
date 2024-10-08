using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Interfaces;
using RevisioneNew.Models;

namespace RevisioneNew.Services
{
    public class OpportuntiyService : ServiceHelper, IOpportunityInterface
    {
        public OpportuntiyService(ServiceClient serviceClient) : base(serviceClient)
        {
        }

        public Datatable<OpportunityModel> GetaAllOpportunities(string Draw, string? sortColumn, PagingInfo pagingInfo, string searchValue = null, string sortOrder = "asc")
        {
            EntityCollection allOpportunities = GetAll("opportunity", new ColumnSet("name", "description", "statecode", "createdon"), pagingInfo, sortColumn, ["name", "description"], searchValue, sortOrder);

            List<OpportunityModel> opportunityList = new List<OpportunityModel>();
            foreach (var item in allOpportunities.Entities)
            {
                opportunityList.Add(new OpportunityModel
                {
                    Topic = item.GetAttributeValue<string>("name"),
                    Description = item.GetAttributeValue<string>("description"),
                    Status = ((OpportunityStateCode)item.GetAttributeValue<OptionSetValue>("statecode").Value).ToString(),
                    CreatedON = item.GetAttributeValue<DateTime>("createdon"),
                    Id = item.GetAttributeValue<Guid>("opportunityid")
                });
            }
            Datatable<OpportunityModel> resultTable = new Datatable<OpportunityModel>
            {
                Draw = Draw,
                RecordsFiltered = allOpportunities.TotalRecordCount,
                RecordsTotal = allOpportunities.TotalRecordCount,
                Data = opportunityList
            };
            return resultTable;
        }
    }
}
