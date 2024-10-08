using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Interfaces;
using RevisioneNew.Models;

namespace RevisioneNew.Services
{
    public class LeadService : ServiceHelper, ILeadInterface
    {
        private ServiceClient _serviceClient;
        public LeadService(ServiceClient serviceClient) : base(serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public Datatable<LeadModel> GetaAllLeades(string Draw, string? sortColumn, PagingInfo pagingInfo, string searchValue = null, string sortOrder = "asc")
        {
            EntityCollection allLeades = GetAll("lead", new ColumnSet("subject", "fullname", "statecode", "createdon"), pagingInfo, sortColumn, ["subject", "fullname"], searchValue, sortOrder);
            List<LeadModel> leadList = new List<LeadModel>();
            foreach (var item in allLeades.Entities)
            {
                leadList.Add(new LeadModel
                {
                    TopicName = item.GetAttributeValue<string>("subject"),
                    FullName = item.GetAttributeValue<string>("fullname"),
                    Status = ((LeadStateCode)item.GetAttributeValue<OptionSetValue>("statecode").Value).ToString(),
                    CreatedON = item.GetAttributeValue<DateTime>("createdon"),
                    Id = item.GetAttributeValue<Guid>("leadid")
                });
            }

            return new Datatable<LeadModel> { Draw = Draw, RecordsFiltered = allLeades.TotalRecordCount, RecordsTotal = allLeades.TotalRecordCount, Data = leadList };
        }

        public LeadModel GetLeadById(Guid LeadID)
        {
            ColumnSet columnSet = new ColumnSet("subject","fullname","createdon","statecode", "parentcontactid", "parentaccountid", "budgetamount", "description", "firstname", "lastname", "jobtitle", "telephone1", "emailaddress1","statecode", "companyname");
            Entity Lead = GetById("lead", LeadID, columnSet);

            LeadModel result = new LeadModel()
            {
                // Topic and Full Name
                TopicName = Lead.Contains("subject") ? Lead.GetAttributeValue<string>("subject") : "Not Available",
                FullName = Lead.Contains("fullname") ? Lead.GetAttributeValue<string>("fullname") : "Not Available",

                // Status: Maps the statecode option set
                Status = Lead.Contains("statecode") ? ((LeadStateCode)Lead.GetAttributeValue<OptionSetValue>("statecode").Value).ToString() : "Status is not given.",

                // Created On
                CreatedON = Lead.Contains("createdon") ? Lead.GetAttributeValue<DateTime>("createdon") : DateTime.Now,

                // Lead ID
                Id = Lead.GetAttributeValue<Guid>("leadid"),

                // Contact Details
                FirstName = Lead.Contains("firstname") ? Lead.GetAttributeValue<string>("firstname") : "Not Available",
                LastName = Lead.Contains("lastname") ? Lead.GetAttributeValue<string>("lastname") : "Not Available",
                CompanyName = Lead.Contains("companyname") ? Lead.GetAttributeValue<string>("companyname") : "Not Available",
                JobTitle = Lead.Contains("jobtitle") ? Lead.GetAttributeValue<string>("jobtitle") : "Not Available",
                EmailAddress = Lead.Contains("emailaddress1") ? Lead.GetAttributeValue<string>("emailaddress1") : "Not Available",
                Telephone = Lead.Contains("telephone1") ? Lead.GetAttributeValue<string>("telephone1") : "Not Available",

                // Lead-associated account or contact
                AccountName = Lead.Contains("parentaccountid") ? Lead.GetAttributeValue<EntityReference>("parentaccountid").Name : "Not Available",
                ContactName = Lead.Contains("parentcontactid") ? Lead.GetAttributeValue<EntityReference>("parentcontactid").Name : "Not Available",

                // Other Details
                EstimatedBudget = Lead.Contains("budgetamount") ? Lead.GetAttributeValue<Money>("budgetamount").Value : 0m,
                Description = Lead.Contains("description") ? Lead.GetAttributeValue<string>("description") : "Not Available"
            };
                return result;
            
        }
    }
}
