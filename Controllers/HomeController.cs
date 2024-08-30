using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using REAgency.BLL.DTO.Persons;
using REAgency.BLL.Interfaces;
using REAgency.BLL.Interfaces.Locations;
using REAgency.BLL.Interfaces.Object;
using REAgency.BLL.Interfaces.Persons;
using REAgency.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace REAgency.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IOperationService _operationService;
        private readonly IEstateTypeService _estateTypeService;
        private readonly ILocalityService _localityService;
        private readonly IFlatService _flatService;
        private readonly IClientService _clientService;
       

        public HomeController(IOperationService operationService, IEstateTypeService estateTypeService, ILocalityService localityService, IFlatService flatService, IClientService clientService)
        {
            //_logger = logger;
            _operationService = operationService;
            _estateTypeService = estateTypeService;
            _localityService = localityService;
            _flatService = flatService;
            _clientService = clientService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            ViewBag.OperatrionsList = new SelectList(await _operationService.GetAll(), "Id", "Name");

            ViewBag.EsateTypesList = new SelectList(await _estateTypeService.GetAll(), "Id", "Name");
            ViewBag.LocalitiesList = new SelectList(await _localityService.GetLocalities(), "Id", "Name");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Objects()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

		public async Task <IActionResult> FindByType(HomePageViewModel homePageViewModel)
		{
            string type = homePageViewModel.type;
            var typeOf = await _estateTypeService.GetByName(type);
            var i = await _flatService.GetFlatsByType(typeOf.Id);


            return View("Objects", i);
		}

        public async Task<IActionResult> SendApplication(HomePageViewModel homePageViewModel)
        {

            ClientDTO client = new ClientDTO();
            if (!Regex.IsMatch(homePageViewModel.appPhone, @"^\d+$"))
            {
                ModelState.AddModelError("appPhone", "�������� ���� �����");
                return View("Index", homePageViewModel);
            }
            else if (!Regex.IsMatch(homePageViewModel.appPhone, @"^0\d{9}$"))
            {
                ModelState.AddModelError("appPhone", "����� �������� �� ������ ���� 10 ���� � ���������� � 0.");
                return View("Index", homePageViewModel);
            }
            client.Name = homePageViewModel.appName;
            client.Phone1 = homePageViewModel.appPhone;
            client.status = false;
            client.userStatus = false;

            await _clientService.CreateClient(client);
            TempData["SuccessMessage"] = "true";

            return RedirectToAction("Index", "Home");
        }

    }
}
