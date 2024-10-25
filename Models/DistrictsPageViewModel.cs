using REAgency.BLL.DTO.Locations;
using REAgency.BLL.DTO.Persons;

namespace REAgency.Models
{
    public class DistrictsPageViewModel
    {
        public IEnumerable<DistrictDTO> Districts { get; }
        public PageViewModel PageViewModel { get; }
      
        public DistrictsPageViewModel(IEnumerable<DistrictDTO> districts, PageViewModel viewModel)
        {
            Districts = districts;
            PageViewModel = viewModel;

        }
    }
}
