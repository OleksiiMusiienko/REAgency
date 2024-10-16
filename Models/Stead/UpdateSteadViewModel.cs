using System.ComponentModel.DataAnnotations;
using REAgencyEnum;

namespace REAgency.Models.Stead
{
    public class UpdateSteadViewModel
    {
        public int steadId { get; set; }

        [Required(ErrorMessage = "\"Тип операції\" обов'язково!")]
        [Display(Name = "Тип операції")]
        public int OperationId { get; set; }

        //адрес
        [Required(ErrorMessage = "\"Область\" обов'язково!")]
        [Display(Name = "Область")]
        public int RegionId { get; set; }

        [Required(ErrorMessage = "\"Район\" обов'язково!")]
        [Display(Name = "Район")]
        public int DistrictId { get; set; }

        [Required(ErrorMessage = "\"Населенний пункт\" обов'язково!")]
        [Display(Name = "Населенний пункт")]
        public int LocalityId { get; set; }

        public int countryId { get; set; }

        [Required(ErrorMessage = "\"Вулиця\" обов'язково!")]
        [Display(Name = "Вулиця")]
        public string Street { get; set; }

        [Display(Name = "Номер")]
        public int? numberStreet { get; set; }

        //цена
        [Required(ErrorMessage = "Поле \"Ціна\" обов'язкове!")]
        [Display(Name = "Ціна")]
        [Range(30, int.MaxValue)]
        public int Price { get; set; }

        [Display(Name = "Валюта")]
        public int currencyId { get; set; }

        [Required(ErrorMessage = "\"Загальна площа\" обов'язково!")]
        [Display(Name = "Загальна площа, сот")]
        [Range(15, double.MaxValue)]
        public double Area { get; set; }

        //параметры участка


        [Required(ErrorMessage = "\"Кадастровий номер обов'язково!")]
        [Display(Name = "Кадастровий номер")]
        [RegularExpression(@"^\d{10}:\d{2}:\d{3}:\d{4}$", ErrorMessage = "Не корректно веден кадастровый номер")]
        [Length(22, 22)]
        public string Cadastr { get; set; }

        [Required(ErrorMessage = "\"Цільове призначення обов'язково!")]
        [Display(Name = "Цільове призначення")]
        public int Use { get; set; }

        public string? Path { get; set; }

        //описание
        [Required(ErrorMessage = "\"Опис\" обов'язкове!")]
        [Display(Name = "Опис об'єкта")]
        [StringLength(1500), MinLength(10)]  //не забыть поставить ограничение
        public string Description { get; set; }

        //данные клиента
        [Required(ErrorMessage = "\"Ім'я\" обов'язково!")]
        [Display(Name = "Ім'я клієнта")]
        public string Name { get; set; }

        [Required(ErrorMessage = "\"Номер телефону\" обов'язково!")]
        [Display(Name = "Номер телефону клієнта")]
        [RegularExpression(@"^((\+)?\b(8|38)?(0[\d]{2}))([\d-]{5,8})([\d]{2})")]
        public string Phone1 { get; set; }

        [Display(Name = "Співробітник")]
        public int employeeId { get; set; }

        [Display(Name = "Статус")]
        public bool status { get; set; }

        public int estateObjectId { get; set; }
        public int countViews { get; set; }

        public int clientId { get; set; }
        public int locationId { get; set; }
        public DateTime Date { get; set; }
        public ObjectType estateType { get; set; }

        public int unitAreaId { get; set; }

        public List<string> photos { get; set; }
    }
}
