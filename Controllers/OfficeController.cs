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

        public async Task<IActionResult> Index()
        {
            if(HttpContext.Session.GetString("User") != "employee")
            {
                return RedirectToAction("Index", "Home");
            }
            IEnumerable<EstateObjectDTO> objects = await _objectService.GetEstateObjectByEmployeeId(HttpContext.Session.GetInt32("Id")!.Value);
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
            }).ToList();

            return View(listObjects);
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

        public async Task<string> AddFoto(EstateObjectDTO estateObjectDTO, IFormFileCollection formFiles)
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
				return pathdirectory;
			}
            return null;
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
                
                estateObjectDTO.localityId = flatViewModel.LocalityId;
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

                string pathFoto = AddFoto(estateObjectDTO, formFiles).Result; //добавляем фото и получаем путь

                estateObjectDTO.pathPhoto = pathFoto; //добавляем путь в обьект

                await _objectService.UpdateEstateObject(estateObjectDTO); //обновляем обьект
                
                //и дальше создаем Flat

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
	}
}
