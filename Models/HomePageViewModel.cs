using REAgencyEnum;

namespace REAgency.Models
{
    public class HomePageViewModel
    {
       public string appName { get; set; }
        public string appPhone { get; set; }
        //public string? appComment { get; set; }
        public  ObjectType objectType { get; set; }

        //поля для поиска

        public int operationTypeId {  get; set; }

        public int localityId { get; set; }
    }
}
