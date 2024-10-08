using Microsoft.Crm.Sdk.Messages;
using Microsoft.Graph;
using Microsoft.PowerPlatform.Dataverse.Client;
using RevisioneNew.Interfaces;
using RevisioneNew.Models;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;

namespace RevisioneNew.Services
{
    public class ServiceHelper : IServiceInterface
    {
        protected readonly ServiceClient _serviceClient;
        public ServiceHelper(ServiceClient serviceClient) {
            _serviceClient = serviceClient;
        }

        public void Delete(string entityName, Guid entityID)
        {
            _serviceClient.Delete(entityName, entityID);
        }

        public EntityCollection GetAll(string entityName, ColumnSet columnSet, PagingInfo pagingInfo, string? sortColumn, string[]Searchcolumns, string searchValue = null, string sortOrder = "asc")
        {
            QueryExpression getAllDataQuery = new QueryExpression(entityName);
            OrderType order = sortOrder == "asc" ? OrderType.Ascending : OrderType.Descending;

            getAllDataQuery.PageInfo = pagingInfo;

            getAllDataQuery.ColumnSet = columnSet;

            if (!string.IsNullOrEmpty(searchValue) && Searchcolumns.Length>0)
            {
                FilterExpression filters = new FilterExpression(LogicalOperator.Or);

                foreach (var name in Searchcolumns)
                {
                    filters.AddCondition(name, ConditionOperator.Like, "%" + searchValue + "%"); 
                }

                getAllDataQuery.Criteria.AddFilter(filters);
            }


            if (sortColumn != null)
            {
                getAllDataQuery.AddOrder(sortColumn, order);
            }
            return _serviceClient.RetrieveMultiple(getAllDataQuery);
        }

        public Microsoft.Xrm.Sdk.Entity GetById(string entityName, Guid entityID, ColumnSet columnSet)
        {
            return _serviceClient.Retrieve(entityName, entityID, columnSet);
        }

        public ServiceClient Service()
        {
            return _serviceClient;
        }
    }
}
