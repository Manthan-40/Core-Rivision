using RevisioneNew.Lables;
using System.ComponentModel.DataAnnotations;

namespace RevisioneNew.Models
{
    public class LeadModel
    {
        [Display(Name = "id", ResourceType = typeof(LeadLables))]
        public Guid Id { get; set; }

        [StringLength(60, MinimumLength = 6)]
        [Required(ErrorMessage = "Please Enter Topic")]
        [Display(Name = "topicName", ResourceType = typeof(LeadLables))]
        public string ? TopicName { get; set; }

        [StringLength(60, MinimumLength = 6)]
        [Required(ErrorMessage = "Please Enter FullName")]
        [Display(Name = "fullName", ResourceType = typeof(LeadLables))]
        public string ? FullName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "createdON", ResourceType = typeof(LeadLables))]
        public DateTime CreatedON { get; set; }
        [Display(Name = "stateCode", ResourceType = typeof(LeadLables))]
        public String ? Status { get; set; }


        //Contect Dettails
        [Display(Name = "firstName", ResourceType = typeof(LeadLables))]
        public string FirstName { get; set; }
        [Display(Name = "lastName", ResourceType = typeof(LeadLables))]
        public string LastName { get; set; }
        [Display(Name = "companyName", ResourceType = typeof(LeadLables))]
        public string CompanyName { get; set; }
        [Display(Name = "jobTitle", ResourceType = typeof(LeadLables))]
        public string JobTitle { get; set; }
        [Display(Name = "email", ResourceType = typeof(LeadLables))]
        public string EmailAddress { get; set; }
        [Display(Name = "teleNumber", ResourceType = typeof(LeadLables))]
        public string Telephone { get; set; }


        //Lead associate account or Contect
        [Display(Name = "parentAccount", ResourceType = typeof(LeadLables))]
        public string AccountName { get; set; } 
        [Display(Name = "parentContact", ResourceType = typeof(LeadLables))]
        public string ContactName { get; set; } 

        //otherDetails  
        [Display(Name = "budget", ResourceType = typeof(LeadLables))]
        public decimal EstimatedBudget { get; set; }
        [Display(Name = "description", ResourceType = typeof(LeadLables))]
        public string Description { get; set; }
    }
}
