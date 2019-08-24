using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        public async Task<IActionResult> settings(SettingsDTO settings)
        {
            User user = PageInit();
            user.Name = settings.Name == null ? user.Name : settings.Name;
            user.Surname = settings.Surname == null ? user.Surname : settings.Surname;
            user.Password = settings.Password == null ? user.Password : CryptoService.HashingPassword(settings.Password);

            unitWork.Users.Update(user);
            unitWork.SaveChanges();

            await UpdateNameClaims(user);

            TempData["Success"] = "true";
            return RedirectToAction("settings");
        }

        private User PageInit()
        {
            string login = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email)).Value;
            return unitWork.Users.GetByEmail(login);
        }

        private async Task UpdateNameClaims(User user)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.IsAdmin == true ? "Admin" : "User"),
                new Claim(ClaimTypes.Name, user.Name+"  "+user.Surname)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}