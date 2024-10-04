using Microsoft.AspNetCore.Mvc;

namespace RevisioneNew.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public string ? OrderName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string ? Status { get; set; } // Pending, Confirmed, Fulfilled, etc.

        // Foreign Keys
        public Guid QuoteId { get; set; }
        public Quote ? Quote { get; set; }
    }
}
