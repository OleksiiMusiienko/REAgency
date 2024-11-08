using Microsoft.AspNetCore.Mvc;
using MimeKit;
using REAgency.BLL.DTO.Persons;
using REAgency.BLL.Interfaces.Persons;
using REAgency.Models;
using REAgency.BLL.Infrastructure;
using Org.BouncyCastle.Ocsp;
using REAgency.BLL.Services.Persons;
using REAgency.DAL.Entities.Person;

namespace REAgency.Controllers
{
    public class AccountController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IEmployeeService _employeeService;
        public AccountController(IClientService clientService, IEmployeeService employeeService)
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
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPassword)
        {
            if (ModelState.IsValid)
            {
                PersonDTO person = new PersonDTO();
                var client = await _clientService.GetClientByEmail(forgotPassword.Email);
                var employee = await _employeeService.GetEmployeeByEmail(forgotPassword.Email);
                if (client.Email == null && employee == null)
                {
                    ModelState.AddModelError("Email", "Користувача немає з данною адресою!");
                    return View("Index", forgotPassword);
                }
                else
                {
                    if (employee != null)
                    {
                        if (employee.Email != null)
                            person = employee;
                    }
                    if (client != null)
                    {
                        if (client.Email != null)
                            person = client;
                    }
                    var message = new MimeMessage();

                    //от кого отправляем и заголовок

                    message.From.Add(new MailboxAddress("АН Городок — відновлення пароля", "babenko.viktoria.v@gmail.com"));

                    //кому отправляем

                    message.To.Add(new MailboxAddress("", forgotPassword.Email));

                    //тема письма

                    message.Subject = "Відновлення пароля!";

                    //тело письма
                    var bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: 'Arial', sans-serif;
                                background-color: #f4f4f4;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                border-radius: 5px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                align-items: center;
                            }}
                            h1 {{
                                color: #E7734F;
                                margin-left: 30px;
                            }}
                            a {{
                                color: white;
                            }}
                            .block {{
                                height: 130px;
                                background-size: cover;
                                width: 100%;
                                display: flex;
                                justify-content: right;
                                margin-right: 10px;
                            }}
                            .title {{
                                padding: 10px;
                                color: #E7734F;
                                font-size: 1.5rem;
                                font-weight: 600;
                            }}
                            .button {{
                                text-decoration: none;
                                color: white;
                                padding: 7px 20px;
                                background-color: #E7734F;
                                margin-left: 30px;
                            }}
                            .textbefore {{
                                padding: 20px 20px 10px 20px;

                            }}
                            .footer {{
                                border: none;
                                background-color: #0166AA;
                                border-radius: 10px;
                                color: white;
                                text-decoration: none;
                                text-align: center;
                                padding: 5px;
                                box-shadow: 0 0 5px -5px green;
                                width: auto;
                                height: auto;
                            }}
                            .footer a {{
                                text-decoration: none;
                                color: white;
                            }}
                            .footcont {{
                                padding-right: 40px;
                            }}
                            .footer:hover{{
                                transition: all 0.1s;
                                box-shadow: 0 0 20px -2px #0166AA;
                                background-color: #0166AA;
                                color: white
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class=""block"">
                                <div class=""title"">
                                <img src=""https://img-resizer.prd.01.eu-west-1.eu.olx.org/img-eu-olxua-production/949446435_1_261x203_rev001.jpg"" width=""125px"" height=""96,9px"" border-radius=""30px"" style=""margin-bottom:-63px""/>
                                </div>
                            </div>

                            <div class=""textbefore"">Привіт, {person.Name} </p>
                            <p>Ви отримали цей лист, тому що попросили відновлення пароля на АН Городок. Якщо ви цього не робили, просто проігноруйте цей лист.</p></div>      
                            <div class=""footer"">
                                 <a class=""footer"" href=""https://localhost:7133/Account/Reset?email={person.Email}"">Відновити пароль</a></div>
                            </div>
                        </div>

                    </body>
                    </html>
                
                    ";

                    message.Body = bodyBuilder.ToMessageBody();
                    using (var client1 = new MailKit.Net.Smtp.SmtpClient())
                    {
                        await client1.ConnectAsync("smtp.gmail.com", 465, true);
                        await client1.AuthenticateAsync("testgorodok2024@gmail.com", "lmcn fbsn gydo jghp");
                        await client1.SendAsync(message);
                        await client1.DisconnectAsync(true);
                    }

                    return RedirectToAction("Index", "Home");

                }

            }
            return View("Index");

        }
        public async Task<IActionResult> Reset(string? email)
        {
            try
            {
               if (email == null)
                {
                    return NotFound();
                }
                ResetPasswordViewModel reset = new ResetPasswordViewModel();
                var client = await _clientService.GetClientByEmail(email);
                if (client.Email != null)
                {
                    reset.id = client.Id;
                    reset.personMail = client.Email;
                    reset.employee = false;
                    return View(reset);
                }
                var employee = await _employeeService.GetEmployeeByEmail(email);
                if (employee.Email != null)
                {
                    reset.id = employee.Id;
                    reset.personMail = employee.Email;
                    reset.employee = true;
                    return View(reset);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reset(ResetPasswordViewModel reset)
        {
            try
            {
                if (reset.Password.Length < 8)
                {
                    ModelState.AddModelError("Password", "Пароль має містити 8 символів");
                    return View("Reset", reset);
                }
                else if (!reset.Password.Any(char.IsDigit))
                {
                    ModelState.AddModelError("Password", "Пароль повинен містити хоча б одну цифру");
                    return View("Reset", reset);
                }
                if (reset.Password != reset.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Паролі не збігаються");
                    return View("Index", reset);
                }
                if (!reset.employee)
                {
                    ClientDTO clientDTO = new ClientDTO
                    {
                        Id = reset.id,
                        Password = reset.Password
                    };
                    await _clientService.UpdateClientPassword(clientDTO);
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    EmployeeDTO employeeDTO = new EmployeeDTO
                    {
                        Id = reset.id,
                        Password = reset.Password
                    };
                    await _employeeService.UpdateEmployeePassword(employeeDTO);
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

}
