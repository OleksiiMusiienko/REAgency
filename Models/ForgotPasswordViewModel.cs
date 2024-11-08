using System.ComponentModel.DataAnnotations;

namespace REAgency.Models
{
    public class ForgotPasswordViewModel
    {
        [EmailAddress]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$", ErrorMessage = "Не корректно веден Email адрес")]
        public string Email { get; set; }
    }
}
