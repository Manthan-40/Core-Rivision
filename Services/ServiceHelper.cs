using Microsoft.Crm.Sdk.Messages;
using Microsoft.Graph;
using Microsoft.PowerPlatform.Dataverse.Client;
using RevisioneNew.Interfaces;
using RevisioneNew.Models;
using Microsoft.Xrm.Sdk.Query;

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

        public ServiceClient Service()
        {
            return _serviceClient;
        }
    }
}
