namespace RevisioneNew.Models
{
    public class Quote
    {
        public Guid QuoteId { get; set; }
        public string QuoteNumber { get; set; }
        public string QuoteName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Status { get; set; } // Draft, Active, Closed, etc.
        public string Statuscode { get; set; } // Draft, Active, Closed, etc.

        // Foreign Keys
        public Guid OpportunityId { get; set; }
        public OpportunityModel Opportunity { get; set; }
    }
}
