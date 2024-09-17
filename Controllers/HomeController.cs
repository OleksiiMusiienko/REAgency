using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using REAgency.BLL.DTO;
using REAgency.BLL.DTO.Locations;
using REAgency.BLL.DTO.Object;
using REAgency.BLL.DTO.Persons;
using REAgency.BLL.Interfaces;
using REAgency.BLL.Interfaces.Locations;
using REAgency.BLL.Interfaces.Object;
using REAgency.BLL.Interfaces.Persons;
using REAgency.Models;
using REAgencyEnum;
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
        private readonly IEstateObjectService _estateObjectService;


        


        public HomeController(IOperationService operationService, ILocalityService localityService, IFlatService flatService, IClientService clientService, 
            IHouseSevice houseSevice, IOfficeService officeService, IGarageService garageService, IAreaService areaService, ICurrencyService currencyService, IEstateObjectService estateObjectService)
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
            _estateObjectService = estateObjectService;
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
           
            IEnumerable<OperationDTO> operations = await _operationService.GetAll();
            IEnumerable<AreaDTO> areas = await _areaService.GetAll();
            IEnumerable<CurrencyDTO> currencies = await _currencyService.GetAll();

            if(homePageViewModel.objectType == ObjectType.Flat)
            {
                IEnumerable<FlatDTO> flats = await _flatService.GetAllFlats();

                return View("Objects", SelectFlats(flats, operations, areas, currencies));
            }
            else if (homePageViewModel.objectType == ObjectType.House)
            {
                IEnumerable<HouseDTO> houses = await _houseService.GetAllHouses();

                return View("Objects", SelectHouses(houses,operations,areas,currencies));
            }
            else if (homePageViewModel.objectType == ObjectType.Office)
            {
                IEnumerable<OfficeDTO> offices = await _officeService.GetOffices();

                return View("Objects", SelectOffices(offices,operations,areas,currencies));
            }
            else if (homePageViewModel.objectType == ObjectType.Garage)
            {
                IEnumerable<GarageDTO> garages = await _garageService.GetGarages();

                return View("Objects", SelectGarages(garages,operations,areas,currencies));
            }

             return View();

            
		}

        public async Task<IActionResult> Search(HomePageViewModel homePageViewModel)
        {
            int opTypeId = homePageViewModel.operationTypeId;
            int localityId = homePageViewModel.localityId;
            int estateTypeId = homePageViewModel.estateTypeId;
            int minPrice = homePageViewModel.minPrice;
            int maxPrice = homePageViewModel.maxPrice;
            double minArea = homePageViewModel.minArea;
            double maxArea = homePageViewModel.maxArea;

            IEnumerable<EstateObjectDTO> estateObjects = await _estateObjectService.GetAllEstateObjects();

            IEnumerable<OperationDTO> operations = await _operationService.GetAll();
            IEnumerable<AreaDTO> areas = await _areaService.GetAll();
            IEnumerable<CurrencyDTO> currencies = await _currencyService.GetAll();



            //if (opTypeId == 0 && localityId == 0 && estateTypeId == 0)
            //{
            //    var allEstateObjects = SelectEstateObject(estateObjects, operations, areas, currencies);
            //    return View("Objects", allEstateObjects);
            //}
            //else if (opTypeId != 0 && localityId == 0)
            //{
            //    IEnumerable<EstateObjectDTO> estateObjectsByOp = await _estateObjectService.GetEstateObjectByOperationId(opTypeId);

            //    var selectedByOperation = SelectEstateObject(estateObjectsByOp, operations, areas, currencies);
            //    return View("Objects", selectedByOperation);
            //}
            //else if (localityId != 0 && opTypeId == 0)
            //{
            //    IEnumerable<EstateObjectDTO> estateObjectsByLocality
            //        = await _estateObjectService.GetEstateObjectByLocalityId(localityId);
            //    var selectedByLocality = SelectEstateObject(estateObjectsByLocality, operations, areas, currencies);
            //    return View("Objects", selectedByLocality);
            //}
            //else if(estateTypeId != 0 && localityId == 0 && opTypeId == 0)
            //{
            //    IEnumerable<EstateObjectDTO> estateObjectsByEstateType= await _estateObjectService.GetEstateObjectByEstateTypeId(estateTypeId);
            //    var selectedEstateObjects = SelectEstateObject(estateObjectsByEstateType, operations, areas, currencies);
            //    return View("Objects", selectedEstateObjects);
            //}
            //else if(opTypeId != 0 && localityId != 0 && estateTypeId == 0)
            //{
            //    IEnumerable<EstateObjectDTO> estateObjectsByOperationAndLocality = await _estateObjectService.GetEstateObjectByOperationAndLocalityId(opTypeId, localityId);
            //    var selectedByLocalityAndOperation = SelectEstateObject(estateObjectsByOperationAndLocality, operations, areas, currencies);
            //    return View("Objects", selectedByLocalityAndOperation);
            //}
            //else if(estateTypeId != 0 && localityId != 0 && opTypeId != 0)
            //{
            //    IEnumerable<EstateObjectDTO> estateObjectsByAll = await _estateObjectService.GetEstateObjectsByAllParameters(opTypeId, localityId,estateTypeId);
            //    var selectedByAll = SelectEstateObject(estateObjectsByAll, operations, areas, currencies);
            //    return View("Objects", selectedByAll);
            //}

            var estateObjects1 = await _estateObjectService.GetFilteredEstateObjects(
        estateTypeId, opTypeId, localityId, minPrice, maxPrice, minArea, maxArea);


            return View("Objects", SelectEstateObject(estateObjects1, operations, areas, currencies));
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
                objectType = ObjectType.Flat


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
                objectType = ObjectType.House

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
                objectType = ObjectType.Garage
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
                objectType = ObjectType.Office

            }).ToList();

            return viewModel;
        }

        public ObjectsViewModel SelectFlat(FlatDTO flat, IEnumerable<OperationDTO> operations,
          IEnumerable<AreaDTO> areas, IEnumerable<CurrencyDTO> currencies)
        {
            var viewModel = new ObjectsViewModel
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
                objectType = ObjectType.Flat


            };
            return viewModel;
        }

        public ObjectsViewModel SelectGarage(GarageDTO garage, IEnumerable<OperationDTO> operations,
           IEnumerable<AreaDTO> areas, IEnumerable<CurrencyDTO> currencies)
        {
            var viewModel = new ObjectsViewModel
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
                objectType = ObjectType.Garage
            };

            return viewModel;
        }

        public IEnumerable<ObjectsViewModel> SelectEstateObject(IEnumerable<EstateObjectDTO> estateObjects, IEnumerable<OperationDTO> operations,
        IEnumerable<AreaDTO> areas, IEnumerable<CurrencyDTO> currencies)
        {
            var viewModel = estateObjects.Select(estateObjectDTO => new ObjectsViewModel
            {
                Id = estateObjectDTO.Id,
                countViews = estateObjectDTO.countViews,
                employeeId = estateObjectDTO.employeeId,
                operationId = estateObjectDTO.operationId,
                operationName = operations.FirstOrDefault(op => op.Id == estateObjectDTO.operationId)?.Name,
                locationId = estateObjectDTO.locationId,
                Street = estateObjectDTO.Street,
                numberStreet = estateObjectDTO.numberStreet,
                Price = estateObjectDTO.Price,
                currencyId = estateObjectDTO.currencyId,
                currencyName = currencies.FirstOrDefault(c => c.Id == estateObjectDTO.currencyId)?.Name,
                Area = estateObjectDTO.Area,
                unitAreaId = estateObjectDTO.unitAreaId,
                unitAreaName = areas.FirstOrDefault(a => a.Id == estateObjectDTO.unitAreaId).Name,
                Description = estateObjectDTO.Description,
                Status = estateObjectDTO.Status,
                Date = estateObjectDTO.Date,
                pathPhoto = estateObjectDTO.pathPhoto,
               
                //typeObject = "Квартира",
                //objectType = ObjectsViewModel.ObjectType.Flat


            }).ToList();
            return viewModel;
        }

    }
}
