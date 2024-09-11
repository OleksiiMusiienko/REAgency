using static REAgency.DAL.Entities.Object.EstateObject;
namespace REAgency.Models
{
    public class HomePageViewModel
    {
        public string appName { get; set; }
        public string appPhone { get; set; }
        //public string? appComment { get; set; }
        public ObjectType type { get; set; }
    }
}
