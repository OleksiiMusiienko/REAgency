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
            ClientDTO clientLogin = await _clientService.GetClientByEmail(reg.RegisterEmail);
            if (reg.RegisterEmail == clientLogin.Email)
            {
                ModelState.AddModelError("RegisterEmail", "Користувач із таким логіном існує");
                return View("Index", reg);
            }
            if(reg.RegisterPassword != reg.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Паролі не збігаються");
                return View("Index", reg);
            }

            client.Name = reg.RegisterName;
            client.Email = reg.RegisterEmail;
            client.status = true;
            client.userStatus = true;
            client.operationId = 2;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthViewModel loginModel)
        {
            if(loginModel == null)
            {
                ModelState.AddModelError("", "Щось пішло не так");
                return View("Index", loginModel);
            }

            var user = await _clientService.GetClientByEmail(loginModel.LoginEmail);
            if (user == null)
            {
                ModelState.AddModelError("LoginPassword", "Не правильний логін або пароль");
                return View("Index", loginModel);
            }

            string? salt = user.Salt;
            byte[] password = Encoding.Unicode.GetBytes(salt + loginModel.LoginPassword);

            byte[] byteHash = SHA256.HashData(password);

            StringBuilder hash = new StringBuilder(byteHash.Length);
            for (int i = 0; i < byteHash.Length; i++)
                hash.Append(string.Format("{0:X2}", byteHash[i]));

            if (user.Password != hash.ToString())
            {
                ModelState.AddModelError("LoginPassword", "Не правильний логін або пароль");
                return View("Index", loginModel);
            }

            HttpContext.Session.SetString("Login", user.Email);
            HttpContext.Session.SetInt32("Id", user.Id);

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
