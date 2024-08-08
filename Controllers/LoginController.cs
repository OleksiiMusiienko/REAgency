using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using REAgency.BLL.DTO.Persons;
using REAgency.BLL.Interfaces.Persons;
using REAgency.BLL.Services;
using REAgency.BLL.Services.Persons;
using REAgency.Models;

namespace REAgency.Controllers
{
    public class LoginController : Controller
    {
        private readonly IClientService _clientService;

        public LoginController(IClientService clientService)
        {
            _clientService = clientService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AuthViewModel reg)
        {
            ClientDTO client = new ClientDTO();
            if (reg.RegisterPassword.Length < 8)
            {
                ModelState.AddModelError("Password", "Пароль має містити 8 символів");
                return View("Index", reg);
            }
            else if (!reg.RegisterPassword.Any(char.IsDigit))
            {
                ModelState.AddModelError("Password", "Пароль повинен містити хоча б одну цифру");
                return View("Index", reg);
            }
            client.Name = reg.RegisterName;
            client.Email = reg.RegisterEmail;
            client.status = true;
            client.userStatus = true;
            client.operationId = 5;

            client.Phone1 = "099655244";


            byte[] saltbuf = new byte[16];

            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(saltbuf);

            StringBuilder sb = new StringBuilder(16);
            for (int i = 0; i < 16; i++)
                sb.Append(string.Format("{0:X2}", saltbuf[i]));
            string salt = sb.ToString();


            byte[] password = Encoding.Unicode.GetBytes(salt + reg.RegisterPassword);

            byte[] byteHash = SHA256.HashData(password);

            StringBuilder hash = new StringBuilder(byteHash.Length);
            for (int i = 0; i < byteHash.Length; i++)
                hash.Append(string.Format("{0:X2}", byteHash[i]));

            client.Password = hash.ToString();
            client.Salt = salt;
            await _clientService.CreateClient(client);

            HttpContext.Session.SetString("Email", client.Email);
            HttpContext.Session.SetInt32("Id", client.Id);

            return RedirectToAction("Index", "Home");
           

        }

        [HttpPost]
        public async Task<IActionResult> CheckEmail([FromBody] string login)
        {
           
            ClientDTO client = await _clientService.GetClientByEmail(login);
            if (login == client.Email)
            {
                return Json(new { success = false, message = "Email exists" });
            }
            else
            {
                return Json(new { success = true, message = "" });
            }

            
        }

    }
}
