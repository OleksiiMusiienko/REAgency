using Microsoft.AspNetCore.Http;
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
using REAgency.DAL.Entities.Locations;
using REAgency.DAL.Entities.Object;
using REAgency.DAL.Entities.Person;
using REAgency.DAL.Interfaces;
using REAgency.Models;
using REAgencyEnum;
using System.Diagnostics;
using System.Text.RegularExpressions;
using LandUse = REAgencyEnum.LandUse;

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
        private readonly ISteadService _steadService;
        private readonly ILocationService _locationService;

        int pageSize = 9;




        public HomeController(IOperationService operationService, ILocalityService localityService, IFlatService flatService, IClientService clientService, 
            IHouseSevice houseService, IOfficeService officeService, IGarageService garageService, IAreaService areaService, ICurrencyService currencyService, 
            IEstateObjectService estateObjectService, ISteadService steadService, ILocationService locationService)
        {
            //_logger = logger;
            _operationService = operationService;
           
            _localityService = localityService;
            _flatService = flatService;
            _clientService = clientService;
            _houseService = houseService;
            _officeService = officeService;
            _garageService = garageService;
            _areaService = areaService;
            _currencyService = currencyService;
            _estateObjectService = estateObjectService;
            _steadService = steadService;
            _locationService = locationService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            ViewBag.OperatrionsList = new SelectList(await _operationService.GetAll(), "Id", "Name");

            //ViewBag.EstateTypesList = new SelectList(await _estateTypeService.GetAll(), "Id", "Name");
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


		public async Task <IActionResult> ShowObjectsByType(HomePageViewModel homePageViewModel, int page = 1)
		{
            

            IEnumerable<OperationDTO> operations = await _operationService.GetAll();
            IEnumerable<AreaDTO> areas = await _areaService.GetAll();
            IEnumerable<CurrencyDTO> currencies = await _currencyService.GetAll();
            var objectTypeSession = HttpContext.Session.GetString("objectType");

            if (homePageViewModel.objectType != null)
            {
                HttpContext.Session.SetString("objectType", homePageViewModel.objectType.ToString());
            }
           


            if (homePageViewModel.objectType == ObjectType.Flat || objectTypeSession == "Flat")
            {
                IEnumerable<FlatDTO> flats = await _flatService.GetAllFlats();

                var selectedFlats = SelectFlats(flats, operations, areas, currencies);
                var count = selectedFlats.Count();
                var items = selectedFlats.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
                
                //pageViewModel.typeOfAction = "ShowObjectsByType";
                ObjectPageViewModel objectPageViewModel = new ObjectPageViewModel(items, pageViewModel);
                objectPageViewModel.typeOfAction = "ShowObjectsByType";
                return View("Objects", objectPageViewModel);
            }
            else if (homePageViewModel.objectType == ObjectType.House || objectTypeSession == "House")
            {
                IEnumerable<HouseDTO> houses = await _houseService.GetAllHouses();

                var selectedHouses = SelectHouses(houses, operations, areas, currencies);
                var count = selectedHouses.Count();
                var items = selectedHouses.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

                //pageViewModel.typeOfAction = "ShowObjectsByType";
                ObjectPageViewModel objectPageViewModel = new ObjectPageViewModel(items, pageViewModel);
                objectPageViewModel.typeOfAction = "ShowObjectsByType";

                return View("Objects", objectPageViewModel);
            }
            else if (homePageViewModel.objectType == ObjectType.Office || objectTypeSession == "Office")
            {
                IEnumerable<OfficeDTO> offices = await _officeService.GetOffices();

                var selectedOffices = SelectOffices(offices, operations, areas, currencies);
                var count = selectedOffices.Count();
                var items = selectedOffices.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

                //pageViewModel.typeOfAction = "ShowObjectsByType";
                ObjectPageViewModel objectPageViewModel = new ObjectPageViewModel(items, pageViewModel);
                objectPageViewModel.typeOfAction = "ShowObjectsByType";
                return View("Objects", objectPageViewModel);
            }
            else if (homePageViewModel.objectType == ObjectType.Garage || objectTypeSession == "Garage")
            {
                IEnumerable<GarageDTO> garages = await _garageService.GetGarages();

                var selectedGarages = SelectGarages(garages, operations, areas, currencies);
                var count = selectedGarages.Count();
                var items = selectedGarages.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

                //pageViewModel.typeOfAction = "ShowObjectsByType";
                ObjectPageViewModel objectPageViewModel = new ObjectPageViewModel(items, pageViewModel);
                objectPageViewModel.typeOfAction = "ShowObjectsByType";
                return View("Objects", objectPageViewModel);
            }
            else if(homePageViewModel.objectType == ObjectType.Stead || objectTypeSession == "Stead")
            {
                IEnumerable<SteadDTO> steads = await _steadService.GetSteads();
                var selectedSteads = SelectSteads(steads, operations, areas, currencies);
                var count = selectedSteads.Count();
                var items = selectedSteads.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

                //pageViewModel.typeOfAction = "ShowObjectsByType";
                ObjectPageViewModel objectPageViewModel = new ObjectPageViewModel(items, pageViewModel);
                objectPageViewModel.typeOfAction = "ShowObjectsByType";
                return View("Objects", objectPageViewModel);
            }


             return View();

            
		}

        public async Task<IActionResult> Search(HomePageViewModel homePageViewModel, int page = 1)
        {
            int opTypeId = homePageViewModel.operationTypeId;
            int localityId = homePageViewModel.localityId;
            int estateTypeId = homePageViewModel.estateTypeId;
            int minPrice = homePageViewModel.minPrice;
            int maxPrice = homePageViewModel.maxPrice;
            double minArea = homePageViewModel.minArea;
            double maxArea = homePageViewModel.maxArea;

            if(opTypeId != 0 || localityId != 0 || estateTypeId != 0 ||
                minPrice != 0 || maxPrice != 0 || minArea != 0 || maxArea != 0)
            {
                HttpContext.Session.SetInt32("opTypeId", opTypeId);
                HttpContext.Session.SetInt32("localityId", localityId);
                HttpContext.Session.SetInt32("estateTypeId", estateTypeId);
                HttpContext.Session.SetInt32("minPrice", minPrice);
                HttpContext.Session.SetInt32("maxPrice", maxPrice);
                HttpContext.Session.SetString("minArea", minArea.ToString("R"));
                HttpContext.Session.SetString("maxArea", maxArea.ToString("R"));
            }
            

            var opTypeIdSession = HttpContext.Session.GetInt32("opTypeId");
            var localityIdSession = HttpContext.Session.GetInt32("localityId");
            var estateTypeIdSession = HttpContext.Session.GetInt32("estateTypeId");
            var minPriceSession = HttpContext.Session.GetInt32("minPrice");
            var maxPriceSession = HttpContext.Session.GetInt32("maxPrice");

            
            var minAreaSessionStr = HttpContext.Session.GetString("minArea");
            var maxAreaSessionStr = HttpContext.Session.GetString("maxArea");

            double? minAreaSession = double.TryParse(minAreaSessionStr, out _) ? minArea : (double?)null;
            double? maxAreaSession = double.TryParse(maxAreaSessionStr, out _) ? maxArea : (double?)null;



            IEnumerable<OperationDTO> operations = await _operationService.GetAll();
            IEnumerable<AreaDTO> areas = await _areaService.GetAll();
            IEnumerable<CurrencyDTO> currencies = await _currencyService.GetAll();
            IEnumerable<LocationDTO> locations = await _locationService.GetLocations();
            IEnumerable<LocalityDTO> localities = await _localityService.GetLocalities();

            var filtredEstateObjects = await _estateObjectService.GetFilteredEstateObjects(estateTypeId, opTypeId, localityId, minPrice, maxPrice, minArea, maxArea);


            ViewBag.OperatrionsList = new SelectList(await _operationService.GetAll(), "Id", "Name");
            ViewBag.LocalitiesList = new SelectList(await _localityService.GetLocalities(), "Id", "Name");

            if (opTypeId != 0 || localityId != 0 || estateTypeId != 0 ||
                minPrice != 0 || maxPrice != 0 || minArea != 0 || maxArea != 0 )
            {

                return View("Objects", ShowObjectsWithPagination(filtredEstateObjects, operations, areas, currencies, localities, locations, page));
            }
            else
            {
                if(opTypeIdSession == null && localityIdSession == null &&
                    estateTypeIdSession == null && minPriceSession == null &&
                    maxPriceSession == null && minAreaSession == null && maxAreaSession == null)
                {
                    return View("Objects", ShowObjectsWithPagination(filtredEstateObjects, operations, areas, currencies, localities, locations, page));

                }
                else
                {
                    var filtredEstateObjectsFromSession = await _estateObjectService.GetFilteredEstateObjects(
                    estateTypeIdSession, opTypeIdSession, localityIdSession, minPriceSession, maxPriceSession, minAreaSession, maxAreaSession);

                    return View("Objects", ShowObjectsWithPagination(filtredEstateObjectsFromSession, operations, areas, currencies, localities, locations, page));
                }
                
              
            }
           
        }

        public ObjectPageViewModel ShowObjectsWithPagination(IEnumerable<EstateObjectDTO> filtredEstateObjects, IEnumerable<OperationDTO> operations,
            IEnumerable<AreaDTO> areas, IEnumerable<CurrencyDTO> currencies, IEnumerable<LocalityDTO> localities, IEnumerable<LocationDTO> locations, int page = 1)
        {
            var estateObjects = SelectEstateObject(filtredEstateObjects, operations, areas, currencies, locations, localities);
            var count = estateObjects.Count();
            var items = estateObjects.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            //pageViewModel.typeOfAction = "Search";
            ObjectPageViewModel objectPageViewModel = new ObjectPageViewModel(items, pageViewModel);
            objectPageViewModel.typeOfAction = "Search";
            return objectPageViewModel;
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

        public List<ObjectsViewModel> SelectSteads(IEnumerable<SteadDTO> steads, IEnumerable<OperationDTO> operations,
           IEnumerable<AreaDTO> areas, IEnumerable<CurrencyDTO> currencies)
        {
            var viewModel = steads.Select(stead => new ObjectsViewModel
            {
                Id = stead.Id,
                countViews = stead.countViews,
                employeeId = stead.employeeId,
                operationId = stead.operationId,
                operationName = operations.FirstOrDefault(op => op.Id == stead.operationId)?.Name,
                locationId = stead.locationId,
                Street = stead.Street,
                numberStreet = stead.numberStreet,
                Price = stead.Price,
                currencyId = stead.currencyId,
                currencyName = currencies.FirstOrDefault(c => c.Id == stead.currencyId)?.Name!,
                Area = stead.Area,
                unitAreaId = stead.unitAreaId,
                unitAreaName = areas.FirstOrDefault(a => a.Id == stead.unitAreaId).Name,
                Description = stead.Description,
                Status = stead.Status,
                Date = stead.Date,
                pathPhoto = stead.pathPhoto,
                Cadastr = stead.Cadastr,
                Use = stead.Use.ToString(),
                typeObject = "Ділянка",
                objectType = ObjectType.Stead

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
        IEnumerable<AreaDTO> areas, IEnumerable<CurrencyDTO> currencies, IEnumerable<LocationDTO> locations, IEnumerable<LocalityDTO> localities)
        {
            var viewModel = estateObjects.Select(estateObjectDTO => new ObjectsViewModel
            {
                Id = estateObjectDTO.Id,
                countViews = estateObjectDTO.countViews,
                employeeId = estateObjectDTO.employeeId,
                operationId = estateObjectDTO.operationId,
                operationName = operations.FirstOrDefault(op => op.Id == estateObjectDTO.operationId)?.Name,
                locationId = estateObjectDTO.locationId,
                localityId = (int)locations.FirstOrDefault(l => l.Id == estateObjectDTO.locationId).LocalityId,

                localityName = localities.FirstOrDefault(l => l.Id == locations.FirstOrDefault(l => l.Id == estateObjectDTO.locationId).LocalityId)?.Name,
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

                typeObject = estateObjectDTO.estateType.ToString(),

                //objectType = ObjectsViewModel.ObjectType.Flat


            }).ToList();
            return viewModel;
        }

    }
}
