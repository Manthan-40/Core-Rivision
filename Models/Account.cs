namespace RevisioneNew.Models
{
    public class Account
    {
        public Guid AccountId { get; set; } // Unique identifier for the account
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string WebsiteUrl { get; set; }
        //public string Fax { get; set; }

        // Primary Address (Main Business Address)
        public string Address1_Line1 { get; set; }
        public string Address1_City { get; set; }
        public string Address1_State { get; set; }
        public string Address1_Country { get; set; }
        public string Address1_PostalCode { get; set; }

        // Shipping Address (Optional Secondary Address)
        public string Address2_Line1 { get; set; }
        public string Address2_City { get; set; }
        public string Address2_State { get; set; }
        public string Address2_Country { get; set; }
        public string Address2_PostalCode { get; set; }

        // Foreign Key Relationships
        public ICollection<Contact> Contacts { get; set; } // An account can have multiple contacts

        // Status/Activity Fields
        public string Status { get; set; } // Active, Inactive, etc.
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
