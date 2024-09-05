using Microsoft.Identity.Client;
using REAgency.BLL.DTO.Persons;
using System.ComponentModel.DataAnnotations;

namespace REAgency.Models
{
    public class ResetPasswordViewModel
    {
        public int id { get; set; }
        public string? personMail {  get; set; }
        public bool employee { get; set; }
        [DataType(DataType.Password)]
        public string ?Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Паролі не збігаються")]
        public string ?ConfirmPassword { get; set; }

    }
}
