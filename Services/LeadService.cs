using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Interfaces;
using RevisioneNew.Models;
using System.Xml.Linq;

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
                TopicName = Lead.Contains("subject") ? Lead.GetAttributeValue<string>("subject") : string.Empty,
                FullName = Lead.Contains("fullname") ? Lead.GetAttributeValue<string>("fullname") : string.Empty,

                // Status: Maps the statecode option set
                Status = Lead.Contains("statecode") ? ((LeadStateCode)Lead.GetAttributeValue<OptionSetValue>("statecode").Value).ToString() : string.Empty,

                // Created On
                CreatedON = Lead.Contains("createdon") ? Lead.GetAttributeValue<DateTime>("createdon") : DateTime.Now,

                // Lead ID
                Id = Lead.GetAttributeValue<Guid>("leadid"),

                // Contact Details
                FirstName = Lead.Contains("firstname") ? Lead.GetAttributeValue<string>("firstname") : string.Empty,
                LastName = Lead.Contains("lastname") ? Lead.GetAttributeValue<string>("lastname") : string.Empty,
                CompanyName = Lead.Contains("companyname") ? Lead.GetAttributeValue<string>("companyname") : string.Empty,
                JobTitle = Lead.Contains("jobtitle") ? Lead.GetAttributeValue<string>("jobtitle") : string.Empty,
                EmailAddress = Lead.Contains("emailaddress1") ? Lead.GetAttributeValue<string>("emailaddress1") : string.Empty,
                Telephone = Lead.Contains("telephone1") ? Lead.GetAttributeValue<string>("telephone1") : string.Empty,

                // Lead-associated account or contact
                AccountName = Lead.Contains("parentaccountid") ? Lead.GetAttributeValue<EntityReference>("parentaccountid").Name : string.Empty,
                ContactName = Lead.Contains("parentcontactid") ? Lead.GetAttributeValue<EntityReference>("parentcontactid").Name : string.Empty,

                AccountID = Lead.Contains("parentaccountid") ? Lead.GetAttributeValue<EntityReference>("parentaccountid").Id :new Guid(),
                ContactID = Lead.Contains("parentcontactid") ? Lead.GetAttributeValue<EntityReference>("parentcontactid").Id :new Guid(),

                // Other Details
                EstimatedBudget = Lead.Contains("budgetamount") ? Lead.GetAttributeValue<Money>("budgetamount").Value : 0m,
                Description = Lead.Contains("description") ? Lead.GetAttributeValue<string>("description") : string.Empty
            };

            List<AccountModel> accounts = GetAllAccounts();
            List<SelectListItem> accountList = accountSelectList(accounts, Lead.GetAttributeValue<EntityReference>("parentaccountid"));

            List<ContactModel> contacts = GetAllContacts();
            List<SelectListItem> contactslist = contactSelectList(contacts, Lead.GetAttributeValue<EntityReference>("parentcontactid"));

            result.AccountList = accountList;
            result.ContactList = contactslist;
            
            return result;  
        }

        public List<SelectListItem> accountSelectList(List<AccountModel> accounts, EntityReference currentAccount)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var account in accounts)
            {
                if (currentAccount != null)
                {

                        selectList.Add(new SelectListItem
                        {
                            Value = account.AccountId.ToString(),
                            Text = account.AccountName,
                            Selected = account.AccountId == currentAccount.Id
                        });
                    
                }
                else
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = account.AccountId.ToString(),
                        Text = account.AccountName,
                    });
                }
            }
            return selectList;
        }

        public List<SelectListItem> contactSelectList(List<ContactModel> contacts, EntityReference currentContact)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var contact in contacts)
            {
                if (currentContact != null)
                {
                    if (contact.ContactId == currentContact.Id)
                    {
                        selectList.Add(new SelectListItem
                        {
                            Value = contact.ContactId.ToString(),
                            Text = contact.FullName,
                            Selected = true
                        });
                    }
                    else
                    {
                        selectList.Add(new SelectListItem
                        {
                            Value = contact.ContactId.ToString(),
                            Text = contact.FullName,
                        });
                    }
                }
                else
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = contact.ContactId.ToString(),
                        Text = contact.FullName,
                    });
                }
            }
            return selectList;
        }

        public LeadModel GetEmptyModel()
        {
            LeadModel model = new LeadModel();

            List<AccountModel> accounts = GetAllAccounts();
            List<SelectListItem> accountList = accountSelectList(accounts, null);

            List<ContactModel> contacts = GetAllContacts();
            List<SelectListItem> contactslist = contactSelectList(contacts, null);

            model.AccountList = accountList;
            model.ContactList = contactslist;

            return model;
        }

        public void CreateLead(LeadModel model)
        {
            Entity newLead = new Entity("lead");

            newLead["subject"] = model.TopicName;
            newLead["firstname"] = model.FirstName;
            newLead["lastname"] = model.LastName;
            newLead["jobtitle"] = model.JobTitle;
            newLead["telephone1"] = model.Telephone;
            newLead["emailaddress1"] = model.EmailAddress;
            newLead["companyname"] = model.CompanyName;
            newLead["description"] = model.Description;
            newLead["budgetamount"] = model.EstimatedBudget;
            if (model.AccountID.HasValue)
            {
                newLead["parentaccountid"] = new EntityReference("account", model.AccountID.Value);
            }
            if (model.ContactID.HasValue)
            {
                newLead["parentcontactid"] = new EntityReference("contact", model.ContactID.Value);
            }

            Create(newLead);

        }

        public void UpdateLead(LeadModel model)
        {
            Entity newLead = new Entity("lead",model.Id);

            newLead["subject"] = model.TopicName;
            newLead["firstname"] = model.FirstName;
            newLead["lastname"] = model.LastName;
            newLead["jobtitle"] = model.JobTitle;
            newLead["telephone1"] = model.Telephone;
            newLead["emailaddress1"] = model.EmailAddress;
            newLead["companyname"] = model.CompanyName;
            newLead["description"] = model.Description;
            newLead["budgetamount"] = model.EstimatedBudget;
            if (model.AccountID.HasValue)
            {
                newLead["parentaccountid"] = new EntityReference("account", model.AccountID.Value);
            }
            if (model.ContactID.HasValue)
            {
                newLead["parentcontactid"] = new EntityReference("contact", model.ContactID.Value);
            }

            Update(newLead);
        }

        public void DeleteLead(Guid LeadID)
        {
            Delete("lead", LeadID);
        }
    }
}
