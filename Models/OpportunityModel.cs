using System.ComponentModel.DataAnnotations;

namespace RevisioneNew.Models
{
    public class OpportunityModel
    {
        public Guid Id { get; set; }

        [StringLength(60, MinimumLength = 6)]
        [Required(ErrorMessage = "Please Enter Topic")]
        public string Topic { get; set; }

        [StringLength(60, MinimumLength = 6)]
        [Required(ErrorMessage = "Please Enter FullName")]
        public string Description { get; set; }
        public DateTime CreatedON { get; set; }

        public String Status { get; set; }
    }
}
