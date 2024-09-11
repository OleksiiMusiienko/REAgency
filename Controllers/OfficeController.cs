using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using REAgency.BLL.DTO;
using REAgency.BLL.DTO.Object;
using REAgency.BLL.Interfaces;
using REAgency.BLL.Interfaces.Object;
using REAgency.BLL.Services.Objects;
using REAgency.Models;

namespace REAgency.Controllers
{
    public class OfficeController : Controller
    {
        private readonly IEstateObjectService _objectService;
        private readonly IOperationService _operationService;
        private List<ObjectsViewModel> listObjects;
        public OfficeController(IEstateObjectService objectService, IOperationService operationService)
        {
            _objectService = objectService;
            _operationService = operationService;
            listObjects = new List<ObjectsViewModel>();
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<EstateObjectDTO> objects = await _objectService.GetAllEstateObjects();

            IEnumerable<OperationDTO> operations = await _operationService.GetAll();

            listObjects = objects.Select(estateObject => new ObjectsViewModel
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
        public async Task<IActionResult> MyObjects()
        {
            IEnumerable<EstateObjectDTO> objects = await _objectService.GetAllEstateObjects();
            IEnumerable<OperationDTO> operations = await _operationService.GetAll();

            listObjects = objects.Select(estateObject => new ObjectsViewModel
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
                if(ovm.type == DAL.Entities.Object.EstateObject.ObjectType.Flat)
                {
                    _objectService?.CreateEstateObject(CreateFlat(ovm));                    
                }

            }
            return RedirectToAction("Index");
        }

        //metods create estate object
        public EstateObjectDTO CreateFlat(ObjectsViewModel ovm)
        {
            FlatDTO flatDTO= new FlatDTO();
            //var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ObjectsViewModel, EstateObjectDTO>()).CreateMapper(); //создаем обьект и говорим что на что мы мапим
            //return mapper.Map(ovm, flatDTO);          

            return flatDTO;
        }





    }
}
