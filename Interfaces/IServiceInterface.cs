using Microsoft.Graph;

namespace RevisioneNew.Interfaces
{
    public interface IServiceInterface
    {
        public void Delete(string entityName, Guid entityID); 
    }
}
