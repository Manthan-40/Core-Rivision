namespace RevisioneNew.Models
{
    public class InvoiceModel
    {
        public Guid InvoiceId { get; set; }
        public string ? InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Status { get; set; } // Unpaid, Paid, Overdue

        // Foreign Keys
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
