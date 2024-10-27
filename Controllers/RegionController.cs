using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc;
using REAgency.BLL.DTO.Locations;
using REAgency.BLL.DTO.Persons;
using REAgency.BLL.Interfaces.Locations;
using REAgency.Models;

namespace REAgency.Controllers
{
    public class RegionController : Controller
    {
        private readonly IRegionService _regionService;
       
        public RegionController(IRegionService regionService) 
        {
            _regionService = regionService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("IsAdmin") == "True")
            {
                IEnumerable<RegionDTO> redions = await _regionService.GetRegions();
                return View(redions);
            }
            return View();
        }

        public IActionResult Create()
        {
            return View("AddRegion");
        }

        public async Task<IActionResult> CreateRegion(RegionDTO model)
        {
            if (HttpContext.Session.GetString("IsAdmin") == "True")
            {
                RegionDTO regionDTO = new RegionDTO
                {
                    Name = model.Name,
                    CountryId = 1
                };
                await _regionService.CreateRegion(regionDTO);
                return RedirectToAction("Index", "Office");
            }
            ModelState.AddModelError("", "У Вас немає прав для виконання данної операції");
            return View();
        }

        public async Task<IActionResult> Update(int id)
        {
            var region = await _regionService.GetRegionById(id);
            return View("UpdateRegion", region);
        }

        public async Task<IActionResult> UpdateRegion(RegionDTO model)
        {
            if (HttpContext.Session.GetString("IsAdmin") == "True")
            {
                RegionDTO regionDTO = new RegionDTO
                {
                    Id = model.Id,
                    Name = model.Name,
                    CountryId = model.CountryId
                };
                await _regionService.UpdateRegion(regionDTO);
                return RedirectToAction("Index", "Office");
            }
            ModelState.AddModelError("", "У Вас немає прав для виконання данної операції");
            return View();
        }
    }
}
