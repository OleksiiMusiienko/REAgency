using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Ocsp;
using REAgency.BLL.DTO.Persons;
using REAgency.BLL.Interfaces.Persons;
using REAgency.DAL.Entities.Person;
using REAgency.Models;

namespace REAgency.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public int pageSize = 9;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        public async Task <IActionResult> Index(int page = 1)
        {
            if (HttpContext.Session.GetString("IsAdmin") == "True")
            {
                IEnumerable<EmployeeDTO> employees = await _employeeService.GetEmployees();
                var count = employees.Count();
                var items = employees.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

                EmployeesPageViewModel employeesPageViewModel = new EmployeesPageViewModel(items, pageViewModel);
                return View(employeesPageViewModel);
            }

            return View();

        }

        public IActionResult Create()
        {
            return View("AddEmployee");
        }

        
        public async Task<IActionResult> CreateEmployee(IFormFile formFile, EmployeeDTO employee)
        {
            try
            {
                if(employee.Name != null && employee.Phone1 != null && employee.Phone2 != null && employee.Description != null &&
                    employee.DateOfBirth != null && employee.Email != null)
                {
                    EmployeeDTO employeesEmail = await _employeeService.GetEmployeeByEmail(employee.Email);
                    if (employeesEmail != null)
                    {
                        return View("AddEmployee", employee);
                    }
                    if (formFile == null)
                    {
                        ModelState.AddModelError("", "Фото має бути обов'язково");
                        return View("AddEmployee", employee);
                    }

                    var salt = GenerateSalt();
                    var password = GenerateHashPassword(salt, "employee"); //базовый пароль который нужно потом уже сотруднику изменить
                    byte[] avatar = ImageToByteArray(formFile);

                    EmployeeDTO employeeDTO = new EmployeeDTO
                    {
                        Name = employee.Name,
                        Phone1 = employee.Phone1,
                        Phone2 = employee.Phone2,
                        Email = employee.Email,
                        userStatus = true,
                        postId = 2, //обсудить
                        Password = password,
                        Salt = salt,
                        adminStatus = employee.adminStatus,
                        Description = employee.Description,
                        Avatar = avatar,
                        dateReg = DateTime.Now,
                        DateOfBirth = employee.DateOfBirth
                    };

                    await _employeeService.CreateEmployee(employeeDTO);
                    return RedirectToAction("Index", "Office");
                }
                
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            ModelState.AddModelError("", "Всі поля мають бути заповнені");
            return View("AddEmployee");
        }

        public string GenerateSalt()
        {
            byte[] saltbuf = new byte[16];

            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(saltbuf);

            StringBuilder sb = new StringBuilder(16);
            for (int i = 0; i < 16; i++)
                sb.Append(string.Format("{0:X2}", saltbuf[i]));
            string salt = sb.ToString();

            return salt;
        }

        public string GenerateHashPassword(string salt, string password)
        {
            byte[] passwordArray = Encoding.Unicode.GetBytes(salt + password);

            byte[] byteHash = SHA256.HashData(passwordArray);

            StringBuilder hash = new StringBuilder(byteHash.Length);
            for (int i = 0; i < byteHash.Length; i++)
                hash.Append(string.Format("{0:X2}", byteHash[i]));
            return hash.ToString();
        }

        public byte[] ImageToByteArray(IFormFile image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.CopyTo(ms); 
                return ms.ToArray();
            }
        }
        public Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                Image img = Image.FromStream(ms);  
                return img;
            }
        }
        public async Task<IActionResult> CheckEmail([FromBody] string login)
        {

            EmployeeDTO employee = await _employeeService.GetEmployeeByEmail(login);
            if (employee != null)
            {
                return Json(new { success = false, message = "Працівник з таким email вже існує" });
            }
            else
            {
                return Json(new { success = true, message = "" });
            }


        }

        public async Task<IActionResult> Update(int id)
        {
            EmployeeDTO employeeDTO = await _employeeService.GetEmployeeById(id);
            return View("UpdateEmployee", employeeDTO);
        }

        public async Task<IActionResult> UpdateEmployee(EmployeeDTO model, IFormFile formFile)
        {
            if(formFile  != null)
            {
                byte[] avatar = ImageToByteArray(formFile);
                await _employeeService.UpdateEmployeeAvatar(avatar, model.Id);
                
            }
            EmployeeDTO employeeDTO = new EmployeeDTO();
            employeeDTO.Id = model.Id;
            employeeDTO.Name = model.Name;
            employeeDTO.Email = model.Email;
            employeeDTO.Phone1 = model.Phone1;
            employeeDTO.Phone2 = model.Phone2;
            employeeDTO.dateReg = model.dateReg;
            employeeDTO.DateOfBirth = model.DateOfBirth;
            employeeDTO.Description = model.Description;
            employeeDTO.adminStatus = model.adminStatus;
            employeeDTO.userStatus = model.userStatus;
            employeeDTO.postId = model.postId;

            await _employeeService.UpdateEmployee(employeeDTO);



            return RedirectToAction("Index", "Office");
        }

    }
}
