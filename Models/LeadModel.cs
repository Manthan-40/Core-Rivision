using Microsoft.AspNetCore.Mvc.Rendering;
using RevisioneNew.Lables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RevisioneNew.Models
{
    public class LeadModel
    {
        [Display(Name = "id", ResourceType = typeof(LeadLables))]
        public Guid Id { get; set; }

        [StringLength(60, MinimumLength = 6, ErrorMessage = "Topic name must be between 6 and 60 characters.")]
        [Required(ErrorMessage = "Please enter a topic name.")]
        [Display(Name = "topicName", ResourceType = typeof(LeadLables))]
        public string ? TopicName { get; set; }


        [Display(Name = "fullName", ResourceType = typeof(LeadLables))]
        public string ? FullName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        [Display(Name = "createdON", ResourceType = typeof(LeadLables))]
        public DateTime CreatedON { get; set; }

        [Display(Name = "stateCode", ResourceType = typeof(LeadLables))]
        public string? Status { get; set; }

        // Contact Details
        [StringLength(50, MinimumLength = 0, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Required(ErrorMessage = "Please enter the first name.")]
        [Display(Name = "firstName", ResourceType = typeof(LeadLables))]
        public string FirstName { get; set; }

        [StringLength(50, MinimumLength = 0, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Required(ErrorMessage = "Please enter the last name.")]
        [Display(Name = "lastName", ResourceType = typeof(LeadLables))]
        public string LastName { get; set; }

        [StringLength(100,MinimumLength =0, ErrorMessage = "Company name cannot exceed 100 characters.")]

        [Display(Name = "companyName", ResourceType = typeof(LeadLables))]
        public string ? CompanyName { get; set; }

        [StringLength(50, MinimumLength =0, ErrorMessage = "Job title cannot exceed 50 characters.")]
        [Display(Name = "jobTitle", ResourceType = typeof(LeadLables))]
        public string ? JobTitle { get; set; }

        [Required(ErrorMessage = "Please enter the email address.")]
        [Display(Name = "email", ResourceType = typeof(LeadLables))]
        [RegularExpression(@"^[A-Za-z0-9\._%+\-]+@[A-Za-z0-9\.\-]+\.[A-Za-z]{2,4}$", ErrorMessage = "Enter Valid Mail")]
        public string EmailAddress { get; set; }

        [Display(Name = "teleNumber", ResourceType = typeof(LeadLables))]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter a valid telephone number.")]
        public string ? Telephone { get; set; }

        [Display(Name = "parentAccount", ResourceType = typeof(LeadLables))]
        public string ? AccountName { get; set; }

        [Display(Name = "parentContact", ResourceType = typeof(LeadLables))]
        public string ? ContactName { get; set; }

        public Guid ? AccountID { get; set; }

        public Guid ? ContactID { get; set; }

        // Other Details  
        [Range(0, double.MaxValue, ErrorMessage = "Estimated budget must be a positive number.")]
        [Display(Name = "budget", ResourceType = typeof(LeadLables))]
        public decimal ? EstimatedBudget { get; set; }

        [StringLength(500, MinimumLength =0, ErrorMessage = "Description cannot exceed 500 characters.")]
        [Display(Name = "description", ResourceType = typeof(LeadLables))]
        public string ? Description { get; set; }

        // NotMapped attributes for select lists
        [NotMapped]
        public List<SelectListItem> ? AccountList { get; set; }

        [NotMapped]
        public List<SelectListItem> ? ContactList { get; set; }
    }
}
