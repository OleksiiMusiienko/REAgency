using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc;
using REAgency.BLL.DTO.Locations;
using REAgency.BLL.DTO.Persons;
using REAgency.BLL.Interfaces.Persons;
using REAgency.DAL.Entities.Locations;
using REAgency.Models;

namespace REAgency.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;

        public int pageSize = 18;
        public ClientController(IClientService clientService) 
        { 
            _clientService = clientService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            IEnumerable<ClientDTO> clients = await _clientService.GetClients();
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                int employeeId = (int)HttpContext.Session.GetInt32("Id");
                clients.Where(c => c.employeeId == employeeId).ToList();
            }
               
            var count = clients.Count();
            var items = clients.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            ClientsPageViewModel clientsPageViewModel = new ClientsPageViewModel(items, pageViewModel);
            return View(clientsPageViewModel);

        }
    }
}
