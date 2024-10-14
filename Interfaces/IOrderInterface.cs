using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Models;

namespace RevisioneNew.Interfaces
{
    public interface IOrderInterface
    {
        public Datatable<OrderModel> GetaAllOrders(string Draw, string? sortColumn, PagingInfo pagingInfo, string searchValue = null, string sortOrder = "asc");

        public bool FulfillOrder(Guid orderid);
        public bool CancelOrder(Guid orderid);
        public string CreateInvoice(Guid orderid);
        public void DeleteOrder(Guid orderid);
    }
}
