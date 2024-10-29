using REAgency.BLL.DTO.Persons;

namespace REAgency.Models
{
    public class ClientsPageViewModel
    {
        public IEnumerable<ClientDTO> Clients { get; }
        public PageViewModel PageViewModel { get; }
        
        public ClientsPageViewModel(IEnumerable<ClientDTO> clients, PageViewModel viewModel)
        {
            Clients = clients;
            PageViewModel = viewModel;

        }
    }
}
