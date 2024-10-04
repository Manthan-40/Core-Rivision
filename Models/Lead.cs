using System.ComponentModel.DataAnnotations;

namespace RevisioneNew.Models
{
    public class Lead
    {
        public Guid Id { get; set; }

        [StringLength(60, MinimumLength = 6)]
        [Required(ErrorMessage = "Please Enter Topic")]
        public string ? TopicName { get; set; }

        [StringLength(60, MinimumLength = 6)]
        [Required(ErrorMessage = "Please Enter FullName")]
        public string ? FullName { get; set; }
        public DateTime CreatedON { get; set; }

        public String ? Status { get; set; }
    }
}
