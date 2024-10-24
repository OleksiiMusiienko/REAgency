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
using REAgency.DAL.Entities.Object;
using REAgency.Models;
using REAgency.Models.Flat;
using REAgency.Models.Garage;
using REAgency.Models.House;
using REAgency.Models.Office;
using REAgency.Models.Parking;
using REAgency.Models.Premis;
using REAgency.Models.Room;
using REAgency.Models.Stead;
using REAgency.Models.Storage;
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
        private readonly IHouseSevice _houseService;
        private readonly IWebHostEnvironment _env;
        private readonly IRoomService _roomService;
        private readonly ISteadService _steadService;
        private readonly IOfficeService _officeService;
        private readonly IPremisService _premisService;
        private readonly IParkingService _parkingService;
        private readonly IStorageService _storageService;
        private readonly IGarageService _garageService;
        public int pageSize = 9;
       

        public OfficeController(IEstateObjectService objectService, IOperationService operationService, IRegionService regionService, 
            IDistrictService districtService, ILocalityService localityService, ICurrencyService currencyService, IClientService clientService,
            IEmployeeService employeeService, IWebHostEnvironment appEnvironment, ILocationService locationService,
            IAreaService areaService, IFlatService flatService, IHouseSevice houseService, IWebHostEnvironment env, IRoomService roomService,
            ISteadService steadService, IOfficeService officeService, IPremisService premisService, IParkingService parkingService,
            IStorageService storageService, IGarageService garageService)
        {
            _env = env;
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
            _houseService= houseService;
            _roomService = roomService;
            _steadService = steadService;
            _officeService = officeService;
            _premisService = premisService;
            _parkingService = parkingService;
            _storageService = storageService;
            _garageService = garageService;
		}

        public async Task<IActionResult> Index(int page = 1)
        {
            if (HttpContext.Session.GetString("User") != "employee")
            {
                return RedirectToAction("Index", "Home");
            }
            IEnumerable<ObjectsViewModel> estateObjects;

            IEnumerable<EstateObjectDTO> objects = await _objectService.GetEstateObjectByEmployeeId(HttpContext.Session.GetInt32("Id")!.Value);
            IEnumerable<OperationDTO> operations = await _operationService.GetAll();
            IEnumerable<AreaDTO> areas = await _areaService.GetAll();
            IEnumerable<CurrencyDTO> currencies = await _currencyService.GetAll();
            IEnumerable<LocationDTO> locations = await _locationService.GetLocations();
            IEnumerable<LocalityDTO> localities = await _localityService.GetLocalities();
            
            if (HttpContext.Session.GetString("IsAdmin") == "True")
            {
                IEnumerable<EstateObjectDTO> allObjects = await _objectService.GetAllEstateObjects(); //↑
                estateObjects = SelectEstateObject(allObjects, operations, areas, currencies, locations, localities);
            }
            else
            {
                 estateObjects = SelectEstateObject(objects, operations, areas, currencies, locations, localities);
            }
           
            var count = estateObjects.Count();
            var items = estateObjects.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
          
            ObjectPageViewModel objectPageViewModel = new ObjectPageViewModel(items, pageViewModel);

            ViewBag.OperatrionsList = new SelectList(await _operationService.GetAll(), "Id", "Name");
            ViewBag.LocalitiesList = new SelectList(await _localityService.GetLocalities(), "Id", "Name");
            ViewBag.EmployeesList = new SelectList(await _employeeService.GetEmployees(), "Id", "Name");
            return View(objectPageViewModel);

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
                string pathdirectory = @"/images/" + id;
                estateObjectDTO.pathPhoto = pathdirectory; //добавляем путь в обьект
                await _objectService.UpdateEstateObjectPath(estateObjectDTO); //обновляем обьект               
			}
		}

        #region create
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
                case "House":
                    return View("AddHouseView");
                case "Room":
                    return View("AddRoomView");
                case "Stead":
                    return View("AddSteadView");
                case "Office":
                    return View("AddOfficeView");
                case "Garage":
                    return View("AddGarageView");
                case "Premis":
                    return View("AddPremisView");
                case "Parking":
                    return View("AddParkingView");
                case "Storage":
                    return View("AddStorageView");

            }
            return View();
        }

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
                LocationDTO locationDTO = await CreateLocation(flatViewModel.RegionId, flatViewModel.DistrictId, flatViewModel.LocalityId, estateObjectDTO.Date); 
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
        public async Task<IActionResult> CreateHouse(AddHouseViewModel houseViewModel, IFormFileCollection formFiles)
        {
            //    1.Создать estateObject
            //    2. Добавить его в базу что бы получить Id и сформировать путь к папке с фотографиями
            //    3. Добавить фото
            //    4. Создать flatDTO
            //    5. Подать его в базу

            if (ModelState.IsValid && formFiles != null)
            {
                EstateObjectDTO estateObjectDTO = new EstateObjectDTO();
                ClientDTO clientDTO = await CreateClient(houseViewModel.Name, houseViewModel.Phone1);

                estateObjectDTO.clientId = clientDTO.Id;
                estateObjectDTO.employeeId = (int)HttpContext.Session.GetInt32("Id");
                estateObjectDTO.operationId = houseViewModel.OperationId;
               
                estateObjectDTO.LocalityId = houseViewModel.LocalityId;
                estateObjectDTO.Street = houseViewModel.Street;
                estateObjectDTO.numberStreet = houseViewModel.numberStreet;
                estateObjectDTO.Price = houseViewModel.Price;
                estateObjectDTO.currencyId = houseViewModel.currencyId;
                estateObjectDTO.Area = houseViewModel.Area;
                estateObjectDTO.unitAreaId = 1;             //нет смысла тянуть Id там 3 шт в базе
                estateObjectDTO.Description = houseViewModel.Description;
                estateObjectDTO.Status = false;
                estateObjectDTO.estateType = ObjectType.House;
                estateObjectDTO.Date = DateTime.Now;
                LocationDTO locationDTO = await CreateLocation(houseViewModel.RegionId, houseViewModel.DistrictId, houseViewModel.LocalityId, estateObjectDTO.Date);
                estateObjectDTO.locationId = locationDTO.Id;

                await _objectService.CreateEstateObject(estateObjectDTO); //создаем обьект

                estateObjectDTO = await _objectService.GetByDateTime(estateObjectDTO.Date); //получаем его из базы уже с id

                await AddFoto(estateObjectDTO, formFiles); //добавляем фото и получаем путь


                HouseDTO houseDTO = new HouseDTO();
                if (estateObjectDTO != null)
                {
                    houseDTO.estateObjectId = estateObjectDTO.Id;
                    houseDTO.Floors = houseViewModel.Floors;
                    houseDTO.Rooms = houseViewModel.Rooms;
                    houseDTO.kitchenArea = houseViewModel.kitchenArea;
                    houseDTO.livingArea = houseViewModel.livingArea;
                    houseDTO.steadArea = houseViewModel.steadArea;
                }
                await _houseService.CreateHouse(houseDTO); //сохраняем House в базу 

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> CreateRoom(AddRoomViewModel roomViewModel, IFormFileCollection formFiles)
        {
            //    1.Создать estateObject
            //    2. Добавить его в базу что бы получить Id и сформировать путь к папке с фотографиями
            //    3. Добавить фото
            //    4. Создать flatDTO
            //    5. Подать его в базу

            if (ModelState.IsValid && formFiles != null)
            {
                EstateObjectDTO estateObjectDTO = new EstateObjectDTO();
                ClientDTO clientDTO = await CreateClient(roomViewModel.Name, roomViewModel.Phone1);

                estateObjectDTO.clientId = clientDTO.Id;
                estateObjectDTO.employeeId = (int)HttpContext.Session.GetInt32("Id");
                estateObjectDTO.operationId = roomViewModel.OperationId;

                estateObjectDTO.LocalityId = roomViewModel.LocalityId;
                estateObjectDTO.Street = roomViewModel.Street;
                estateObjectDTO.numberStreet = roomViewModel.numberStreet;
                estateObjectDTO.Price = roomViewModel.Price;
                estateObjectDTO.currencyId = roomViewModel.currencyId;
                if(roomViewModel.Area == null)
                {
                    estateObjectDTO.Area = 0;
                }
                else
                {
                    estateObjectDTO.Area = (double)roomViewModel.Area;
                }
                
                estateObjectDTO.unitAreaId = 1;             //нет смысла тянуть Id там 3 шт в базе
                estateObjectDTO.Description = roomViewModel.Description;
                estateObjectDTO.Status = false;
                estateObjectDTO.estateType = ObjectType.Room;
                estateObjectDTO.Date = DateTime.Now;
                LocationDTO locationDTO = await CreateLocation(roomViewModel.RegionId, roomViewModel.DistrictId, roomViewModel.LocalityId, estateObjectDTO.Date);
                estateObjectDTO.locationId = locationDTO.Id;

                await _objectService.CreateEstateObject(estateObjectDTO); //создаем обьект

                estateObjectDTO = await _objectService.GetByDateTime(estateObjectDTO.Date); //получаем его из базы уже с id

                await AddFoto(estateObjectDTO, formFiles); //добавляем фото и получаем путь


                RoomDTO roomDTO = new RoomDTO();
                if (estateObjectDTO != null)
                {
                    roomDTO.estateObjectId = estateObjectDTO.Id;
                    roomDTO.Floor = roomViewModel.Floor;
                    roomDTO.Floors = roomViewModel.Floors;
                    roomDTO.livingArea = roomViewModel.livingArea;
                }
                await _roomService.CreateRoom(roomDTO); //сохраняем Room в базу 

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> CreateStead(AddSteadViewModel steadViewModel, IFormFileCollection formFiles)
        {
            //    1.Создать estateObject
            //    2. Добавить его в базу что бы получить Id и сформировать путь к папке с фотографиями
            //    3. Добавить фото
            //    4. Создать flatDTO
            //    5. Подать его в базу

            if (ModelState.IsValid && formFiles != null)
            {
                EstateObjectDTO estateObjectDTO = new EstateObjectDTO();
                ClientDTO clientDTO = await CreateClient(steadViewModel.Name, steadViewModel.Phone1);

                estateObjectDTO.clientId = clientDTO.Id;
                estateObjectDTO.employeeId = (int)HttpContext.Session.GetInt32("Id");
                estateObjectDTO.operationId = steadViewModel.OperationId;

                estateObjectDTO.LocalityId = steadViewModel.LocalityId;
                estateObjectDTO.Street = steadViewModel.Street;
                estateObjectDTO.numberStreet = steadViewModel.numberStreet;
                estateObjectDTO.Price = steadViewModel.Price;
                estateObjectDTO.currencyId = steadViewModel.currencyId;
                if (steadViewModel.Area == null)
                {
                    estateObjectDTO.Area = 0;
                }
                else
                {
                    estateObjectDTO.Area = (double)steadViewModel.Area;
                }

                estateObjectDTO.unitAreaId = 1;             //нет смысла тянуть Id там 3 шт в базе
                estateObjectDTO.Description = steadViewModel.Description;
                estateObjectDTO.Status = false;
                estateObjectDTO.estateType = ObjectType.Stead;
                estateObjectDTO.Date = DateTime.Now;
                LocationDTO locationDTO = await CreateLocation(steadViewModel.RegionId, steadViewModel.DistrictId, steadViewModel.LocalityId, estateObjectDTO.Date);
                estateObjectDTO.locationId = locationDTO.Id;

                await _objectService.CreateEstateObject(estateObjectDTO); //создаем обьект

                estateObjectDTO = await _objectService.GetByDateTime(estateObjectDTO.Date); //получаем его из базы уже с id

                await AddFoto(estateObjectDTO, formFiles); //добавляем фото и получаем путь


                SteadDTO steadDTO = new SteadDTO();
                if (estateObjectDTO != null)
                {
                    steadDTO.estateObjectId = estateObjectDTO.Id;
                    steadDTO.Cadastr = steadViewModel.Cadastr;
                    if(steadViewModel.Use == 1)
                    {
                        steadDTO.Use = LandUse.residential;
                    }
                    else if(steadViewModel.Use == 2)
                    {
                        steadDTO.Use = LandUse.industrial;
                    }
                    else if (steadViewModel.Use == 3)
                    {
                        steadDTO.Use = LandUse.gardening;
                    }
                    else if (steadViewModel.Use == 4)
                    {
                        steadDTO.Use = LandUse.agricultural;
                    }
                    steadDTO.estateObjectId = estateObjectDTO.Id;
                }
                await _steadService.CreateStead(steadDTO); //сохраняем Room в базу 

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> CreateOffice(AddOfficeViewModel officeViewModel, IFormFileCollection formFiles)

        {
            //    1.Создать estateObject
            //    2. Добавить его в базу что бы получить Id и сформировать путь к папке с фотографиями
            //    3. Добавить фото
            //    4. Создать flatDTO
            //    5. Подать его в базу

            //в модели пропущены поля этажности и количества комнат

            if (ModelState.IsValid && formFiles != null)
            {
                EstateObjectDTO estateObjectDTO = new EstateObjectDTO();
                ClientDTO clientDTO = await CreateClient(officeViewModel.Name, officeViewModel.Phone1);

                estateObjectDTO.clientId = clientDTO.Id;
                estateObjectDTO.employeeId = (int)HttpContext.Session.GetInt32("Id");
                estateObjectDTO.operationId = officeViewModel.OperationId;
                estateObjectDTO.LocalityId = officeViewModel.LocalityId;
                estateObjectDTO.Street = officeViewModel.Street;
                estateObjectDTO.numberStreet = officeViewModel.numberStreet;
                estateObjectDTO.Price = officeViewModel.Price;
                estateObjectDTO.currencyId = officeViewModel.currencyId;
                estateObjectDTO.Area = officeViewModel.Area;
                estateObjectDTO.unitAreaId = 1;             //нет смысла тянуть Id там 3 шт в базе
                estateObjectDTO.Description = officeViewModel.Description;
                estateObjectDTO.Status = false;
                estateObjectDTO.estateType = ObjectType.Office;
                estateObjectDTO.Date = DateTime.Now;
                LocationDTO locationDTO = await CreateLocation(officeViewModel.RegionId, officeViewModel.DistrictId, officeViewModel.LocalityId, estateObjectDTO.Date);
                estateObjectDTO.locationId = locationDTO.Id;

                await _objectService.CreateEstateObject(estateObjectDTO); //создаем обьект

                estateObjectDTO = await _objectService.GetByDateTime(estateObjectDTO.Date); //получаем его из базы уже с id

                await AddFoto(estateObjectDTO, formFiles); //добавляем фото и получаем путь


                OfficeDTO officeDTO = new OfficeDTO();
                if (estateObjectDTO != null)
                {
                    officeDTO.estateObjectId = estateObjectDTO.Id;                 
                }
                await _officeService.CreateOffice(officeDTO); //сохраняем Flat в базу 

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> CreatePremis(AddPremisViewModel premisViewModel, IFormFileCollection formFiles)
        {
            //    1.Создать estateObject
            //    2. Добавить его в базу что бы получить Id и сформировать путь к папке с фотографиями
            //    3. Добавить фото
            //    4. Создать flatDTO
            //    5. Подать его в базу

            //в модели пропущены поля этажности и количества комнат

            if (ModelState.IsValid && formFiles != null)
            {
                EstateObjectDTO estateObjectDTO = new EstateObjectDTO();
                ClientDTO clientDTO = await CreateClient(premisViewModel.Name, premisViewModel.Phone1);

                estateObjectDTO.clientId = clientDTO.Id;
                estateObjectDTO.employeeId = (int)HttpContext.Session.GetInt32("Id");
                estateObjectDTO.operationId = premisViewModel.OperationId;
                estateObjectDTO.LocalityId = premisViewModel.LocalityId;
                estateObjectDTO.Street = premisViewModel.Street;
                estateObjectDTO.numberStreet = premisViewModel.numberStreet;
                estateObjectDTO.Price = premisViewModel.Price;
                estateObjectDTO.currencyId = premisViewModel.currencyId;
                estateObjectDTO.Area = premisViewModel.Area;
                estateObjectDTO.unitAreaId = 1;             //нет смысла тянуть Id там 3 шт в базе
                estateObjectDTO.Description = premisViewModel.Description;
                estateObjectDTO.Status = false;
                estateObjectDTO.estateType = ObjectType.Premis;
                estateObjectDTO.Date = DateTime.Now;
                LocationDTO locationDTO = await CreateLocation(premisViewModel.RegionId, premisViewModel.DistrictId, premisViewModel.LocalityId, estateObjectDTO.Date);
                estateObjectDTO.locationId = locationDTO.Id;

                await _objectService.CreateEstateObject(estateObjectDTO); //создаем обьект

                estateObjectDTO = await _objectService.GetByDateTime(estateObjectDTO.Date); //получаем его из базы уже с id

                await AddFoto(estateObjectDTO, formFiles); //добавляем фото и получаем путь


                PremisDTO premisDTO = new PremisDTO();
                if (estateObjectDTO != null)
                {
                    premisDTO.estateObjectId = estateObjectDTO.Id;
                }
                await _premisService.Create(premisDTO); //сохраняем Premis в базу 

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> CreateParking(AddParkingViewModel parkingViewModel, IFormFileCollection formFiles)
        {
            //    1.Создать estateObject
            //    2. Добавить его в базу что бы получить Id и сформировать путь к папке с фотографиями
            //    3. Добавить фото
            //    4. Создать flatDTO
            //    5. Подать его в базу

            //в модели пропущены поля этажности и количества комнат

            if (ModelState.IsValid && formFiles != null)
            {
                EstateObjectDTO estateObjectDTO = new EstateObjectDTO();
                ClientDTO clientDTO = await CreateClient(parkingViewModel.Name, parkingViewModel.Phone1);

                estateObjectDTO.clientId = clientDTO.Id;
                estateObjectDTO.employeeId = (int)HttpContext.Session.GetInt32("Id");
                estateObjectDTO.operationId = parkingViewModel.OperationId;
                estateObjectDTO.LocalityId = parkingViewModel.LocalityId;
                estateObjectDTO.Street = parkingViewModel.Street;
                estateObjectDTO.numberStreet = parkingViewModel.numberStreet;
                estateObjectDTO.Price = parkingViewModel.Price;
                estateObjectDTO.currencyId = parkingViewModel.currencyId;
                estateObjectDTO.Area = parkingViewModel.Area;
                estateObjectDTO.unitAreaId = 1;             //нет смысла тянуть Id там 3 шт в базе
                estateObjectDTO.Description = parkingViewModel.Description;
                estateObjectDTO.Status = false;
                estateObjectDTO.estateType = ObjectType.Parking;
                estateObjectDTO.Date = DateTime.Now;
                LocationDTO locationDTO = await CreateLocation(parkingViewModel.RegionId, parkingViewModel.DistrictId, parkingViewModel.LocalityId, estateObjectDTO.Date);
                estateObjectDTO.locationId = locationDTO.Id;

                await _objectService.CreateEstateObject(estateObjectDTO); //создаем обьект

                estateObjectDTO = await _objectService.GetByDateTime(estateObjectDTO.Date); //получаем его из базы уже с id

                await AddFoto(estateObjectDTO, formFiles); //добавляем фото и получаем путь


                ParkingDTO parkingDTO = new ParkingDTO();
                if (estateObjectDTO != null)
                {
                    parkingDTO.estateObjectId = estateObjectDTO.Id;
                }
                await _parkingService.CreateParking(parkingDTO);  

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> CreateStorage(AddStorageViewModel storageViewModel, IFormFileCollection formFiles)
        {
            //    1.Создать estateObject
            //    2. Добавить его в базу что бы получить Id и сформировать путь к папке с фотографиями
            //    3. Добавить фото
            //    4. Создать flatDTO
            //    5. Подать его в базу

            //в модели пропущены поля этажности и количества комнат

            if (ModelState.IsValid && formFiles != null)
            {
                EstateObjectDTO estateObjectDTO = new EstateObjectDTO();
                ClientDTO clientDTO = await CreateClient(storageViewModel.Name, storageViewModel.Phone1);

                estateObjectDTO.clientId = clientDTO.Id;
                estateObjectDTO.employeeId = (int)HttpContext.Session.GetInt32("Id");
                estateObjectDTO.operationId = storageViewModel.OperationId;
                estateObjectDTO.LocalityId = storageViewModel.LocalityId;
                estateObjectDTO.Street = storageViewModel.Street;
                estateObjectDTO.numberStreet = storageViewModel.numberStreet;
                estateObjectDTO.Price = storageViewModel.Price;
                estateObjectDTO.currencyId = storageViewModel.currencyId;
                estateObjectDTO.Area = storageViewModel.Area;
                estateObjectDTO.unitAreaId = 1;             //нет смысла тянуть Id там 3 шт в базе
                estateObjectDTO.Description = storageViewModel.Description;
                estateObjectDTO.Status = false;
                estateObjectDTO.estateType = ObjectType.Storage;
                estateObjectDTO.Date = DateTime.Now;
                LocationDTO locationDTO = await CreateLocation(storageViewModel.RegionId, storageViewModel.DistrictId, storageViewModel.LocalityId, estateObjectDTO.Date);
                estateObjectDTO.locationId = locationDTO.Id;

                await _objectService.CreateEstateObject(estateObjectDTO); //создаем обьект

                estateObjectDTO = await _objectService.GetByDateTime(estateObjectDTO.Date); //получаем его из базы уже с id

                await AddFoto(estateObjectDTO, formFiles); //добавляем фото и получаем путь


                StorageDTO storageDTO = new StorageDTO();
                if (estateObjectDTO != null)
                {
                    storageDTO.estateObjectId = estateObjectDTO.Id;
                }
                await _storageService.CreateStorage(storageDTO); //сохраняем Flat в базу 

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> CreateGarage(AddGarageViewModel garageViewModel, IFormFileCollection formFiles)
        {
           
            //в модели пропущены поля этажности и количества комнат

            if (ModelState.IsValid && formFiles != null)
            {
                EstateObjectDTO estateObjectDTO = new EstateObjectDTO();
                ClientDTO clientDTO = await CreateClient(garageViewModel.Name, garageViewModel.Phone1);

                estateObjectDTO.clientId = clientDTO.Id;
                estateObjectDTO.employeeId = (int)HttpContext.Session.GetInt32("Id");
                estateObjectDTO.operationId = garageViewModel.OperationId;
                estateObjectDTO.LocalityId = garageViewModel.LocalityId;
                estateObjectDTO.Street = garageViewModel.Street;
                estateObjectDTO.numberStreet = garageViewModel.numberStreet;
                estateObjectDTO.Price = garageViewModel.Price;
                estateObjectDTO.currencyId = garageViewModel.currencyId;
                estateObjectDTO.Area = garageViewModel.Area;
                estateObjectDTO.unitAreaId = 1;             //нет смысла тянуть Id там 3 шт в базе
                estateObjectDTO.Description = garageViewModel.Description;
                estateObjectDTO.Status = false;
                estateObjectDTO.estateType = ObjectType.Garage;
                estateObjectDTO.Date = DateTime.Now;
                LocationDTO locationDTO = await CreateLocation(garageViewModel.RegionId, garageViewModel.DistrictId, garageViewModel.LocalityId, estateObjectDTO.Date);
                estateObjectDTO.locationId = locationDTO.Id;

                await _objectService.CreateEstateObject(estateObjectDTO); //создаем обьект

                estateObjectDTO = await _objectService.GetByDateTime(estateObjectDTO.Date); //получаем его из базы уже с id

                await AddFoto(estateObjectDTO, formFiles); //добавляем фото и получаем путь


                GarageDTO garageDTO = new GarageDTO();
                if (estateObjectDTO != null)
                {
                    garageDTO.estateObjectId = estateObjectDTO.Id;
                    garageDTO.Floors = garageViewModel.Floors;
                }
                await _garageService.CreateGarage(garageDTO); //сохраняем Flat в базу 

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
        private async Task<LocationDTO> CreateLocation(int regionId, int districtId, int localityId, DateTime dateTime)
        {
            LocationDTO locationDTO = new LocationDTO();
            locationDTO.CountryId = 1; //в базе одна страна
            locationDTO.RegionId = regionId;
            locationDTO.DistrictId = districtId;
            locationDTO.LocalityId = localityId;
            locationDTO.Date = dateTime;
            await _locationService.CreateLocation(locationDTO);
            locationDTO = await _locationService.GetByDateTime(dateTime);
            return locationDTO;
        }

        #endregion

        #region update
        public async Task<IActionResult> Update(int id, string typeObject)
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
                    case "House":
                        HouseDTO house = await _houseService.GetHouseByEstateObjectId(id);
                        return View("UpdateHouse", SelectHouse(house));
                    case "Room":
                        RoomDTO room = await _roomService.GetRoomByEstateObjectId(id);
                        return View("UpdateRoom", SelectRoom(room));
                   case "Office":
                        OfficeDTO office = await _officeService.GetOfficeByEstateObjectId(id);
                        return View("UpdateOffice" , SelectOffice(office));
                    case "Stead":
                        SteadDTO stead = await _steadService.GetSteadByEstateObjectId(id);
                        return View("UpdateStead", SelectStead(stead));
                    case "Garage":
                        GarageDTO garage = await _garageService.GetGarageByEstateObjectId(id);
                        return View("UpdateGarage", SelectGarage(garage));
                    case "Premis":
                        PremisDTO premis = await _premisService.GetPremisByEstateObjectId(id);
                        return View("UpdatePremis", SelectPremis(premis));
                    case "Parking":
                        ParkingDTO parking = await _parkingService.GetParkingByEstateObjectId(id);
                        return View("UpdateParking", SelectParking(parking));
                    case "Storage":
                        StorageDTO storage = await _storageService.GetStorageByEstateObjectId(id);
                        return View("UpdateStorage",SelectStorage(storage));

                }

            }
            catch
            {
                return NotFound();
            }
            return NotFound();
        }
        public async Task<IActionResult> UpdateFlat(UpdateFlatViewModel model, IFormFileCollection formFiles)
        {
            try
            {
                await _clientService.UpdateClientNameAndPhone(model.clientId, model.Name, model.Phone1);

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

                //update photos
                if (formFiles != null)
                {
                    try
                    {
                        string folder = Path.Combine(_env.WebRootPath);
                        folder = folder + model.Path;
                        Directory.Delete(folder, true);
                        var estateObject = await _objectService.GetEstateObjectById(model.estateObjectId);
                        await AddFoto(estateObject, formFiles);
                    }
                    catch (Exception ex) 
                    {
                        Console.WriteLine(ex.Message);
                    }
                   

                  
                }
              

             
                return RedirectToAction("Index", "Office");

            }
            catch 
            {
                return View(model);
            }
            
          
        }
        public async Task<IActionResult> UpdateHouse(UpdateHouseViewModel model, IFormFileCollection formFiles)
        {
            try
            {
                await _clientService.UpdateClientNameAndPhone(model.clientId, model.Name, model.Phone1);

                LocationDTO locationDTO = new LocationDTO();
                locationDTO.Id = model.locationId;
                locationDTO.CountryId = 1;
                locationDTO.LocalityId = model.LocalityId;
                locationDTO.RegionId = model.RegionId;
                locationDTO.DistrictId = model.DistrictId;
                await _locationService.UpdateLocation(locationDTO);


                HouseDTO houseDTO = new HouseDTO();
                houseDTO.Id = model.houseId;
                houseDTO.Floors = model.Floors;
                houseDTO.Rooms = model.Rooms;
                houseDTO.livingArea = model.livingArea;
                houseDTO.kitchenArea = model.kitchenArea;
                houseDTO.steadArea = model.steadArea;
                houseDTO.estateObjectId = model.estateObjectId;
                await _houseService.UpdateHouse(houseDTO);


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
                objectDTO.Date = model.Date;
                objectDTO.clientId = model.clientId;
                objectDTO.estateType = ObjectType.House;
                objectDTO.pathPhoto = model.Path;
                objectDTO.Status = model.status;
                await _objectService.UpdateEstateObject(objectDTO);

                //update photos
                if (formFiles.Count != 0)
                {
                    try
                    {
                        string folder = Path.Combine(_env.WebRootPath);
                        folder = folder + model.Path;
                        Directory.Delete(folder, true);
                        var estateObject = await _objectService.GetEstateObjectById(model.estateObjectId);
                        await AddFoto(estateObject, formFiles);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }



                return RedirectToAction("Index", "Office");

            }
            catch
            {
                return View(model);
            }


        }
        public async Task<IActionResult> UpdateRoom(UpdateRoomViewModel model, IFormFileCollection formFiles)
        {
            try
            {
                await _clientService.UpdateClientNameAndPhone(model.clientId, model.Name, model.Phone1);

                LocationDTO locationDTO = new LocationDTO();
                locationDTO.Id = model.locationId;
                locationDTO.CountryId = 1;
                locationDTO.LocalityId = model.LocalityId;
                locationDTO.RegionId = model.RegionId;
                locationDTO.DistrictId = model.DistrictId;
                await _locationService.UpdateLocation(locationDTO);


                RoomDTO roomDTO = new RoomDTO();
                roomDTO.Id = model.roomId;
                roomDTO.Floors = model.Floors;
                roomDTO.Floor = model.Floor;
                roomDTO.livingArea = model.livingArea;
                roomDTO.estateObjectId = model.estateObjectId;
                await _roomService.UpdateRoom(roomDTO);


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
                objectDTO.Area = (double)model.Area;
                objectDTO.unitAreaId = model.unitAreaId;
                objectDTO.Description = model.Description;
                objectDTO.Date = model.Date;
                objectDTO.clientId = model.clientId;
                objectDTO.estateType = ObjectType.Room;
                objectDTO.pathPhoto = model.Path;
                objectDTO.Status = model.status;
                await _objectService.UpdateEstateObject(objectDTO); //тут исключение

                //update photos
                if (formFiles.Count != 0)
                {
                    try
                    {
                        string folder = Path.Combine(_env.WebRootPath);
                        folder = folder + model.Path;
                        Directory.Delete(folder, true);
                        var estateObject = await _objectService.GetEstateObjectById(model.estateObjectId);
                        await AddFoto(estateObject, formFiles);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }



                return RedirectToAction("Index", "Office");

            }
            catch
            {
                return NotFound();
            }


        }
        public async Task<IActionResult> UpdateOffice(UpdateOfficeViewModel model, IFormFileCollection formFiles)
        {
            try
            {
                await _clientService.UpdateClientNameAndPhone(model.clientId, model.Name, model.Phone1);

                LocationDTO locationDTO = new LocationDTO();
                locationDTO.Id = model.locationId;
                locationDTO.CountryId = 1;
                locationDTO.LocalityId = model.LocalityId;
                locationDTO.RegionId = model.RegionId;
                locationDTO.DistrictId = model.DistrictId;
                await _locationService.UpdateLocation(locationDTO);


                OfficeDTO officeDTO = new OfficeDTO();
                officeDTO.Id = model.officeId;
                officeDTO.estateObjectId = model.estateObjectId;
                await _officeService.Update(officeDTO);


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
                objectDTO.Area = (double)model.Area;
                objectDTO.unitAreaId = model.unitAreaId;
                objectDTO.Description = model.Description;
                objectDTO.Date = model.Date;
                objectDTO.clientId = model.clientId;
                objectDTO.estateType = ObjectType.Office;
                objectDTO.pathPhoto = model.Path;
                objectDTO.Status = model.status;
                await _objectService.UpdateEstateObject(objectDTO); 

                //update photos
                if (formFiles.Count != 0)
                {
                    try
                    {
                        string folder = Path.Combine(_env.WebRootPath);
                        folder = folder + model.Path;
                        Directory.Delete(folder, true);
                        var estateObject = await _objectService.GetEstateObjectById(model.estateObjectId);
                        await AddFoto(estateObject, formFiles);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }



                return RedirectToAction("Index", "Office");

            }
            catch
            {
                return NotFound();
            }


        }
        public async Task<IActionResult> UpdatePremis(UpdatePremisViewModel model, IFormFileCollection formFiles)
        {
            try
            {
                await _clientService.UpdateClientNameAndPhone(model.clientId, model.Name, model.Phone1);

                LocationDTO locationDTO = new LocationDTO();
                locationDTO.Id = model.locationId;
                locationDTO.CountryId = 1;
                locationDTO.LocalityId = model.LocalityId;
                locationDTO.RegionId = model.RegionId;
                locationDTO.DistrictId = model.DistrictId;
                await _locationService.UpdateLocation(locationDTO);


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
                objectDTO.Area = (double)model.Area;
                objectDTO.unitAreaId = model.unitAreaId;
                objectDTO.Description = model.Description;
                objectDTO.Date = model.Date;
                objectDTO.clientId = model.clientId;
                objectDTO.estateType = ObjectType.Premis;
                objectDTO.pathPhoto = model.Path;
                objectDTO.Status = model.status;
                await _objectService.UpdateEstateObject(objectDTO);

                //update photos
                if (formFiles.Count != 0)
                {
                    try
                    {
                        string folder = Path.Combine(_env.WebRootPath);
                        folder = folder + model.Path;
                        Directory.Delete(folder, true);
                        var estateObject = await _objectService.GetEstateObjectById(model.estateObjectId);
                        await AddFoto(estateObject, formFiles);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }

                return RedirectToAction("Index", "Office");

            }
            catch
            {
                return NotFound();
            }


        }
        public async Task<IActionResult> UpdateStead(UpdateSteadViewModel model, IFormFileCollection formFiles)
        {
            try
            {
                await _clientService.UpdateClientNameAndPhone(model.clientId, model.Name, model.Phone1);

                LocationDTO locationDTO = new LocationDTO();
                locationDTO.Id = model.locationId;
                locationDTO.CountryId = 1;
                locationDTO.LocalityId = model.LocalityId;
                locationDTO.RegionId = model.RegionId;
                locationDTO.DistrictId = model.DistrictId;
                await _locationService.UpdateLocation(locationDTO);


                SteadDTO steadDTO = new SteadDTO();
                steadDTO.Id = model.steadId;
                steadDTO.Cadastr = model.Cadastr;
                if (model.Use == 1)
                {
                    steadDTO.Use = LandUse.residential;
                }
                else if (model.Use == 2)
                {
                    steadDTO.Use = LandUse.industrial;
                }
                else if (model.Use == 3)
                {
                    steadDTO.Use = LandUse.gardening;
                }
                else if (model.Use == 4)
                {
                    steadDTO.Use = LandUse.agricultural;
                }
                steadDTO.estateObjectId = model.estateObjectId;
                await _steadService.UpdateStead(steadDTO);


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
                objectDTO.Area = (double)model.Area;
                objectDTO.unitAreaId = model.unitAreaId;
                objectDTO.Description = model.Description;
                objectDTO.Date = model.Date;
                objectDTO.clientId = model.clientId;
                objectDTO.estateType = ObjectType.Stead;
                objectDTO.pathPhoto = model.Path;
                objectDTO.Status = model.status;
                await _objectService.UpdateEstateObject(objectDTO);

                //update photos
                if (formFiles.Count != 0)
                {
                    try
                    {
                        string folder = Path.Combine(_env.WebRootPath);
                        folder = folder + model.Path;
                        Directory.Delete(folder, true);
                        var estateObject = await _objectService.GetEstateObjectById(model.estateObjectId);
                        await AddFoto(estateObject, formFiles);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }

                return RedirectToAction("Index", "Office");

            }
            catch
            {
                return NotFound();
            }


        }
        public async Task<IActionResult> UpdateGarage(UpdateGarageViewModel model, IFormFileCollection formFiles)
        {
            try
            {
                await _clientService.UpdateClientNameAndPhone(model.clientId, model.Name, model.Phone1);

                LocationDTO locationDTO = new LocationDTO();
                locationDTO.Id = model.locationId;
                locationDTO.CountryId = 1;
                locationDTO.LocalityId = model.LocalityId;
                locationDTO.RegionId = model.RegionId;
                locationDTO.DistrictId = model.DistrictId;
                await _locationService.UpdateLocation(locationDTO);


                GarageDTO garageDTO = new GarageDTO();
                garageDTO.Id = model.garageId;
                garageDTO.Floors = model.Floors;
                garageDTO.estateObjectId = model.estateObjectId;
                await _garageService.UpdateGarage(garageDTO);


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
                objectDTO.Area = (double)model.Area;
                objectDTO.unitAreaId = model.unitAreaId;
                objectDTO.Description = model.Description;
                objectDTO.Date = model.Date;
                objectDTO.clientId = model.clientId;
                objectDTO.estateType = ObjectType.Garage;
                objectDTO.pathPhoto = model.Path;
                objectDTO.Status = model.status;
                await _objectService.UpdateEstateObject(objectDTO);

                //update photos
                if (formFiles.Count != 0)
                {
                    try
                    {
                        string folder = Path.Combine(_env.WebRootPath);
                        folder = folder + model.Path;
                        Directory.Delete(folder, true);
                        var estateObject = await _objectService.GetEstateObjectById(model.estateObjectId);
                        await AddFoto(estateObject, formFiles);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }



                return RedirectToAction("Index", "Office");

            }
            catch
            {
                return NotFound();
            }


        }
        public async Task<IActionResult> UpdateParking(UpdateParkingViewModel model, IFormFileCollection formFiles)
        {
            try
            {
                await _clientService.UpdateClientNameAndPhone(model.clientId, model.Name, model.Phone1);

                LocationDTO locationDTO = new LocationDTO();
                locationDTO.Id = model.locationId;
                locationDTO.CountryId = 1;
                locationDTO.LocalityId = model.LocalityId;
                locationDTO.RegionId = model.RegionId;
                locationDTO.DistrictId = model.DistrictId;
                await _locationService.UpdateLocation(locationDTO);



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
                objectDTO.Area = (double)model.Area;
                objectDTO.unitAreaId = model.unitAreaId;
                objectDTO.Description = model.Description;
                objectDTO.Date = model.Date;
                objectDTO.clientId = model.clientId;
                objectDTO.estateType = ObjectType.Parking;
                objectDTO.pathPhoto = model.Path;
                objectDTO.Status = model.status;
                await _objectService.UpdateEstateObject(objectDTO);

                //update photos
                if (formFiles.Count != 0)
                {
                    try
                    {
                        string folder = Path.Combine(_env.WebRootPath);
                        folder = folder + model.Path;
                        Directory.Delete(folder, true);
                        var estateObject = await _objectService.GetEstateObjectById(model.estateObjectId);
                        await AddFoto(estateObject, formFiles);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }


                return RedirectToAction("Index", "Office");

            }
            catch
            {
                return NotFound();
            }


        }
        public async Task<IActionResult> UpdateStorage(UpdateStorageViewModel model, IFormFileCollection formFiles)
        {
            try
            {
                await _clientService.UpdateClientNameAndPhone(model.clientId, model.Name, model.Phone1);

                LocationDTO locationDTO = new LocationDTO();
                locationDTO.Id = model.locationId;
                locationDTO.CountryId = 1;
                locationDTO.LocalityId = model.LocalityId;
                locationDTO.RegionId = model.RegionId;
                locationDTO.DistrictId = model.DistrictId;
                await _locationService.UpdateLocation(locationDTO);


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
                objectDTO.Area = (double)model.Area;
                objectDTO.unitAreaId = model.unitAreaId;
                objectDTO.Description = model.Description;
                objectDTO.Date = model.Date;
                objectDTO.clientId = model.clientId;
                objectDTO.estateType = ObjectType.Storage;
                objectDTO.pathPhoto = model.Path;
                objectDTO.Status = model.status;
                await _objectService.UpdateEstateObject(objectDTO);

                //update photos
                if (formFiles.Count != 0)
                {
                    try
                    {
                        string folder = Path.Combine(_env.WebRootPath);
                        folder = folder + model.Path;
                        Directory.Delete(folder, true);
                        var estateObject = await _objectService.GetEstateObjectById(model.estateObjectId);
                        await AddFoto(estateObject, formFiles);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }



                return RedirectToAction("Index", "Office");

            }
            catch
            {
                return NotFound();
            }


        }
        public UpdateFlatViewModel SelectFlat(FlatDTO flat)
        {
            string rootFolder = Path.Combine(_env.WebRootPath);
            rootFolder = rootFolder  + flat.pathPhoto;
            
            List<string> imagePaths = GetImagePaths(rootFolder, flat.estateObjectId);

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
                countViews = flat.countViews,
                photos = imagePaths,
                Name = flat.clientName,
                Phone1 = flat.clientPhone

            };
            return viewModel;
        }
        public UpdateHouseViewModel SelectHouse(HouseDTO house)
        {
            string rootFolder = Path.Combine(_env.WebRootPath);
            rootFolder = rootFolder + house.pathPhoto;

            List<string> imagePaths = GetImagePaths(rootFolder, house.estateObjectId);

            var viewModel = new UpdateHouseViewModel
            {

                houseId = house.Id,
                employeeId = house.employeeId,
                OperationId = house.operationId,
                Street = house.Street,
                numberStreet = (int)house.numberStreet,
                Price = house.Price,
                currencyId = house.currencyId,
                Area = house.Area,
               
                Description = house.Description,
                Path = house.pathPhoto,
                Floors = house.Floors,
                Rooms = house.Rooms,
                kitchenArea = house.kitchenArea,
                livingArea = house.livingArea,
                steadArea = house.steadArea,
                status = house.Status,
                estateObjectId = house.estateObjectId,
                locationId = house.locationId,
                LocalityId = (int)house.LocalityId,
                RegionId = (int)house.RegionId,
                countryId = house.countryId,
                DistrictId = (int)house.DistrictId,
                clientId = house.clientId,
                Date = house.Date,
                countViews = house.countViews,
                photos = imagePaths,
                Name = house.clientName,
                Phone1 = house.clientPhone

            };
            return viewModel;
        }
        public UpdateRoomViewModel SelectRoom(RoomDTO room)
        {
            string rootFolder = Path.Combine(_env.WebRootPath);
            rootFolder = rootFolder + room.pathPhoto;

            List<string> imagePaths = GetImagePaths(rootFolder, room.estateObjectId);

            var viewModel = new UpdateRoomViewModel
            {

                roomId = room.Id,
                employeeId = room.employeeId,
                OperationId = room.operationId,
                Street = room.Street,
                numberStreet = (int)room.numberStreet,
                Price = room.Price,
                currencyId = room.currencyId,
                Area = room.Area,
                unitAreaId = room.unitAreaId,
                Description = room.Description,
                Path = room.pathPhoto,
                Floor = room.Floor,
                Floors = room.Floors,
                livingArea = room.livingArea,
                status = room.Status,
                estateObjectId = room.estateObjectId,
                locationId = room.locationId,
                LocalityId = (int)room.LocalityId,
                RegionId = (int)room.RegionId,
                countryId = room.countryId,
                DistrictId = (int)room.DistrictId,
                clientId = room.clientId,
                Date = room.Date,
                countViews = room.countViews,
                photos = imagePaths,
                Name = room.clientName,
                Phone1 = room.clientPhone

            };
            return viewModel;
        }
        public UpdateOfficeViewModel SelectOffice(OfficeDTO office)
        {
            string rootFolder = Path.Combine(_env.WebRootPath);
            rootFolder = rootFolder + office.pathPhoto;

            List<string> imagePaths = GetImagePaths(rootFolder, office.estateObjectId);

            var viewModel = new UpdateOfficeViewModel
            {

                officeId = office.Id,
                employeeId = office.employeeId,
                OperationId = office.operationId,
                Street = office.Street,
                numberStreet = (int)office.numberStreet,
                Price = office.Price,
                currencyId = office.currencyId,
                Area = office.Area,
                unitAreaId = office.unitAreaId,
                Description = office.Description,
                Path = office.pathPhoto,
                status = office.Status,
                estateObjectId = office.estateObjectId,
                locationId = office.locationId,
                LocalityId = (int)office.LocalityId,
                RegionId = (int)office.RegionId,
                countryId = office.countryId,
                DistrictId = (int)office.DistrictId,
                clientId = office.clientId,
                Date = office.Date,
                countViews = office.countViews,
                photos = imagePaths,
                Name = office.clientName,
                Phone1 = office.clientPhone

            };
            return viewModel;
        }
        public UpdatePremisViewModel SelectPremis(PremisDTO premis)
        {
            string rootFolder = Path.Combine(_env.WebRootPath);
            rootFolder = rootFolder + premis.pathPhoto;

            List<string> imagePaths = GetImagePaths(rootFolder, premis.estateObjectId);

            var viewModel = new UpdatePremisViewModel
            {

                premisId = premis.Id,
                employeeId = premis.employeeId,
                OperationId = premis.operationId,
                Street = premis.Street,
                numberStreet = (int)premis.numberStreet,
                Price = premis.Price,
                currencyId = premis.currencyId,
                Area = premis.Area,
                unitAreaId = premis.unitAreaId,
                Description = premis.Description,
                Path = premis.pathPhoto,
                status = premis.Status,
                estateObjectId = premis.estateObjectId,
                locationId = premis.locationId,
                LocalityId = (int)premis.LocalityId,
                RegionId = (int)premis.RegionId,
                countryId = premis.countryId,
                DistrictId = (int)premis.DistrictId,
                clientId = premis.clientId,
                Date = premis.Date,
                countViews = premis.countViews,
                photos = imagePaths,
                Name = premis.clientName,
                Phone1 = premis.clientPhone

            };
            return viewModel;
        }
        public UpdateSteadViewModel SelectStead(SteadDTO stead)
        {
            string rootFolder = Path.Combine(_env.WebRootPath);
            rootFolder = rootFolder + stead.pathPhoto;

            List<string> imagePaths = GetImagePaths(rootFolder, stead.estateObjectId);

            var viewModel = new UpdateSteadViewModel
            {

                steadId = stead.Id,
                employeeId = stead.employeeId,
                OperationId = stead.operationId,
                Street = stead.Street,
                numberStreet = (int)stead.numberStreet,
                Price = stead.Price,
                currencyId = stead.currencyId,
                Area = stead.Area,
                unitAreaId = stead.unitAreaId,
                Cadastr = stead.Cadastr,
                Use = (int)stead.Use,
                Description = stead.Description,
                Path = stead.pathPhoto,
                status = stead.Status,
                estateObjectId = stead.estateObjectId,
                locationId = stead.locationId,
                LocalityId = (int)stead.LocalityId,
                RegionId = (int)stead.RegionId,
                countryId = stead.countryId,
                DistrictId = (int)stead.DistrictId,
                clientId = stead.clientId,
                Date = stead.Date,
                countViews = stead.countViews,
                photos = imagePaths,
                Name = stead.clientName,
                Phone1 = stead.clientPhone,
               

            };
            return viewModel;
        }
        public UpdateGarageViewModel SelectGarage(GarageDTO garage)
        {
            string rootFolder = Path.Combine(_env.WebRootPath);
            rootFolder = rootFolder + garage.pathPhoto;

            List<string> imagePaths = GetImagePaths(rootFolder, garage.estateObjectId);

            var viewModel = new UpdateGarageViewModel
            {

                garageId = garage.Id,
                employeeId = garage.employeeId,
                Floors = garage.Floors,
                OperationId = garage.operationId,
                Street = garage.Street,
                numberStreet = (int)garage.numberStreet,
                Price = garage.Price,
                currencyId = garage.currencyId,
                Area = garage.Area,
                unitAreaId = garage.unitAreaId,
                Description = garage.Description,
                Path = garage.pathPhoto,
                status = garage.Status,
                estateObjectId = garage.estateObjectId,
                locationId = garage.locationId,
                LocalityId = (int)garage.LocalityId,
                RegionId = (int)garage.RegionId,
                countryId = garage.countryId,
                DistrictId = (int)garage.DistrictId,
                clientId = garage.clientId,
                Date = garage.Date,
                countViews = garage.countViews,
                photos = imagePaths,
                Name = garage.clientName,
                Phone1 = garage.clientPhone

            };
            return viewModel;
        }
        public UpdateParkingViewModel SelectParking(ParkingDTO parking)
        {
            string rootFolder = Path.Combine(_env.WebRootPath);
            rootFolder = rootFolder + parking.pathPhoto;

            List<string> imagePaths = GetImagePaths(rootFolder, parking.estateObjectId);

            var viewModel = new UpdateParkingViewModel
            {

                parkingId = parking.Id,
                employeeId = parking.employeeId,
                OperationId = parking.operationId,
                Street = parking.Street,
                numberStreet = (int)parking.numberStreet,
                Price = parking.Price,
                currencyId = parking.currencyId,
                Area = parking.Area,
                unitAreaId = parking.unitAreaId,
                Description = parking.Description,
                Path = parking.pathPhoto,
                status = parking.Status,
                estateObjectId = parking.estateObjectId,
                locationId = parking.locationId,
                LocalityId = (int)parking.LocalityId,
                RegionId = (int)parking.RegionId,
                countryId = parking.countryId,
                DistrictId = (int)parking.DistrictId,
                clientId = parking.clientId,
                Date = parking.Date,
                countViews = parking.countViews,
                photos = imagePaths,
                Name = parking.clientName,
                Phone1 = parking.clientPhone

            };
            return viewModel;
        }
        public UpdateStorageViewModel SelectStorage(StorageDTO storage)
        {
            string rootFolder = Path.Combine(_env.WebRootPath);
            rootFolder = rootFolder + storage.pathPhoto;

            List<string> imagePaths = GetImagePaths(rootFolder, storage.estateObjectId);

            var viewModel = new UpdateStorageViewModel
            {

                storageId = storage.Id,
                employeeId = storage.employeeId,
                OperationId = storage.operationId,
                Street = storage.Street,
                numberStreet = (int)storage.numberStreet,
                Price = storage.Price,
                currencyId = storage.currencyId,
                Area = storage.Area,
                unitAreaId = storage.unitAreaId,
                Description = storage.Description,
                Path = storage.pathPhoto,
                status = storage.Status,
                estateObjectId = storage.estateObjectId,
                locationId = storage.locationId,
                LocalityId = (int)storage.LocalityId,
                RegionId = (int)storage.RegionId,
                countryId = storage.countryId,
                DistrictId = (int)storage.DistrictId,
                clientId = storage.clientId,
                Date = storage.Date,
                countViews = storage.countViews,
                photos = imagePaths,
                Name = storage.clientName,
                Phone1 = storage.clientPhone

            };
            return viewModel;
        }
        public static List<string> GetImagePaths(string rootFolder, int objectId)
        {
           
            string[] imageExtensions = { "*.jpg", "*.jpeg", "*.png", "*.gif", "*.bmp", "*.webp" };

            List<string> imagePaths = new List<string>();

            foreach (var extension in imageExtensions)
            {
               
                if (Directory.Exists(rootFolder)) 
                {
                    string[] files = Directory.GetFiles(rootFolder, extension, SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {

                        string fileName = Path.GetFileName(file);
                        string finalPath = $"/images/{objectId}/{fileName}";

                        imagePaths.Add(finalPath);
                    }
                }
            }
            
            return imagePaths;
        }

        #endregion

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

        public async Task<IActionResult> Search(HomePageViewModel homePageViewModel, int page = 1)
        {
            int employeeId = (int)HttpContext.Session.GetInt32("Id");
            int opTypeId = homePageViewModel.operationTypeId;
            int localityId = homePageViewModel.localityId;
            int estateTypeId = homePageViewModel.estateTypeId;
            int minPrice = homePageViewModel.minPrice;
            int maxPrice = homePageViewModel.maxPrice;
            double minArea = homePageViewModel.minArea;
            double maxArea = homePageViewModel.maxArea;

            if (opTypeId != 0 || localityId != 0 || estateTypeId != 0 ||
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

            var filtredEstateObjects = await _objectService.GetFilteredEstateObjects(estateTypeId, opTypeId, localityId, minPrice, maxPrice, minArea, maxArea);
            filtredEstateObjects = filtredEstateObjects.Where(e => e.employeeId == employeeId).ToList();

            ViewBag.OperatrionsList = new SelectList(await _operationService.GetAll(), "Id", "Name");
            ViewBag.LocalitiesList = new SelectList(await _localityService.GetLocalities(), "Id", "Name");

            if (opTypeId != 0 || localityId != 0 || estateTypeId != 0 ||
                minPrice != 0 || maxPrice != 0 || minArea != 0 || maxArea != 0)
            {

                return View("Index", ShowObjectsWithPagination(filtredEstateObjects, operations, areas, currencies, localities, locations, page));
            }
            else
            {
                if (opTypeIdSession == null && localityIdSession == null &&
                    estateTypeIdSession == null && minPriceSession == null &&
                    maxPriceSession == null && minAreaSession == null && maxAreaSession == null)
                {
                    return View("Index", ShowObjectsWithPagination(filtredEstateObjects, operations, areas, currencies, localities, locations, page));

                }
                else
                {
                    var filtredEstateObjectsFromSession = await _objectService.GetFilteredEstateObjects(
                    estateTypeIdSession, opTypeIdSession, localityIdSession, minPriceSession, maxPriceSession, minAreaSession, maxAreaSession);

                    return View("Index", ShowObjectsWithPagination(filtredEstateObjectsFromSession, operations, areas, currencies, localities, locations, page));
                }


            }

        }

        public async Task<IActionResult> SearchForAdmin(HomePageViewModel homePageViewModel, int page = 1)
        {
            int employeeId = homePageViewModel.employeeId;
            string status = homePageViewModel.status;
            int opTypeId = homePageViewModel.operationTypeId;
            int localityId = homePageViewModel.localityId;
            int estateTypeId = homePageViewModel.estateTypeId;
            int minPrice = homePageViewModel.minPrice;
            int maxPrice = homePageViewModel.maxPrice;
            double minArea = homePageViewModel.minArea;
            double maxArea = homePageViewModel.maxArea;

            if (opTypeId != 0 || localityId != 0 || estateTypeId != 0 ||
                minPrice != 0 || maxPrice != 0 || minArea != 0 || maxArea != 0 || employeeId != 0 ||
                status != null)
            {
                HttpContext.Session.SetInt32("opTypeId", opTypeId);
                HttpContext.Session.SetInt32("localityId", localityId);
                HttpContext.Session.SetInt32("estateTypeId", estateTypeId);
                HttpContext.Session.SetInt32("minPrice", minPrice);
                HttpContext.Session.SetInt32("maxPrice", maxPrice);
                HttpContext.Session.SetInt32("employeeId", employeeId);
                HttpContext.Session.SetString("status", status.ToString());
                HttpContext.Session.SetString("minArea", minArea.ToString("R"));
                HttpContext.Session.SetString("maxArea", maxArea.ToString("R"));
            }


            var opTypeIdSession = HttpContext.Session.GetInt32("opTypeId");
            var localityIdSession = HttpContext.Session.GetInt32("localityId");
            var estateTypeIdSession = HttpContext.Session.GetInt32("estateTypeId");
            var minPriceSession = HttpContext.Session.GetInt32("minPrice");
            var maxPriceSession = HttpContext.Session.GetInt32("maxPrice");
            var employeeIdSession = HttpContext.Session.GetInt32("employeeId");
            var statusSession = HttpContext.Session.GetString("status");


            var minAreaSessionStr = HttpContext.Session.GetString("minArea");
            var maxAreaSessionStr = HttpContext.Session.GetString("maxArea");

            double? minAreaSession = double.TryParse(minAreaSessionStr, out _) ? minArea : (double?)null;
            double? maxAreaSession = double.TryParse(maxAreaSessionStr, out _) ? maxArea : (double?)null;



            IEnumerable<OperationDTO> operations = await _operationService.GetAll();
            IEnumerable<AreaDTO> areas = await _areaService.GetAll();
            IEnumerable<CurrencyDTO> currencies = await _currencyService.GetAll();
            IEnumerable<LocationDTO> locations = await _locationService.GetLocations();
            IEnumerable<LocalityDTO> localities = await _localityService.GetLocalities();

            
            var filtredEstateObjects = await _objectService.GetFilteredEstateObjectsForAdmin(estateTypeId, opTypeId, localityId, minPrice, maxPrice, minArea, maxArea, employeeId);
            if(status != "0")
            {
                if(status == "True")
                {
                    filtredEstateObjects = filtredEstateObjects.Where(o => o.Status == true);

                }
                else
                {
                    filtredEstateObjects = filtredEstateObjects.Where(o => o.Status == false);
                }
                
            }
            

            ViewBag.OperatrionsList = new SelectList(await _operationService.GetAll(), "Id", "Name");
            ViewBag.LocalitiesList = new SelectList(await _localityService.GetLocalities(), "Id", "Name");

            if (opTypeId != 0 || localityId != 0 || estateTypeId != 0 ||
                minPrice != 0 || maxPrice != 0 || minArea != 0 || maxArea != 0 || employeeId != 0 || status != "0")
            {

                return View("Index", ShowObjectsWithPagination(filtredEstateObjects, operations, areas, currencies, localities, locations, page));
            }
            else
            {
                if (opTypeIdSession == null && localityIdSession == null &&
                    estateTypeIdSession == null && minPriceSession == null &&
                    maxPriceSession == null && minAreaSession == null && maxAreaSession == null && employeeIdSession == null && statusSession == "0")
                {
                    return View("Index", ShowObjectsWithPagination(filtredEstateObjects, operations, areas, currencies, localities, locations, page));

                }
                else
                {
                    var filtredEstateObjectsFromSession = await _objectService.GetFilteredEstateObjectsForAdmin(
                    estateTypeIdSession, opTypeIdSession, localityIdSession, minPriceSession, maxPriceSession, minAreaSession, maxAreaSession, employeeIdSession);
                    if (statusSession != "0")
                    {
                        if (statusSession == "True")
                        {
                            filtredEstateObjects = filtredEstateObjects.Where(o => o.Status == true);

                        }
                        else
                        {
                            filtredEstateObjects = filtredEstateObjects.Where(o => o.Status == false);
                        }

                    }


                    return View("Index", ShowObjectsWithPagination(filtredEstateObjectsFromSession, operations, areas, currencies, localities, locations, page));
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
            ObjectPageViewModel objectPageViewModel = new ObjectPageViewModel(items, pageViewModel);
            return objectPageViewModel;
        }

    }
}
