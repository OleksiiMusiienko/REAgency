using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using REAgency.BLL.DTO;
using REAgency.BLL.DTO.Locations;
using REAgency.BLL.DTO.Object;
using REAgency.BLL.DTO.Persons;
using REAgency.BLL.Interfaces;
using REAgency.BLL.Interfaces.Locations;
using REAgency.BLL.Interfaces.Object;
using REAgency.BLL.Interfaces.Persons;
using REAgency.Models;
using REAgency.Models.Flat;
using REAgencyEnum;
using System.Data;

namespace REAgency.Controllers
{
    public class OfficeController : Controller
    {
		// IWebHostEnvironment предоставляет информацию об окружении, в котором запущено приложение
		IWebHostEnvironment _appEnvironment;
		private readonly IEstateObjectService _objectService;
        private readonly IOperationService _operationService;
        private readonly IRegionService _regionService;
        private readonly IDistrictService _districtService;
        private readonly ILocalityService _localityService;
        private readonly ICurrencyService _currencyService;
        private readonly IClientService _clientService;
        private readonly IEmployeeService _employeeService;
        private readonly ILocationService _locationService;
        private readonly IAreaService _areaService;
        private readonly IFlatService _flatService;
        public int pageSize = 9;

        public OfficeController(IEstateObjectService objectService, IOperationService operationService, IRegionService regionService, 
            IDistrictService districtService, ILocalityService localityService, ICurrencyService currencyService, IClientService clientService,
            IEmployeeService employeeService, IWebHostEnvironment appEnvironment, ILocationService locationService,
            IAreaService areaService, IFlatService flatService)
        {
            _objectService = objectService;
            _operationService = operationService;
            _regionService = regionService;
            _districtService = districtService;
            _localityService = localityService;
            _currencyService = currencyService; 
            _clientService = clientService;
            _employeeService = employeeService;
			_appEnvironment = appEnvironment;
            _locationService = locationService;
            _areaService = areaService;
            _flatService= flatService;
		}

        public async Task<IActionResult> Index(int page = 1)
        {
            //if(HttpContext.Session.GetString("User") != "employee")
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //IEnumerable<EstateObjectDTO> objects = await _objectService.GetEstateObjectByEmployeeId(HttpContext.Session.GetInt32("Id")!.Value);
            IEnumerable<EstateObjectDTO> objects = await _objectService.GetAllEstateObjects(); //↑

            IEnumerable<OperationDTO> operations = await _operationService.GetAll();
            IEnumerable<AreaDTO> areas = await _areaService.GetAll();
            IEnumerable<CurrencyDTO> currencies = await _currencyService.GetAll();
            IEnumerable<LocationDTO> locations = await _locationService.GetLocations();
            IEnumerable<LocalityDTO> localities = await _localityService.GetLocalities();

            var estateObjects = SelectEstateObject(objects, operations, areas, currencies, locations, localities);
            var count = estateObjects.Count();
            var items = estateObjects.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            pageViewModel.typeOfAction = "Search";
            ObjectPageViewModel objectPageViewModel = new ObjectPageViewModel(items, pageViewModel);
            return View(objectPageViewModel);

        }
        public async Task<IActionResult> GetAllObjects()
        {
            IEnumerable<EstateObjectDTO> objects = await _objectService.GetAllEstateObjects();
            IEnumerable<OperationDTO> operations = await _operationService.GetAll();

            var listObjects = objects.Select(estateObject => new ObjectsViewModel
            {
                Id = estateObject.Id,
                employeeId = estateObject.employeeId,
                operationName = operations.FirstOrDefault(op => op.Id == estateObject.operationId)?.Name,
                locationId = estateObject.locationId,
                Price = estateObject.Price,
                currencyId = estateObject.currencyId,
                Area = estateObject.Area,
                unitAreaId = estateObject.unitAreaId,
                pathPhoto = estateObject.pathPhoto
            }).Where(p => p.employeeId == HttpContext.Session.GetInt32("Id")).ToList();

            return View("Index", listObjects);

        }
        public async Task<IActionResult> Create(string selEstate)
        {
            if (selEstate != null)
            {
                ViewBag.Operations = new SelectList(await _operationService.GetAll(), "Id", "Name");
                ViewBag.Regions = new SelectList(await _regionService.GetRegions(), "Id", "Name", "CountryId");
                ViewBag.Districts = new SelectList(await _districtService.GetDistrict(), "Id", "Name", "RegionId");
                ViewBag.Localities = new SelectList(await _localityService.GetLocalities(), "Id", "Name", "DistrictId");
                ViewBag.Currencies = new SelectList(await _currencyService.GetAll(), "Id", "Name");
            }
            switch (selEstate)
            {
                case "Flat":
                    return View("AddFlatView");
                //case "House":
                //    return View(AddHouseView);
                //case "Room":
                //    return View(AddRoomView);
                //case "Stead":
                //    return View(AddSteadView);
                //case "Office":
                //    return View(AddOfficeView);
                //case "Garage":
                //    return View(AddGarageView);
                //case "Premis":
                //    return View(AddPremisView);
                //case "Parking":
                //    return View(AddParkingView);
                //case "Storage":
                //    return View(AddStorageView);

            }
            return View();
        }

        public async Task AddFoto(EstateObjectDTO estateObjectDTO, IFormFileCollection formFiles)
        {
            if (formFiles != null)
            {
                string id = estateObjectDTO.Id.ToString();
                //string id = "1";

				string path = @"wwwroot\images\";
                string subpath = id;
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                dirInfo.CreateSubdirectory(subpath);

                string[] array = new string[formFiles.Count];
                for (int i = 0; i < formFiles.Count; i++)
                {

                    array[i] = @"\images\" + id + "\\" + formFiles[i].FileName;
                    // Для получения полного пути к каталогу wwwroot
                    // применяется свойство WebRootPath объекта IWebHostEnvironment
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + array[i], FileMode.Create))
                    {
                        await formFiles[i].CopyToAsync(fileStream); // копируем файл в поток
                    }
                }
                string pathdirectory = @"\images\" + id;
                estateObjectDTO.pathPhoto = pathdirectory; //добавляем путь в обьект
                await _objectService.UpdateEstateObjectPath(estateObjectDTO); //обновляем обьект               
			}
		}
        // POST: estateObject/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(ObjectsViewModel ovm)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        if(ovm.objectType == ObjectType.Flat)
        //        {
        //             _objectService?.CreateEstateObject(CreateFlat(ovm));                    
        //        }

        //    }
        //    return RedirectToAction("Index");
        //}

        //metods create estate object

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFlat(AddFlatViewModel flatViewModel, IFormFileCollection formFiles)
        {
            //    1.Создать estateObject
            //    2. Добавить его в базу что бы получить Id и сформировать путь к папке с фотографиями
            //    3. Добавить фото
            //    4. Создать flatDTO
            //    5. Подать его в базу

            if (ModelState.IsValid && formFiles != null)
            {               
                EstateObjectDTO estateObjectDTO = new EstateObjectDTO();        
                ClientDTO clientDTO = await CreateClient(flatViewModel.Name, flatViewModel.Phone1);               

                estateObjectDTO.clientId = clientDTO.Id;
                estateObjectDTO.employeeId = (int)HttpContext.Session.GetInt32("Id");
                estateObjectDTO.operationId = flatViewModel.OperationId;                
                
                estateObjectDTO.LocalityId = flatViewModel.LocalityId;
                estateObjectDTO.Street = flatViewModel.Street;
                estateObjectDTO.numberStreet = flatViewModel.numberStreet;
                estateObjectDTO.Price = flatViewModel.Price;
                estateObjectDTO.currencyId = flatViewModel.currencyId;
                estateObjectDTO.Area = flatViewModel.Area;
                estateObjectDTO.unitAreaId = 1;             //нет смысла тянуть Id там 3 шт в базе
                estateObjectDTO.Description = flatViewModel.Description;
                estateObjectDTO.Status = false;
                estateObjectDTO.estateType = ObjectType.Flat;
                estateObjectDTO.Date = DateTime.Now;
                LocationDTO locationDTO = await CreateLocation(flatViewModel, estateObjectDTO.Date); 
                estateObjectDTO.locationId = locationDTO.Id;                
                
                await _objectService.CreateEstateObject(estateObjectDTO); //создаем обьект

                estateObjectDTO = await _objectService.GetByDateTime(estateObjectDTO.Date); //получаем его из базы уже с id

                await AddFoto(estateObjectDTO, formFiles); //добавляем фото и получаем путь
        

                FlatDTO flatDTO = new FlatDTO(); 
                if (estateObjectDTO != null)
                {                 
                    flatDTO.estateObjectId = estateObjectDTO.Id;
                    flatDTO.Floor = flatViewModel.Floor;
                    flatDTO.Floors = flatViewModel.Floors;
                    flatDTO.Rooms = flatViewModel.Rooms;
                    flatDTO.kitchenArea = flatViewModel.kitchenArea;
                    flatDTO.livingArea = flatViewModel.livingArea;
                }
                await _flatService.CreateFlat(flatDTO); //сохраняем Flat в базу 
                
            }
            return RedirectToAction("Index");
        }
        private async Task<ClientDTO> CreateClient(string clientName, string clientPhone)
        {
            ClientDTO clientDTO = await _clientService.GetByPhone(clientPhone);
            if (clientDTO.Phone1 == null)
            {
                clientDTO = new ClientDTO();
                clientDTO.Phone1 = clientPhone;
                clientDTO.Name = clientName;
                clientDTO.employeeId = HttpContext.Session.GetInt32("Id");
                OperationDTO operationDTO = await _operationService.GetByName("Продаж");
                clientDTO.operationId = operationDTO.Id;
                clientDTO.status = true;
                await _clientService.CreateClient(clientDTO);
                clientDTO = await _clientService.GetByPhone(clientPhone);
            }
            return clientDTO;
           
        }

        public async Task<IActionResult> ShowFlatForUpdate(int id, string typeObject)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                ViewBag.Operations = new SelectList(await _operationService.GetAll(), "Id", "Name");
                ViewBag.Regions = new SelectList(await _regionService.GetRegions(), "Id", "Name");
                ViewBag.Districts = new SelectList(await _districtService.GetDistrict(), "Id", "Name");
                ViewBag.Localities = new SelectList(await _localityService.GetLocalities(), "Id", "Name");
                ViewBag.Currencies = new SelectList(await _currencyService.GetAll(), "Id", "Name");
                ViewBag.Employees = new SelectList(await _employeeService.GetEmployees(), "Id", "Name");
                ViewBag.Areas = new SelectList(await _areaService.GetAll(), "Id", "Name");

                switch (typeObject)
                {
                    case "Flat":
                        FlatDTO flat = await _flatService.GetFlatByEstateObjectId((int)id);
                        return View("UpdateFlat", SelectFlat(flat));
                        //case "House":
                        //    return View(AddHouseView);
                        //case "Room":
                        //    return View(AddRoomView);
                        //case "Stead":
                        //    return View(AddSteadView);
                        //case "Office":
                        //    return View(AddOfficeView);
                        //case "Garage":
                        //    return View(AddGarageView);
                        //case "Premis":
                        //    return View(AddPremisView);
                        //case "Parking":
                        //    return View(AddParkingView);
                        //case "Storage":
                        //    return View(AddStorageView);

                }
               
            }
            catch
            {
                return NotFound();
            }
            return NotFound();
        }

        public async Task<IActionResult> UpdateFlat(UpdateFlatViewModel model)
        {
            try
            {
                LocationDTO locationDTO = new LocationDTO();
                locationDTO.Id = model.locationId;
                locationDTO.CountryId = 1;
                locationDTO.LocalityId = model.LocalityId;
                locationDTO.RegionId = model.RegionId;
                locationDTO.DistrictId = model.DistrictId;
                await _locationService.UpdateLocation(locationDTO);


                FlatDTO flatDTO = new FlatDTO();
                flatDTO.Id = model.flatId;
                flatDTO.Floor = model.Floor;
                flatDTO.Floors = model.Floors;
                flatDTO.Rooms = model.Rooms;
                flatDTO.livingArea = model.livingArea;
                flatDTO.kitchenArea = model.kitchenArea;
                flatDTO.estateObjectId = model.estateObjectId;
                await _flatService.UpdateFlat(flatDTO);


                EstateObjectDTO objectDTO = new EstateObjectDTO();
                objectDTO.Id = model.estateObjectId;
                objectDTO.Street = model.Street;
                objectDTO.numberStreet = model.numberStreet;
                objectDTO.Price = model.Price;
                objectDTO.currencyId = model.currencyId;
                objectDTO.countViews = model.countViews;
                objectDTO.employeeId = model.employeeId;
                objectDTO.clientId = model.clientId;
                objectDTO.locationId = model.locationId;
                objectDTO.operationId = model.OperationId;
                objectDTO.Area = model.Area;
                objectDTO.unitAreaId = model.unitAreaId;
                objectDTO.Description = model.Description;
                objectDTO.Date= model.Date;
                objectDTO.clientId = model.clientId;
                objectDTO.estateType = ObjectType.Flat;
                objectDTO.pathPhoto = model.Path;
                objectDTO.Status = model.status;


                await _objectService.UpdateEstateObject(objectDTO);
                return RedirectToAction("Index", "Home");

            }
            catch 
            {
                return View(model);
            }
            
          
        }

        public UpdateFlatViewModel SelectFlat(FlatDTO flat)
        {
            var viewModel = new UpdateFlatViewModel
            {
              
                flatId = flat.Id,
                employeeId = flat.employeeId,
                OperationId = flat.operationId,
                Street = flat.Street,
                numberStreet = (int)flat.numberStreet,
                Price = flat.Price,
                currencyId = flat.currencyId,
                Area = flat.Area,
                Description = flat.Description,
                Path = flat.pathPhoto,
                Floor = flat.Floor,
                Floors = flat.Floors,
                Rooms = flat.Rooms,
                kitchenArea = flat.kitchenArea,
                livingArea = flat.livingArea,
                status = flat.Status,
                estateObjectId = flat.estateObjectId,
                locationId = flat.locationId,
                LocalityId = (int)flat.LocalityId,
                RegionId = (int)flat.RegionId,
                countryId = flat.countryId,
                DistrictId = (int)flat.DistrictId,
                clientId = flat.clientId,
                Date = flat.Date,
                countViews = flat.countViews
               



            };
            return viewModel;
        }

        private async Task<LocationDTO> CreateLocation(AddFlatViewModel addFlatViewModel, DateTime dateTime)
        {
            LocationDTO locationDTO = new LocationDTO();
            locationDTO.CountryId = 1; //в базе одна страна
            locationDTO.RegionId = addFlatViewModel.RegionId;
            locationDTO.DistrictId = addFlatViewModel.DistrictId;
            locationDTO.LocalityId = addFlatViewModel.LocalityId;
            locationDTO.Date = dateTime;
            await _locationService.CreateLocation(locationDTO);
            locationDTO = await _locationService.GetByDateTime(dateTime);
            return locationDTO;

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
                clientId = estateObjectDTO.clientId,
                clientName = estateObjectDTO.clientName,
                clientPhone = estateObjectDTO.clientPhone,
                typeObject = estateObjectDTO.estateType.ToString(),

                //objectType = ObjectsViewModel.ObjectType.Flat


            }).ToList();
            return viewModel;
        }
    }
}
