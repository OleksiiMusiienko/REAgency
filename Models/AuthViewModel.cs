using System.ComponentModel.DataAnnotations;

namespace REAgency.Models
{
    public class AuthViewModel
    {
        public string ?LoginEmail { get; set; }
        public string ?LoginPassword { get; set; }

        [Required(ErrorMessage = "Поле має бути встановлене")]
        public string RegisterName { get; set; }

        [Required(ErrorMessage = "Поле має бути встановлене")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$", ErrorMessage = "Не корректно веден Email адрес")]
        public string RegisterEmail { get; set; }

        [Required(ErrorMessage = "Поле має бути встановлене")]
        [DataType(DataType.Password)]
        public string RegisterPassword { get; set; }

        [Required(ErrorMessage = "Поле має бути встановлене")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Поле має бути встановлене")]
        public bool confirmPersonalData { get; set; }



    }

}
