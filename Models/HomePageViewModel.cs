using REAgency.BLL.DTO.Object;
namespace REAgency.Models
{
    public class HomePageViewModel
    {
        public enum ObjectType
        {
            Flat, Garage, House, Office, Parking, Premis, Room, Stead, Storage
        }
        public string appName { get; set; }
        public string appPhone { get; set; }
        //public string? appComment { get; set; }
        public  ObjectType objectType { get; set; }

        //поля для поиска

        public int operationTypeId {  get; set; }

        public int localityId { get; set; }
    }
}
