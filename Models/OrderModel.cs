using Microsoft.AspNetCore.Mvc;

namespace RevisioneNew.Models
{
    public class OrderModel
    {
        public Guid OrderId { get; set; }
        public string ? OrderName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime Createdon { get; set; }
        public string ? Status { get; set; } // Pending, Confirmed, Fulfilled, etc.
        public string? CustomerName {  get; set; } 

        // Foreign Keys
        public Guid QuoteId { get; set; }
        public Quote ? Quote { get; set; }
    }
}
