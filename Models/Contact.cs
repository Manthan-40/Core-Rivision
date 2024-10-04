namespace RevisioneNew.Models
{
    public class Contact
    {
        public Guid ContactId { get; set; } // Unique identifier for the contact
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string JobTitle { get; set; }
        public string Address1_Line1 { get; set; }
        public string Address1_City { get; set; }
        public string Address1_State { get; set; }
        public string Address1_Country { get; set; }
        public string Address1_PostalCode { get; set; }

        // Foreign Key for Account (Many Contacts can belong to an Account)
        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        // Status/Activity Fields
        public string Status { get; set; } // Active, Inactive, etc.
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
