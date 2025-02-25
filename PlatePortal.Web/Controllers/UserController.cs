using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlatePortal.Domain.DomainModels;
using PlatePortal.Domain.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PlatePortal.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<PlatePortalApplicationUser> userManager;
        public UserController(UserManager<PlatePortalApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            List<PlatePortalApplicationUser> users = userManager.Users.ToList();
            return View(users);
        }

        public IActionResult ImportUsers(IFormFile file)
        {
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }

            List<UserRegistrationDto> users = getAllUsersFromFile(file.FileName);

            foreach (var item in users)
            {
                var userCheck = userManager.FindByEmailAsync(item.Email).Result;
                if (userCheck == null)
                {
                    var user = new PlatePortalApplicationUser
                    {
                        FirstName = item.Name,
                        LastName = item.LastName,
                        UserName = item.Email,
                        NormalizedUserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        PhoneNumber = item.PhoneNumber,
                        Address=item.Address,
                        UserCart = new ShoppingCart()
                    };
                    var result = userManager.CreateAsync(user, item.Password).Result;

                }
                else
                {
                    continue;
                }
            }
            return RedirectToAction("Index", "User");
        }

        private List<UserRegistrationDto> getAllUsersFromFile(string fileName)
        {
            List<UserRegistrationDto> users = new List<UserRegistrationDto>();
            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        users.Add(new UserRegistrationDto
                        {
                            Email = reader.GetValue(0).ToString(),
                            Name = reader.GetValue(1).ToString(),
                            LastName = reader.GetValue(2).ToString(),
                            PhoneNumber = reader.GetValue(3).ToString(),
                            Address = reader.GetValue(4).ToString(),
                            Password = reader.GetValue(5).ToString(),
                            ConfirmPassword = reader.GetValue(6).ToString()
                        });
                    }
                }
            }
            return users;
        }
    }
}
