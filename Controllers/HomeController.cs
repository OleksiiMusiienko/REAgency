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
        private readonly IGarageService _garageService;
        private readonly IAreaService _areaService;
        private readonly ICurrencyService _currencyService;


        


        public HomeController(IOperationService operationService, ILocalityService localityService, IFlatService flatService, IClientService clientService, 
            IHouseSevice houseSevice, IOfficeService officeService, IGarageService garageService, IAreaService areaService, ICurrencyService currencyService)
        {
            //_logger = logger;
            _operationService = operationService;
           
            _localityService = localityService;
            _flatService = flatService;
            _clientService = clientService;
            _houseService = houseSevice;
            _officeService = officeService;
            _garageService = garageService;
            _areaService = areaService;
            _currencyService = currencyService;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


		public async Task <IActionResult> ShowObjectsByType(HomePageViewModel homePageViewModel)
		{
            //string type = homePageViewModel.type;
            IEnumerable<OperationDTO> operations = await _operationService.GetAll();
            IEnumerable<AreaDTO> areas = await _areaService.GetAll();
            IEnumerable<CurrencyDTO> currencies = await _currencyService.GetAll();

            if (homePageViewModel.objectType == HomePageViewModel.ObjectType.Flat)
            {
                IEnumerable<FlatDTO> flats = await _flatService.GetAllFlats();

                return View("Objects", SelectFlats(flats, operations, areas, currencies));
            }
            else if (homePageViewModel.objectType == HomePageViewModel.ObjectType.House)
            {
                IEnumerable<HouseDTO> houses = await _houseService.GetAllHouses();

                return View("Objects", SelectHouses(houses,operations,areas,currencies));
            }
            else if (homePageViewModel.objectType == HomePageViewModel.ObjectType.Office)
            {
                IEnumerable<OfficeDTO> offices = await _officeService.GetOffices();

                return View("Objects", SelectOffices(offices,operations,areas,currencies));
            }
            else if (homePageViewModel.objectType == HomePageViewModel.ObjectType.Garage)
            {
                IEnumerable<GarageDTO> garages = await _garageService.GetGarages();

                return View("Objects", SelectGarages(garages,operations,areas,currencies));
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

        public List<ObjectsViewModel> SelectFlats(IEnumerable<FlatDTO> flats, IEnumerable<OperationDTO> operations, 
            IEnumerable<AreaDTO> areas, IEnumerable<CurrencyDTO> currencies)
        {
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
                currencyName = currencies.FirstOrDefault(c => c.Id == flat.currencyId)?.Name,
                Area = flat.Area,
                unitAreaId = flat.unitAreaId,
                unitAreaName = areas.FirstOrDefault(a => a.Id == flat.unitAreaId).Name,
                Description = flat.Description,
                Status = flat.Status,
                Date = flat.Date,
                pathPhoto = flat.pathPhoto,
                Floor = flat.Floor,
                Floors = flat.Floors,
                Rooms = flat.Rooms,
                kitchenArea = flat.kitchenArea,
                livingArea = flat.livingArea,
                typeObject = "Квартира",
                objectType = ObjectsViewModel.ObjectType.Flat


            }).ToList();
            return viewModel;
        }

        public List<ObjectsViewModel> SelectHouses(IEnumerable<HouseDTO> houses, IEnumerable<OperationDTO> operations,
            IEnumerable<AreaDTO> areas, IEnumerable<CurrencyDTO> currencies)
        {
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
                currencyName = currencies.FirstOrDefault(c => c.Id == house.currencyId)?.Name,
                Area = house.Area,
                unitAreaId = house.unitAreaId,
                unitAreaName = areas.FirstOrDefault(a => a.Id == house.unitAreaId).Name,
                Description = house.Description,
                Status = house.Status,
                Date = house.Date,
                pathPhoto = house.pathPhoto,

                Floors = house.Floors,
                Rooms = house.Rooms,
                kitchenArea = house.kitchenArea,
                livingArea = house.livingArea,
                steadArea = house.steadArea,
                typeObject = "Будинок",
                objectType = ObjectsViewModel.ObjectType.House

            }).ToList();
            return viewModel;
        }

        public List<ObjectsViewModel> SelectGarages(IEnumerable<GarageDTO> garages, IEnumerable<OperationDTO> operations,
           IEnumerable<AreaDTO> areas, IEnumerable<CurrencyDTO> currencies)
        {
            var viewModel = garages.Select(garage => new ObjectsViewModel
            {
                Id = garage.Id,
                countViews = garage.countViews,
                employeeId = garage.employeeId,
                operationId = garage.operationId,
                operationName = operations.FirstOrDefault(op => op.Id == garage.operationId)?.Name,
                locationId = garage.locationId,
                Street = garage.Street,
                numberStreet = garage.numberStreet,
                Price = garage.Price,
                currencyId = garage.currencyId,
                currencyName = currencies.FirstOrDefault(c => c.Id == garage.currencyId)?.Name!,
                Area = garage.Area,
                unitAreaId = garage.unitAreaId,
                unitAreaName = areas.FirstOrDefault(a => a.Id == garage.unitAreaId).Name,
                Description = garage.Description,
                Status = garage.Status,
                Date = garage.Date,
                pathPhoto = garage.pathPhoto,
                Floors = garage.Floors,
                typeObject = "Гараж",
                objectType = ObjectsViewModel.ObjectType.Garage
            }).ToList();

            return viewModel;
        }

        public List<ObjectsViewModel> SelectOffices(IEnumerable<OfficeDTO> offices, IEnumerable<OperationDTO> operations,
            IEnumerable<AreaDTO> areas, IEnumerable<CurrencyDTO> currencies)
        {
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
                currencyName = currencies.FirstOrDefault(c => c.Id == office.currencyId)?.Name,
                Area = office.Area,
                unitAreaId = office.unitAreaId,
                unitAreaName = areas.FirstOrDefault(a => a.Id == office.unitAreaId).Name,
                Description = office.Description,
                Status = office.Status,
                Date = office.Date,
                pathPhoto = office.pathPhoto,
                typeObject = "Офіс",
                objectType = ObjectsViewModel.ObjectType.Office

            }).ToList();

            return viewModel;
        }
    }
}
