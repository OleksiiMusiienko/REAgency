using System.ComponentModel.DataAnnotations;

namespace REAgency.Models
{
    public class OrderViewModel
    {
        public string name { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        public string phone { get; set; }
        public int id { get; set; }
        public string? typeObject { get; set; }
    }
}
