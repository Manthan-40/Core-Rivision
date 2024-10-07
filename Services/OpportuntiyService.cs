using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Interfaces;

namespace RevisioneNew.Services
{
    public class OpportuntiyService : ServiceHelper, IOpportunityInterface
    {
        public OpportuntiyService(ServiceClient serviceClient) : base(serviceClient)
        {
        }

        public EntityCollection GetaAllOpportunities(string? sortColumn, PagingInfo pagingInfo, string searchValue = null, string sortOrder = "asc")
        {
            QueryExpression getAllOpportunity = new QueryExpression("opportunity");
            OrderType order = sortOrder == "asc"?OrderType.Ascending : OrderType.Descending;
            
            getAllOpportunity.PageInfo = pagingInfo;

            getAllOpportunity.ColumnSet = new ColumnSet("name", "description", "statecode", "createdon");

            if(searchValue != null)
            {
                FilterExpression filters = new FilterExpression(LogicalOperator.Or);

                filters.AddCondition("name",ConditionOperator.Like,"%"+searchValue+"%");
                filters.AddCondition("description",ConditionOperator.Like,"%"+searchValue + "%");

                getAllOpportunity.Criteria.AddFilter(filters);
            }


            if(sortColumn != null)
            {
                getAllOpportunity.AddOrder(sortColumn,order);
            }

            EntityCollection allOpportunities = _serviceClient.RetrieveMultiple(getAllOpportunity);

            return allOpportunities;
        }
    }
}
