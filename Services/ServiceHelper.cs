using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using RevisioneNew.Interfaces;
using RevisioneNew.Models;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.AspNetCore.Http.HttpResults;
using NuGet.Protocol.Plugins;

namespace RevisioneNew.Services
{
    public class ServiceHelper : IServiceInterface
    {
        protected readonly ServiceClient _serviceClient;
        public ServiceHelper(ServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public void Create(Entity entity)
        {
            try
            {
                _serviceClient.Create(entity);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Delete(string entityName, Guid entityID)
        {
            try
            {
                _serviceClient.Delete(entityName, entityID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Microsoft.Xrm.Sdk.Entity Get(string entityName, ColumnSet columnSet, LogicalOperator filterOperator = LogicalOperator.Or, ConditionExpression[] conditions = null)
        {

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet = columnSet;

            if (conditions != null)
            {
                FilterExpression filters = new FilterExpression(filterOperator);

                foreach (var item in conditions)
                {
                    filters.AddCondition(item);
                }
                query.Criteria.AddFilter(filters);
            }
            EntityCollection entities = _serviceClient.RetrieveMultiple(query);
            return entities.Entities[0];

        }

        public EntityCollection GetAll(string entityName, ColumnSet columnSet, PagingInfo pagingInfo = null, string? sortColumn = null, string[] Searchcolumns = null, string searchValue = null, string sortOrder = "asc")
        {
            QueryExpression getAllDataQuery = new QueryExpression(entityName);
            OrderType order = sortOrder == "asc" ? OrderType.Ascending : OrderType.Descending;
            if (pagingInfo != null)
            {
                getAllDataQuery.PageInfo = pagingInfo;
            }
            getAllDataQuery.ColumnSet = columnSet;

            if (!string.IsNullOrEmpty(searchValue) && Searchcolumns.Length > 0)
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

        public List<AccountModel> GetAllAccounts()
        {
            EntityCollection acountEntities = GetAll("account", new ColumnSet("name"));
            List<AccountModel> accounts = new List<AccountModel>();
            foreach (var a in acountEntities.Entities)
            {
                accounts.Add(new AccountModel
                {
                    AccountId = a.Id,
                    AccountName = a.Contains("name") ? a.GetAttributeValue<string>("name") : string.Empty
                });
            }
            return accounts;
        }

        public List<ContactModel> GetAllContacts()
        {
            EntityCollection contactEntities = GetAll("contact", new ColumnSet("fullname"));
            List<ContactModel> contacts = new List<ContactModel>();
            foreach (var a in contactEntities.Entities)
            {
                contacts.Add(new ContactModel
                {
                    ContactId = a.Id,
                    FullName = a.Contains("fullname") ? a.GetAttributeValue<string>("fullname") : string.Empty
                });
            }
            return contacts;
        }

        public Microsoft.Xrm.Sdk.Entity GetById(string entityName, Guid entityID, ColumnSet columnSet)
        {
            try
            {
                return _serviceClient.Retrieve(entityName, entityID, columnSet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Microsoft.Xrm.Sdk.Entity GetCurrentUser(string email)
        {
            QueryExpression query = new QueryExpression("contact");
            query.ColumnSet = new ColumnSet("firstname", "lastname", "emailaddress1", "md_isfirsttimelogin");
            query.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, email);

            EntityCollection entities = _serviceClient.RetrieveMultiple(query);

            return entities.Entities.Count > 0 ? entities.Entities[0] : new Entity();
        }

        public bool SendMail(Microsoft.Xrm.Sdk.Entity Sender, Microsoft.Xrm.Sdk.Entity Receiver, Microsoft.Xrm.Sdk.Entity Template = null)
        {
            try
            {
                Entity ReceiverContact = new Entity();
                if (Receiver.LogicalName == "systemuser")
                {
                    string sendMail = Receiver.Contains("internalemailaddress") ? Receiver.GetAttributeValue<string>("internalemailaddress") : String.Empty;
                    ReceiverContact = Get("contact", new ColumnSet(false), LogicalOperator.And, [new ConditionExpression("emailaddress1", ConditionOperator.Equal, sendMail)]);
                }
                else
                {
                    ReceiverContact = Receiver;
                }
                //create activityparty
                Entity Fromparty = new Entity("activityparty");
                Entity Toparty = new Entity("activityparty");

                //To set to Contact
                Toparty["partyid"] = new EntityReference(ReceiverContact.LogicalName, ReceiverContact.Id);

                //From set to User
                Fromparty["partyid"] = new EntityReference(Sender.LogicalName, Sender.Id);

                //create email Object and set attributes

                Entity email = new Entity("email");
                email["from"] = new Entity[] { Fromparty };
                email["to"] = new Entity[] { Toparty };
                email["directioncode"] = true;

                //setting the Regarding as Contact
                email["regardingobjectid"] = new EntityReference(ReceiverContact.LogicalName, ReceiverContact.Id);

                if (Template != null)
                {
                    SendEmailFromTemplateRequest emailUsingTemplateReq = new SendEmailFromTemplateRequest
                    {
                        // The Email Object created
                        Target = email,

                        // The Email Template Id
                        TemplateId = Template.Id,

                        // Template Regarding Record Id
                        RegardingId = ReceiverContact.Id,

                        //Template Regarding Record’s Logical Name
                        RegardingType = ReceiverContact.LogicalName
                    };
                    SendEmailFromTemplateResponse emailResp = (SendEmailFromTemplateResponse)_serviceClient.Execute(emailUsingTemplateReq);
                    return true;
                }
                else
                {
                    email["subject"] = "Welcome to Portal";
                    email["description"] = "Hello " + Receiver.GetAttributeValue<string>("firstname") + ", Welcome to our Portal.\n\nThanks for choosing us.";
                    Guid emailId = _serviceClient.Create(email);
                    SendEmailRequest emailReq = new SendEmailRequest()
                    {
                        EmailId = emailId,
                        TrackingToken = "",
                        IssueSend = true
                    };
                    SendEmailResponse sendEmailResp = (SendEmailResponse)_serviceClient.Execute(emailReq);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public bool SendSupportMail(SupportSignIn ssi)
        {
            Entity Sender = Get("queue", new ColumnSet("name", "emailaddress"), LogicalOperator.And, [new ConditionExpression("emailaddress", ConditionOperator.Equal, "supportsignin@Mand2113.onmicrosoft.com")]);

            Entity ReceiverAccount = Get("account", new ColumnSet("name", "primarycontactid"), LogicalOperator.And, [new ConditionExpression("name", ConditionOperator.Equal, "Support to Sign In Account")]);

            EntityReference ReceiverContact = ReceiverAccount.Contains("primarycontactid") ? ReceiverAccount.GetAttributeValue<EntityReference>("primarycontactid") : new EntityReference();

            if (ReceiverContact.Id != null && Sender.Id != null)
            {
                Entity Fromparty = new Entity("activityparty");
                Entity Toparty = new Entity("activityparty");

                //To set to Contact
                Toparty["partyid"] = new EntityReference(ReceiverContact.LogicalName, ReceiverContact.Id);

                //From set to User
                Fromparty["partyid"] = new EntityReference(Sender.LogicalName, Sender.Id);

                //create email Object and set attributes

                Entity email = new Entity("email");
                email["from"] = new Entity[] { Fromparty };
                email["to"] = new Entity[] { Toparty };
                email["directioncode"] = true;

                email["subject"] = ssi.Subject;
                email["description"] = "<p><b>Email: </b>" + ssi.Email + "</p><p><b>Description:</b> " + ssi.Description + "</p>";

                //setting the Regarding as Contact
                email["regardingobjectid"] = new EntityReference(ReceiverContact.LogicalName, ReceiverContact.Id);

                Guid emailId = _serviceClient.Create(email);
                SendEmailRequest emailReq = new SendEmailRequest()
                {
                    EmailId = emailId,
                    TrackingToken = "",
                    IssueSend = true
                };
                SendEmailResponse sendEmailResp = (SendEmailResponse)_serviceClient.Execute(emailReq);
                return true;
            }
            return false;

        }

        public ServiceClient Service()
        {
            return _serviceClient;
        }

        public void Update(Entity entity)
        {
            try
            {
                _serviceClient.Update(entity);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
