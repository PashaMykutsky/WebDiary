using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organizer.Business;
using Organizer.Models;
using Organizer.Models.DTO;

namespace Organizer.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class userController : Controller
    {
        private ModelContext context;
        private UnitOfWork unitWork;

        public userController(ModelContext context)
        {
            this.context = context;
            unitWork = new UnitOfWork(context);
        }
        
        [HttpGet]
        public IActionResult home()
        {
            return View();
        }
        [HttpGet]
        public IActionResult about()
        {
            return View();
        }
        [HttpGet]
        public IActionResult records()
        {
            return View();
        }
        [HttpGet]
        public IActionResult settings()
        {
            User user = PageInit();
            return View(new SettingsDTO {
                Name = user.Name,
                Surname = user.Surname
            });
        }
        [HttpPost]
        public IActionResult settings(SettingsDTO settings)
        {
            User user = PageInit();
            user.Name = settings.Name == null ? user.Name : settings.Name;
            user.Surname = settings.Surname == null ? user.Surname : settings.Surname;
            user.Password = settings.Password == null ? user.Password : CryptoService.HashingPassword(settings.Password);

            unitWork.Users.Update(user);
            unitWork.SaveChanges();

            ViewBag.Success = "true";
            return View();
        }

        private User PageInit()
        {
            string login = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email)).Value;
            return unitWork.Users.GetByEmail(login);
        }
    }
}