﻿using Microsoft.AspNetCore.Mvc;
using REAgency.BLL.DTO;
using REAgency.BLL.DTO.Object;
using REAgency.BLL.Interfaces;
using REAgency.BLL.Interfaces.Object;
using REAgency.Models;
using System.Data;
using REAgencyEnum;
using Microsoft.AspNetCore.Mvc.Rendering;
using REAgency.BLL.Interfaces.Locations;
using REAgency.BLL.Interfaces.Persons;
using REAgency.BLL.DTO.Persons;
using Org.BouncyCastle.Utilities;
using REAgency.BLL.DTO.Locations;
using REAgency.BLL.Services.Locations;
using REAgency.Models.Flat;

namespace REAgency.Controllers
{
    public class OfficeController : Controller
    {
        private readonly IEstateObjectService _objectService;
        private readonly IOperationService _operationService;
        private readonly IRegionService _regionService;
        private readonly IDistrictService _districtService;
        private readonly ILocalityService _localityService;
        private readonly ICurrencyService _currencyService;
        private readonly IClientService _clientService;
        private readonly IEmployeeService _employeeService;
        private readonly IFlatService _flatService;
        private readonly IEstateObjectService _estateObjectService;
        private readonly ILocationService _locationService;
        private readonly IAreaService _areaService;
        
        public OfficeController(IEstateObjectService objectService, IOperationService operationService, IRegionService regionService, 
            IDistrictService districtService, ILocalityService localityService, ICurrencyService currencyService, IClientService clientService,
            IEmployeeService employeeService, IFlatService flatService, IEstateObjectService estateObjectService, ILocationService locationService,
            IAreaService areaService)
        {
            _objectService = objectService;
            _operationService = operationService;
            _regionService = regionService;
            _districtService = districtService;
            _localityService = localityService;
            _currencyService = currencyService; 
            _clientService = clientService;
            _employeeService = employeeService;
            _flatService = flatService;
            _estateObjectService = estateObjectService;
            _locationService = locationService;
            _areaService = areaService;
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

        //public async Task<IActionResult> AddFoto(IFormFile[] uploadedFiles)
        //{
        //    if (uploadedFiles != null)
        //    {

        //    }
        //}

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
        public async Task CreateFlat(AddFlatViewModel flatViewModel, IFormFileCollection formFiles)
        {
            //сохранение фото в базу, формировать путь, создавать папку, проверять формат и размер, редактировать
            //фото, загружать в папку(создавать папку с именем == iDобьекта)

            //Последовательность подачи обьекта
            //1.Создать клиента
            //2.Создать локацию
            //3.

            if (ModelState.IsValid && formFiles!=null)
            {               
                EstateObjectDTO estateObjectDTO = new EstateObjectDTO();            

                ClientDTO clientDTO = await CreateClient(flatViewModel.Name, flatViewModel.Phone1);                

                estateObjectDTO.clientId = clientDTO.Id;

                estateObjectDTO.employeeId = (int)HttpContext.Session.GetInt32("Id"); 

                estateObjectDTO.operationId = flatViewModel.OperationId;                

              FlatDTO flatDTO= new FlatDTO(); 
            }
        }
        private async Task<ClientDTO> CreateClient(string clientName, string clientPhone)
        {
                ClientDTO clientDTO = new ClientDTO();
                clientDTO.Phone1 = clientPhone;
                clientDTO.Name = clientName;
                clientDTO.employeeId = HttpContext.Session.GetInt32("Id");
                OperationDTO operationDTO = await _operationService.GetByName("Продаж");
                clientDTO.operationId = operationDTO.Id;
                clientDTO.status = true;
                await _clientService.CreateClient(clientDTO);
                clientDTO = await _clientService.GetByPhone(clientPhone);
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
                locationDTO.CountryId = model.countryId;
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


                await _estateObjectService.UpdateEstateObject(objectDTO);
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
                LocalityId = flat.LocalityId,
                RegionId = flat.RegionId,
                countryId = flat.countryId,
                DistrictId = flat.DistrictId,
                clientId = flat.clientId,
                Date = flat.Date,
                countViews = flat.countViews
               



            };
            return viewModel;
        }


    }
}
