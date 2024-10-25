using System.Drawing.Printing;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using REAgency.BLL.DTO.Locations;
using REAgency.BLL.Interfaces.Locations;
using REAgency.BLL.Services.Locations;
using REAgency.DAL.Entities.Person;
using REAgency.Models;

namespace REAgency.Controllers
{
    public class LocalityController : Controller
    {
        private readonly ILocalityService _localityService;
        private readonly IDistrictService _districtService;
        private readonly IRegionService _regionService;

        public int pageSize = 18;
        public LocalityController(ILocalityService localityService, IDistrictService districtService, IRegionService regionService)
        {
            _localityService = localityService;
            _districtService = districtService;
            _regionService = regionService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {

            IEnumerable<LocalityDTO> locality = await _localityService.GetLocalities();
            var count = locality.Count();
            var items = locality.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

            LocalitiesPageViewModel localitiesPageViewModel = new LocalitiesPageViewModel(items, pageViewModel);
            return View(localitiesPageViewModel);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.DistrictsList = new SelectList(await _districtService.GetDistricts(), "Id", "Name");
            return View("AddLocality");

        }
        public async Task<IActionResult> CreateLocality(LocalityDTO model)
        {
            if (HttpContext.Session.GetString("IsAdmin") == "True")
            {
                LocalityDTO localityDTO = new LocalityDTO
                {
                    Name = model.Name,
                    DistrictId = model.DistrictId
                };
                await _localityService.CreateLocality(localityDTO);
                return RedirectToAction("Index", "Office");
            }
            ModelState.AddModelError("", "У Вас немає прав для виконання данної операції");
            return View();
        }

        public async Task<IActionResult> Update(int id)
        {
            var locality = await _localityService.GetLocalityById(id);
            return View("UpdateLocality", locality);
        }

        public async Task<IActionResult> UpdateLocality(LocalityDTO model)
        {
            if (HttpContext.Session.GetString("IsAdmin") == "True")
            {
                LocalityDTO localityDTO = new LocalityDTO { Id = model.Id, Name = model.Name, DistrictId = model.DistrictId };
                await _localityService.UpdateLocality(localityDTO);
                return RedirectToAction("Index", "Office");
            }
            ModelState.AddModelError("", "У Вас немає прав для виконання данної операції");
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ShowRegionOfDistrict([FromBody] int districtId)
        {
            IEnumerable<DistrictDTO> districts = await _districtService.GetDistricts();
            DistrictDTO districtDTO = districts.Where(r => r.Id == districtId).First();
            if (districtDTO != null)
            {
                return Json(new { success = false, message = districtDTO.regionName });
            }
            else
            {
                return Json(new { success = true, message = "" });
            }

        }
    }
}