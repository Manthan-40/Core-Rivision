using Microsoft.AspNetCore.Mvc.Rendering;
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

        public OpportunityModel GetOpportunityById(Guid opportunityId)
        {
            // Define the columns to retrieve from the opportunity entity
            ColumnSet columnSet = new ColumnSet("name", "createdon", "statecode", "parentcontactid", "parentaccountid", "budgetamount", "description", "decisionmaker", "customerneed", "proposedsolution", "pricelevelid", "transactioncurrencyid");

            // Retrieve the opportunity by ID
            Entity opportunity = GetById("opportunity", opportunityId, columnSet);

            // Map the retrieved opportunity fields to the OpportunityModel
            OpportunityModel result = new OpportunityModel()
            {
                // Topic (maps to "name" in opportunity entity)
                Topic = opportunity.Contains("name") ? opportunity.GetAttributeValue<string>("name") : string.Empty,

                // Status (maps the statecode option set)
                Status = opportunity.Contains("statecode") ? ((OpportunityStateCode)opportunity.GetAttributeValue<OptionSetValue>("statecode").Value).ToString() : string.Empty,

                // Created On
                CreatedON = opportunity.Contains("createdon") ? opportunity.GetAttributeValue<DateTime>("createdon") : DateTime.Now,

                // Opportunity ID
                Id = opportunity.GetAttributeValue<Guid>("opportunityid"),

                // Budget Amount
                BudgetAmount = opportunity.Contains("budgetamount") ? opportunity.GetAttributeValue<Money>("budgetamount").Value : 0m,

                // Description
                Description = opportunity.Contains("description") ? opportunity.GetAttributeValue<string>("description") : string.Empty,

                // Decision Maker
                //DecisionMaker = opportunity.Contains("decisionmaker") ? opportunity.GetAttributeValue<string>("decisionmaker") : string.Empty,

                // Customer Need
                CustomerNeed = opportunity.Contains("customerneed") ? opportunity.GetAttributeValue<string>("customerneed") : string.Empty,

                // Proposed Solution
                ProposedSolution = opportunity.Contains("proposedsolution") ? opportunity.GetAttributeValue<string>("proposedsolution") : string.Empty,

                // Parent Account and Contact
                ParentAccountID = opportunity.Contains("parentaccountid") ? opportunity.GetAttributeValue<EntityReference>("parentaccountid").Id : new Guid(),
                ParentContactID = opportunity.Contains("parentcontactid") ? opportunity.GetAttributeValue<EntityReference>("parentcontactid").Id : new Guid(),

                // Price Level
                PriceLevelID = opportunity.Contains("pricelevelid") ? opportunity.GetAttributeValue<EntityReference>("pricelevelid").Id : new Guid(),

                // Currency
                CurrencyID = opportunity.Contains("transactioncurrencyid") ? opportunity.GetAttributeValue<EntityReference>("transactioncurrencyid").Id : new Guid(),
            };

            // Fetch accounts and map them to select list for UI
            List<AccountModel> accounts = GetAllAccounts();
            List<SelectListItem> accountList = accountSelectList(accounts, opportunity.Contains("parentaccountid") ? opportunity.GetAttributeValue<EntityReference>("parentaccountid") : null);

            // Fetch contacts and map them to select list for UI
            List<ContactModel> contacts = GetAllContacts();
            List<SelectListItem> contactList = contactSelectList(contacts, opportunity.Contains("parentcontactid") ? opportunity.GetAttributeValue<EntityReference>("parentcontactid") : null);

            // Fetch price levels and currencies for dropdowns
            List<PriceLevelModel> priceLevels = GetAllPriceLevels();
            List<SelectListItem> priceListItems = priceLevelSelectList(priceLevels, opportunity.Contains("pricelevelid") ? opportunity.GetAttributeValue<EntityReference>("pricelevelid") : null);

            List<CurrencyModel> currencies = GetAllCurrencies();
            List<SelectListItem> currencyList = currencySelectList(currencies, opportunity.Contains("transactioncurrencyid") ? opportunity.GetAttributeValue<EntityReference>("transactioncurrencyid") : null);



            // Populate the OpportunityModel lists for dropdowns
            result.AccountList = accountList;
            result.ContactList = contactList;
            result.PriceListitems = priceListItems;
            result.CurrencyList = currencyList;

            return result;
        }

        public OpportunityModel GetEmptyModel()
        {
            OpportunityModel model = new OpportunityModel();

            // Fetch all accounts and map them to select list
            List<AccountModel> accounts = GetAllAccounts();
            List<SelectListItem> accountList = accountSelectList(accounts, null);

            // Fetch all contacts and map them to select list
            List<ContactModel> contacts = GetAllContacts();
            List<SelectListItem> contactList = contactSelectList(contacts, null);

            // Fetch all price levels and map them to select list
            List<PriceLevelModel> priceLevels = GetAllPriceLevels();
            List<SelectListItem> priceListItems = priceLevelSelectList(priceLevels, null);

            // Fetch all currencies and map them to select list
            List<CurrencyModel> currencies = GetAllCurrencies();
            List<SelectListItem> currencyList = currencySelectList(currencies, null);

            // Populate the OpportunityModel lists for dropdowns
            model.AccountList = accountList;
            model.ContactList = contactList;
            model.PriceListitems = priceListItems;
            model.CurrencyList = currencyList;

            return model;
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

                    selectList.Add(new SelectListItem
                    {
                        Value = contact.ContactId.ToString(),
                        Text = contact.FullName,
                        Selected = contact.ContactId == currentContact.Id
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
            return selectList;
        }

        public List<SelectListItem> priceLevelSelectList(List<PriceLevelModel> pricelevels, EntityReference currentPricelevel)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var pricelevel in pricelevels)
            {
                if (currentPricelevel != null)
                {

                    selectList.Add(new SelectListItem
                    {
                        Value = pricelevel.PriceLevelId.ToString(),
                        Text = pricelevel.PriceLevelName,
                        Selected = pricelevel.PriceLevelId == currentPricelevel.Id
                    });

                }
                else
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = pricelevel.PriceLevelId.ToString(),
                        Text = pricelevel.PriceLevelName,
                    });
                }
            }
            return selectList;
        }


        public List<SelectListItem> currencySelectList(List<CurrencyModel> currencies, EntityReference currentCurrency)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var currency in currencies)
            {
                if (currentCurrency != null)
                {

                    selectList.Add(new SelectListItem
                    {
                        Value = currency.CurrencyId.ToString(),
                        Text = currency.CurrencyName,
                        Selected = currency.CurrencyId == currentCurrency.Id
                    });

                }
                else
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = currency.CurrencyId.ToString(),
                        Text = currency.CurrencyName,
                    });
                }
            }
            return selectList;
        }

        public void CreateOpportunity(OpportunityModel model)
        {
            Entity newOpportunity = new Entity("opportunity");

            // Set core fields for the opportunity
            newOpportunity["name"] = model.Topic; // Topic of the opportunity
            newOpportunity["description"] = model.Description; // Description of the opportunity
            newOpportunity["budgetamount"] = new Money(model.BudgetAmount ?? 0); // Budget amount
            //newOpportunity["decisionmaker"] = model.DecisionMaker; // Decision maker
            newOpportunity["customerneed"] = model.CustomerNeed; // Customer needs
            newOpportunity["proposedsolution"] = model.ProposedSolution; // Proposed solution

            // Check if ParentAccountID is set and map it to parentaccountid field
            if (model.ParentAccountID.HasValue)
            {
                newOpportunity["parentaccountid"] = new EntityReference("account", model.ParentAccountID.Value);
            }

            // Check if ParentContactID is set and map it to parentcontactid field
            if (model.ParentContactID.HasValue)
            {
                newOpportunity["parentcontactid"] = new EntityReference("contact", model.ParentContactID.Value);
            }

            // Check if PriceLevelID is set and map it to pricelevelid field
            if (model.PriceLevelID.HasValue)
            {
                newOpportunity["pricelevelid"] = new EntityReference("pricelevel", model.PriceLevelID.Value);
            }

            // Set currency for the opportunity
            newOpportunity["transactioncurrencyid"] = new EntityReference("transactioncurrency", model.CurrencyID);

            // Create the opportunity in CRM
            Create(newOpportunity);
        }


        public void UpdateOpportunity(OpportunityModel model)
        {
            Entity opportunityEntity = new Entity("opportunity", model.Id);

            // Set core fields for the opportunity
            opportunityEntity["name"] = model.Topic; // Topic of the opportunity
            opportunityEntity["description"] = model.Description; // Description of the opportunity
            opportunityEntity["budgetamount"] = new Money(model.BudgetAmount ?? 0); // Budget amount
            //opportunityEntity["decisionmaker"] = model.DecisionMaker; // Decision maker
            opportunityEntity["customerneed"] = model.CustomerNeed; // Customer needs
            opportunityEntity["proposedsolution"] = model.ProposedSolution; // Proposed solution

            // Check if ParentAccountID is set and map it to parentaccountid field
            if (model.ParentAccountID.HasValue)
            {
                opportunityEntity["parentaccountid"] = new EntityReference("account", model.ParentAccountID.Value);
            }

            // Check if ParentContactID is set and map it to parentcontactid field
            if (model.ParentContactID.HasValue)
            {
                opportunityEntity["parentcontactid"] = new EntityReference("contact", model.ParentContactID.Value);
            }

            // Check if PriceLevelID is set and map it to pricelevelid field
            if (model.PriceLevelID.HasValue)
            {
                opportunityEntity["pricelevelid"] = new EntityReference("pricelevel", model.PriceLevelID.Value);
            }

            // Set currency for the opportunity
            opportunityEntity["transactioncurrencyid"] = new EntityReference("transactioncurrency", model.CurrencyID);

            // Update the opportunity in CRM
            Update(opportunityEntity);
        }


        public void DeleteOpportunity(Guid opportunityId)
        {
            Delete("opportunity", opportunityId);
        }
    }
}
