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

        public int estateTypeId { get; set; }

        public string ?status {  get; set; }
        public int employeeId { get; set; }

        public int minPrice { get; set; }
        public int maxPrice { get; set; }

        public double minArea { get; set; }
        public double maxArea { get; set; }


    }
}
