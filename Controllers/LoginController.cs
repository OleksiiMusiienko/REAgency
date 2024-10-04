using Microsoft.AspNetCore.Mvc;
using REAgency.BLL.DTO.Persons;
using REAgency.BLL.Interfaces.Persons;
using REAgency.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace REAgency.Controllers
{
    public class LoginController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IEmployeeService _employeeService;

        public LoginController(IClientService clientService, IEmployeeService employeeService)
        {
            _clientService = clientService;
            _employeeService = employeeService;

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
            else if(reg.RegisterPassword != reg.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Паролі не збігаються");
                return View("Index", reg);
            }
            else if (!Regex.IsMatch(reg.RegisterPhone, @"^\d+$"))
            {
                ModelState.AddModelError("RegisterPhone", "Допустимі лише цифри");
                return View("Index", reg);
            }
            else if (!Regex.IsMatch(reg.RegisterPhone, @"^0\d{9}$"))
            {
                ModelState.AddModelError("RegisterPhone", "Номер телефону має містити рівно 10 цифр і починатися з 0.");
                return View("Index", reg);
            }
            else if (reg.confirmPersonalData != true)
            {
                ModelState.AddModelError("confirmPersonalData", "Це обов'язкове поле");
                return View("Index", reg);
            }

            client.Name = reg.RegisterName;
            client.Email = reg.RegisterEmail;
            client.status = false; // rigth one
            client.userStatus = true; // right one
            client.Phone1 = reg.RegisterPhone;


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
            HttpContext.Session.SetString("Name", client.Name);
            HttpContext.Session.SetInt32("Id", client.Id);

            return RedirectToAction("Index", "Home");
           

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthViewModel loginModel)
        {
            if(loginModel.LoginEmail == null || loginModel.LoginPassword == null)
            {
                ModelState.AddModelError("", "Введіть всі поля");
                return View("Index", loginModel);
            }

            var client = await _clientService.GetClientByEmail(loginModel.LoginEmail);
            var employee = await _employeeService.GetEmployeeByEmail(loginModel.LoginEmail);
            if(employee == null)
            {
                //Логика входа клиента
                if (client == null)
                {
                    ModelState.AddModelError("LoginPassword", "Не правильний логін");
                    return View("Index", loginModel);
                } 
                else
                {
                    if (client.Password != Decryption(client.Salt, loginModel.LoginPassword)) 
                    {
                        ModelState.AddModelError("LoginPassword", "Неправильний логін або пароль");
                        return View("Index", loginModel);
                    }
                    else
                    {
                        HttpContext.Session.SetString("Login", client.Email);                        
                        HttpContext.Session.SetString("Name", client.Name);                       
                        HttpContext.Session.SetInt32("Id", client.Id);
                        HttpContext.Session.SetString("User", "user");
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else // если не null значит программа обнаружила что входит сотрудник
            {
                if (employee.Password != Decryption(employee.Salt, loginModel.LoginPassword)) 
                {
                    ModelState.AddModelError("LoginPassword", "Неправильний логін або пароль");
                    return View("Index", loginModel);
                }
                else
                {

                    HttpContext.Session.SetString("Login", employee.Email);
                    HttpContext.Session.SetString("Name", employee.Name);                   
                    HttpContext.Session.SetInt32("Id", employee.Id);
                   
                    HttpContext.Session.SetString("User", "employee");
                    if(employee.adminStatus == true)
                    {
                        HttpContext.Session.SetString("IsAdmin", "True");
                    }
                    return RedirectToAction("Index", "Home");
                }
            } 

             
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

        public string Decryption(string salt,  string enteredPassword) {
           
            byte[] password = Encoding.Unicode.GetBytes(salt + enteredPassword);

            byte[] byteHash = SHA256.HashData(password);

            StringBuilder hash = new StringBuilder(byteHash.Length);
            for (int i = 0; i < byteHash.Length; i++)
                hash.Append(string.Format("{0:X2}", byteHash[i]));
            return hash.ToString();

        }

    }
}
