
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace RevisioneNew.Interfaces
{
    public interface IServiceInterface
    {
        public void Delete(string entityName, Guid entityID);

        public EntityCollection GetAll(string entityName, ColumnSet columnSet, PagingInfo pagingInfo, string? sortColumn, string[] Searchcolumns, string searchValue = null, string sortOrder = "asc");

        public Entity GetById(string entityName, Guid entityID, ColumnSet columnSet);
    }
}
