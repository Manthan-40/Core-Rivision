using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Interfaces;
using RevisioneNew.Models;

namespace RevisioneNew.Services
{
    public class OrderService : ServiceHelper, IOrderInterface
    {
        public OrderService(ServiceClient serviceClient) : base(serviceClient)
        {
        }

        public bool FulfillOrder(Guid orderid)
        {
            try
            {
                FulfillSalesOrderRequest fulfillOrderRequest = new FulfillSalesOrderRequest
                {
                    OrderClose = new Entity("orderclose")
                    {
                        Attributes =
                        {
                            {"subject", "Order fulfilled" },
                            { "salesorderid", new EntityReference("salesorder", orderid) }
                        }
                    },
                    Status = new OptionSetValue(-1)
                };

                FulfillSalesOrderResponse response = (FulfillSalesOrderResponse)_serviceClient.Execute(fulfillOrderRequest);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public bool CancelOrder(Guid orderid)
        {
            try
            {

                CancelSalesOrderRequest cancelOrderRequest = new CancelSalesOrderRequest
                {
                    OrderClose = new Entity("orderclose")
                    {
                        Attributes =
                        {
                            {"subject","Order FUlfilled" },
                            { "salesorderid", new EntityReference("salesorder", orderid) }
                        }
                    },
                    Status = new OptionSetValue(-1)
                };

                CancelSalesOrderResponse response = (CancelSalesOrderResponse)_serviceClient.Execute(cancelOrderRequest);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public Datatable<OrderModel> GetaAllOrders(string Draw, string? sortColumn, PagingInfo pagingInfo, string searchValue = null, string sortOrder = "asc")
        {
            EntityCollection allOrders = GetAll("salesorder", new ColumnSet("name", "customerid", "statecode", "createdon", "totalamount"), pagingInfo, sortColumn, ["name", "customerid", "totalamount"], searchValue, sortOrder);

            List<OrderModel> orders = new List<OrderModel>();
            foreach (var item in allOrders.Entities)
            {
                orders.Add(new OrderModel
                {
                    OrderId = item.Id,
                    OrderName = item.Contains("name")?item.GetAttributeValue<string>("name"):string.Empty,
                    CustomerName= item.Contains("customerid") ? item.GetAttributeValue<EntityReference>("customerid").Name : string.Empty,
                    TotalAmount = item.Contains("totalamount") ? item.GetAttributeValue<Money>("totalamount").Value : 0,
                    Status = item.Contains("statecode") ? ((OrderStateCode)(item.GetAttributeValue<OptionSetValue>("statecode").Value)).ToString() : String.Empty,
                    Createdon = item.Contains("createdon") ? item.GetAttributeValue<DateTime>("createdon") : DateTime.MinValue,
                });
            }
            Datatable<OrderModel> resultTable = new Datatable<OrderModel>
            {
                Draw = Draw,
                RecordsFiltered = allOrders.TotalRecordCount,
                RecordsTotal = allOrders.TotalRecordCount,
                Data = orders
            };
            return resultTable;
        }
        public string CreateInvoice(Guid orderid)
        {
            ConvertSalesOrderToInvoiceRequest invoiceRequest = new ConvertSalesOrderToInvoiceRequest()
            {
                SalesOrderId = orderid,
                ColumnSet = new ColumnSet("invoicenumber")
            };

            ConvertSalesOrderToInvoiceResponse invoiceResponse = (ConvertSalesOrderToInvoiceResponse)_serviceClient.Execute(invoiceRequest);
            return invoiceResponse.Entity.GetAttributeValue<string>("invoicenumber");
        }

        public void DeleteOrder(Guid orderid)
        {
            _serviceClient.Delete("salesorder", orderid);
        }
    }
}
