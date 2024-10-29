using REAgencyEnum;
using System.ComponentModel.DataAnnotations;

namespace REAgency.Models.Flat
{
    public class DetailseFlatViewModel
    {
        public int flatId { get; set; }
        public int countViews { get; set; }  //количество просмотров обьекта считает средний слой

        //ез нижнего слоя придет обьект Client, нам нужно отобразить на вьюшке
        //(для сотрудника или админа) имя и телефон клиента

        public int clientId { get; set; }
        public string clientName { get; set; }
        public string clientPhone { get; set; }
        //приходит сотрудник, отображаем имя и телефон для всех
        public int employeeId { get; set; }
        public string employeeName { get; set; }
        public string employeePhone { get; set; }

        public int operationId { get; set; }
        public string operationName { get; set; }

        //по локациям отображаем страну, область, район, населенный пункт
        public int locationId { get; set; }
        public string locationName { get; set; }
        public int countryId { get; set; }
        public string countryName { get; set; }
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        public int? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int? LocalityId { get; set; }
        public string LocalityName { get; set; }
        public string? Street { get; set; }

        public int? numberStreet { get; set; }

        public int Price { get; set; }
        public int currencyId { get; set; }
        public string currencyName { get; set; }

        public double Area { get; set; }
        public int unitAreaId { get; set; }
        public string areaName { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public DateTime Date { get; set; }
        public List<string> photos { get; set; }
        public string Path { get; set; }
        public ObjectType estateType { get; set; }
        public int Floor { get; set; }
        public int Floors { get; set; }

        public int Rooms { get; set; }

        public double kitchenArea { get; set; }

        public double livingArea { get; set; }

        public int estateObjectId { get; set; }
    }
}
