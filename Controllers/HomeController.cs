using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using REAgency.BLL.DTO;
using REAgency.BLL.DTO.Object;
using REAgency.BLL.DTO.Persons;
using REAgency.BLL.Interfaces;
using REAgency.BLL.Interfaces.Locations;
using REAgency.BLL.Interfaces.Object;
using REAgency.BLL.Interfaces.Persons;
using REAgency.DAL.Entities.Object;
using REAgency.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace REAgency.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IOperationService _operationService;
        
        private readonly ILocalityService _localityService;
        private readonly IFlatService _flatService;
        private readonly IClientService _clientService;
        private readonly IHouseSevice _houseService;
        private readonly IOfficeService _officeService;




        public HomeController(IOperationService operationService, ILocalityService localityService, IFlatService flatService, IClientService clientService, 
            IHouseSevice houseSevice, IOfficeService officeService)
        {
            //_logger = logger;
            _operationService = operationService;
           
            _localityService = localityService;
            _flatService = flatService;
            _clientService = clientService;
            _houseService = houseSevice;
            _officeService = officeService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            ViewBag.OperatrionsList = new SelectList(await _operationService.GetAll(), "Id", "Name");

            //ViewBag.EsateTypesList = new SelectList(await _estateTypeService.GetAll(), "Id", "Name");
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
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            
            return RedirectToAction("Index");
        }
        public IActionResult Office()
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

            if (type == "Квартира")
            {
                IEnumerable<FlatDTO> flats = await _flatService.GetAllFlats();

                IEnumerable<OperationDTO> operations = await _operationService.GetAll();

                var viewModel = flats.Select(flat => new ObjectsViewModel
                {
                    Id = flat.Id,
                    countViews = flat.countViews,
                    employeeId = flat.employeeId,
                    operationId = flat.operationId,
                    operationName = operations.FirstOrDefault(op => op.Id == flat.operationId)?.Name,
                    locationId = flat.locationId,
                    Street = flat.Street,
                    numberStreet = flat.numberStreet,
                    Price = flat.Price,
                    currencyId = flat.currencyId,
                    Area = flat.Area,
                    unitAreaId = flat.unitAreaId,
                    Description = flat.Description,
                    Status = flat.Status,
                    Date = flat.Date,
                    pathPhoto = flat.pathPhoto,
                    Floor = flat.Floor,
                    Floors = flat.Floors,
                    Rooms = flat.Rooms,
                    kitchenArea = flat.kitchenArea,
                    livingArea = flat.livingArea,
                    type = "Квартира"

                }).ToList();

                return View("Objects", viewModel);
            }
            else if (type == "Будинки")
            {
                IEnumerable<HouseDTO> houses = await _houseService.GetAllHouses();

                IEnumerable<OperationDTO> operations = await _operationService.GetAll();

                var viewModel = houses.Select(house => new ObjectsViewModel
                {
                    Id = house.Id,
                    countViews = house.countViews,
                    employeeId = house.employeeId,
                    operationId = house.operationId,
                    operationName = operations.FirstOrDefault(op => op.Id == house.operationId)?.Name,
                    locationId = house.locationId,
                    Street = house.Street,
                    numberStreet = house.numberStreet,
                    Price = house.Price,
                    currencyId = house.currencyId,
                    Area = house.Area,
                    unitAreaId = house.unitAreaId,
                    Description = house.Description,
                    Status = house.Status,
                    Date = house.Date,
                    pathPhoto = house.pathPhoto,
                   
                    Floors = house.Floors,
                    Rooms = house.Rooms,
                    kitchenArea = house.kitchenArea,
                    livingArea = house.livingArea,
                    steadArea = house.steadArea,
                    type = "Будинок"

                }).ToList();

                return View("Objects", viewModel);
            }
            else if (type == "Офіси")
            {
                IEnumerable<OfficeDTO> offices = await _officeService.GetOffices();

                IEnumerable<OperationDTO> operations = await _operationService.GetAll();

                var viewModel = offices.Select(office => new ObjectsViewModel
                {
                    Id = office.Id,
                    countViews = office.countViews,
                    employeeId = office.employeeId,
                    operationId = office.operationId,
                    operationName = operations.FirstOrDefault(op => op.Id == office.operationId)?.Name,
                    locationId = office.locationId,
                    Street = office.Street,
                    numberStreet = office.numberStreet,
                    Price = office.Price,
                    currencyId = office.currencyId,
                    Area = office.Area,
                    unitAreaId = office.unitAreaId,
                    Description = office.Description,
                    Status = office.Status,
                    Date = office.Date,
                    pathPhoto = office.pathPhoto,

                    type = "Офіс"

                }).ToList();

                return View("Objects", viewModel);
            }
             return View();

            
		}

        public async Task<IActionResult> SendApplication(HomePageViewModel homePageViewModel)
        {

            ClientDTO client = new ClientDTO();
            if (!Regex.IsMatch(homePageViewModel.appPhone, @"^\d+$"))
            {
                ModelState.AddModelError("appPhone", "Допустимі лише цифри");
                return View("Index", homePageViewModel);
            }
            else if (!Regex.IsMatch(homePageViewModel.appPhone, @"^0\d{9}$"))
            {
                ModelState.AddModelError("appPhone", "Номер телефону має містити рівно 10 цифр і починатися з 0.");
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
