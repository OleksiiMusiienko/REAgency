using Microsoft.AspNetCore.Mvc;

namespace REAgency.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
