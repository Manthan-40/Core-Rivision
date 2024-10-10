using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RevisioneNew.Models
{
    public class OpportunityModel
    {
        public Guid Id { get; set; }

        [StringLength(60, MinimumLength = 6)]
        [Required(ErrorMessage = "Please enter a topic.")]
        public string Topic { get; set; }

        [StringLength(60, MinimumLength = 6)]
        [Required(ErrorMessage = "Please enter a description.")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedON { get; set; }

        [StringLength(50)]
        public string ? Status { get; set; }

        // New fields based on the logical names provided

        public Guid? ParentAccountID { get; set; }

        public Guid? ParentContactID { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Budget amount must be a positive value.")]
        public decimal? BudgetAmount { get; set; }

        [StringLength(500, ErrorMessage = "Customer need cannot exceed 500 characters.")]
        public string? CustomerNeed { get; set; }

        [StringLength(500, ErrorMessage = "Proposed solution cannot exceed 500 characters.")]
        public string? ProposedSolution { get; set; }

        public Guid? PriceLevelID { get; set; }

        public Guid CurrencyID { get; set; }

        [NotMapped]
        public List<SelectListItem>? AccountList { get; set; }

        [NotMapped]
        public List<SelectListItem>? ContactList { get; set; }

        [NotMapped]
        public List<SelectListItem>? PriceListitems { get; set; }

        [NotMapped]
        public List<SelectListItem>? CurrencyList { get; set; }
    }

}
