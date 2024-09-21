using REAgencyEnum;
using System.ComponentModel.DataAnnotations;

namespace REAgency.Models
{
    public class ObjectsViewModel
    {
        //поля из EstateObjects
       
        #region FildsFromEstateObjects
        public int Id { get; set; } //это поле есть во всех таблицах
        public int countViews { get; set; } 

        public int clientId { get; set; }
    
        public int? employeeId { get; set; }
    

        public int operationId { get; set; }
     
        public int locationId { get; set; }
       
        public string? Street { get; set; }

        public int? numberStreet { get; set; }

        [Required(ErrorMessage = "Поле \"Ціна\" обов'язкове!")]
        [Display(Name = "Ціна")]
        [Range(30, int.MaxValue)]
        public int Price { get; set; }
        public int currencyId { get; set; }
        public string currencyName { get; set; }
       

        [Required(ErrorMessage = "Поле \"Загальга площа\" обов'язкове!")]
        [Display(Name = "Загальна площа")]
        [Range(0.1, double.MaxValue)]
        public double Area { get; set; }
        public int unitAreaId { get; set; }

        public string unitAreaName { get; set; }
      

        [Required(ErrorMessage = "Поле \"Опис\" обов'язкове!")]
        [Display(Name = "Опис")]
        [StringLength(1500), MinLength(150)]
        public string Description { get; set; }
        public bool Status { get; set; }
        public DateTime Date { get; set; }
        public string? pathPhoto { get; set; }

        #endregion

        //Поля из Flat

        #region FildsFromFlat

        [Required(ErrorMessage = "Поле \"Поверх\" обов'язкове!")]
        [Display(Name = "Поверх")]
        [Range(0, int.MaxValue)]
        public int Floor { get; set; } //это поле также есть в Room

        [Required(ErrorMessage = "Поле \"Поверховість\" обов'язкове!")]
        [Display(Name = "Поверховість")]
        [Range(0, int.MaxValue)]
        public int Floors { get; set; } //это поле также есть в House, Garage, Room

        [Required(ErrorMessage = "Поле \"Кількість кімнат\" обов'язкове!")]
        [Display(Name = "Кількість кімнат")]
        [Range(1, 11)]
        public int Rooms { get; set; } //это поле также есть в House

        [Required(ErrorMessage = "Поле \"Площа кухні\" обов'язкове!")]
        [Display(Name = "Площа кухні")]
        [Range(3, double.MaxValue)]
        public double kitchenArea { get; set; } //это поле также есть в House

        [Required(ErrorMessage = "Поле \"Житлова площа\" обов'язкове!")]
        [Display(Name = "Житлова площа")]
        [Range(8, double.MaxValue)]
        public double livingArea { get; set; } //это поле также есть в House, Room

        #endregion

        //Поля из House

        #region FildsFromHouse

        [Required(ErrorMessage = "Поле \"Площа ділянки\" обов'язкове!")]
        [Display(Name = "Площа ділянки")]
        [Range(3, double.MaxValue, ErrorMessage = "Площа ділянки не може бути менше 3 сот.")]
        public double steadArea { get; set; }

        #endregion

        //Поля из Stead 
        #region FildsFromStead

        [Required(ErrorMessage = "Поле \"Кадастр\" обов'язкове!")]
        public string Cadastr { get; set; }
        [Required(ErrorMessage = "Поле \"Землекористування\" обов'язкове!")]
        public string Use { get; set; }

        #endregion

        public string? operationName { get; set; }

        public string? typeObject{ get; set; }
        public ObjectType objectType { get; set; }

        public int localityId { get; set; }

        public string? localityName {  get; set; }

        
        
    }
}
