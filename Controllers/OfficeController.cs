using Microsoft.AspNetCore.Mvc;
using REAgency.BLL.DTO;
using REAgency.BLL.DTO.Object;
using REAgency.BLL.Interfaces;
using REAgency.BLL.Interfaces.Object;
using REAgency.Models;
using System.Data;
using REAgencyEnum;

namespace REAgency.Controllers
{
    public class OfficeController : Controller
    {
        private readonly IEstateObjectService _objectService;
        private readonly IOperationService _operationService;
        public OfficeController(IEstateObjectService objectService, IOperationService operationService)
        {
            _objectService = objectService;
            _operationService = operationService;
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
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: estateObject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ObjectsViewModel ovm)
        {

            if (ModelState.IsValid)
            {
                if(ovm.objectType == ObjectType.Flat)
                {
                     _objectService?.CreateEstateObject(CreateFlat(ovm));                    
                }

            }
            return RedirectToAction("Index");
        }

        //metods create estate object
        public EstateObjectDTO CreateFlat(ObjectsViewModel ovm)
        {
            //сохранение фото в базу, формировать путь, создавать папку, проверять формат и размер, редактировать
            //фото, загружать в папку(создавать папку с именем == iDобьекта)
            FlatDTO flatDTO= new FlatDTO();  
            

            return flatDTO;
        }





    }
}
