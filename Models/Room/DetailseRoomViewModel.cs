using REAgencyEnum;
using System.ComponentModel.DataAnnotations;

namespace REAgency.Models.Room
{
    public class DetailseRoomViewModel
    {
        public int roomId { get; set; }
        public int countViews { get; set; }

        public int clientId { get; set; }
        public string clientName { get; set; }
        public string clientPhone { get; set; }

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

        public double livingArea { get; set; }
        public int Floor { get; set; }
        public int Floors { get; set; }
    }
}
