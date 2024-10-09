using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RevisioneNew.Models
{
    public class SupportSignIn
    {
        [Required(ErrorMessage ="Subject is Required")]
        [DisplayName("Subject of Support")]
        [StringLength(60, ErrorMessage ="Subject's maximum length is 60.")]
        public string Subject {  get; set; }
        [Required(ErrorMessage = "Email is Required")]
        [DisplayName("Email")]
        [RegularExpression(@"^[A-Za-z0-9\._%+\-]+@[A-Za-z0-9\.\-]+\.[A-Za-z]{2,4}$", ErrorMessage ="Enter valid Email")]
        public string Email {  get; set; }
        [Required(ErrorMessage = "Description is Required")]
        [DisplayName("Description of Support")]
        [StringLength(250, ErrorMessage ="Subject's maximum length is 250.")]
        public string Description {  get; set; }
    }
}
