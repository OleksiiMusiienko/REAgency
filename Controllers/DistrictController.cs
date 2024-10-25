using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using REAgency.BLL.DTO.Locations;
using REAgency.BLL.Interfaces.Locations;
using REAgency.Models;

namespace REAgency.Controllers
{
    public class DistrictController : Controller
    {
        private readonly IRegionService _regionService;
        private readonly IDistrictService _districtService;
        public int pageSize = 18;
        public DistrictController(IRegionService regionService, IDistrictService districtService) 
        {
            _regionService = regionService;
            _districtService = districtService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            
            IEnumerable<DistrictDTO> districts = await _districtService.GetDistricts();
            var count = districts.Count();
            var items = districts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

            DistrictsPageViewModel districtsPageViewModel = new DistrictsPageViewModel(items, pageViewModel);
            return View(districtsPageViewModel);
            
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.RegionsList = new SelectList(await _regionService.GetRegions(), "Id", "Name");

            return View("AddDistrict");
        }

        public async Task<IActionResult> CreateDistrict(DistrictDTO model)
        {
            if (HttpContext.Session.GetString("IsAdmin") == "True")
            {
                DistrictDTO districtDTO = new DistrictDTO
                {
                    Name = model.Name,
                    RegionId = model.RegionId,

                };
                await _districtService.CreateDistrict(districtDTO);
                return RedirectToAction("Index", "Office");
            }
            ModelState.AddModelError("", "У Вас немає прав для виконання данної операції");
            return View();
        }

        public async Task<IActionResult> Update(int id)
        {
            var district = await _districtService.GetDistrictById(id);
            return View("UpdateDistrict", district);
        }

        public async Task<IActionResult> UpdateDistrict(DistrictDTO districtDTO)
        {
            if (HttpContext.Session.GetString("IsAdmin") == "True") 
            { 
                DistrictDTO district = new DistrictDTO {Id = districtDTO.Id, Name = districtDTO.Name, RegionId = districtDTO.RegionId };
                await _districtService.UpdateDistrict(district);
                return RedirectToAction("Index", "Office");
            }
            ModelState.AddModelError("", "У Вас немає прав для виконання данної операції");
            return View();
        }

    }
}
