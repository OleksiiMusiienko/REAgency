using REAgency.BLL.DTO.Locations;

namespace REAgency.Models
{
    public class LocalitiesPageViewModel
    {
        public IEnumerable<LocalityDTO> Localities { get; }
        public PageViewModel PageViewModel { get; }

        public LocalitiesPageViewModel(IEnumerable<LocalityDTO> localities, PageViewModel viewModel)
        {
            Localities = localities;
            PageViewModel = viewModel;

        }
    }
}
