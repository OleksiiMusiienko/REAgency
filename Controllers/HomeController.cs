using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using REAgency.BLL.Interfaces;
using REAgency.BLL.Interfaces.Locations;
using REAgency.Models;
using System.Diagnostics;


namespace REAgency.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IOperationService _operationService;
        private readonly IEstateTypeService _estateTypeService;
        private readonly ILocalityService _localityService;
        public HomeController(IOperationService operationService, IEstateTypeService estateTypeService, ILocalityService localityService)
        {
            //_logger = logger;
            _operationService = operationService;
            _estateTypeService = estateTypeService;
            _localityService = localityService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            ViewBag.OperatrionsList = new SelectList(await _operationService.GetAll(), "Id", "Name");

            ViewBag.EsateTypesList = new SelectList(await _estateTypeService.GetAll(), "Id", "Name");
            ViewBag.LocalitiesList = new SelectList(await _localityService.GetLocalities(), "Id", "Name");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
