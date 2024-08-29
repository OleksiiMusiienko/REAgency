using System.ComponentModel.DataAnnotations;

namespace REAgency.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string ?Email { get; set; }
    }
}
