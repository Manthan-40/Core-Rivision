
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Models;

namespace RevisioneNew.Interfaces
{
    public interface IServiceInterface
    {
        public void Delete(string entityName, Guid entityID);

        public EntityCollection GetAll(string entityName, ColumnSet columnSet, PagingInfo pagingInfo = null, string? sortColumn=null, string[] Searchcolumns=null, string searchValue = null, string sortOrder = "asc");

        public Entity GetById(string entityName, Guid entityID, ColumnSet columnSet);

        public Entity Get(string entityName, ColumnSet columnSet, LogicalOperator filterOperator = LogicalOperator.Or, ConditionExpression[] conditions =null);
        public Entity GetCurrentUser(string email);

        public void Update(Entity entity);
        public void Create(Entity entity);
        public bool SendMail(Entity Sender, Entity Receiver, Entity Template = null );
        public bool SendSupportMail(SupportSignIn ssi);

    }
}
