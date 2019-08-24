using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Organizer.Business;
using Organizer.Models;
using Organizer.Models.DTO;

namespace Organizer.Controllers
{
    
    public class accountController : Controller
    {
        private ModelContext context;
        private UnitOfWork unitWork;
        
        public accountController(ModelContext context)
        {
            this.context = context;
            unitWork = new UnitOfWork(context);
        }
        
        [HttpGet]
        public IActionResult login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult home()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> login(AuthDTO authUser)
        {
            if (ModelState.IsValid)
            {
                User user = unitWork.Users.UserVerification(authUser.login, authUser.password);
                if (user != null)
                {
                    await Authenticate(user);

                    if (user.IsAdmin)
                    {
                        return RedirectToAction("home", "admin");
                    }
                    return RedirectToAction("home", "user");
                }
            }
            ViewBag.Error = "error";
            return View();
        }
        [HttpGet]
        public IActionResult register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> register(RegDTO regUser)
        {
            if (ModelState.IsValid)
            {
                if (unitWork.Users.GetByEmail(regUser.Email) == null)
                {
                    User user = new User
                    {
                        Name = regUser.Name,
                        Surname = regUser.Surname,
                        Email = regUser.Email,
                        Password = regUser.Password,
                        IsAdmin = true
                    };

                    unitWork.Users.Create(user);
                    unitWork.SaveChanges();

                    await Authenticate(user);

                    if (user.IsAdmin)
                    {
                        return RedirectToAction("home", "admin");
                    }
                    return RedirectToAction("home", "user");
                }
            }
            return View();
        }
        [HttpGet]
        public IActionResult forgot()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> forgot(string email)
        {
            User user = unitWork.Users.GetByEmail(email);
            if (user != null)
            {
                string culture = GetUserCultureFromCookie();
                string code = CryptoService.GetRandomPassword();

                if (culture == "en")
                {
                    await EmailService.SendEmailAsync(email, "New password in diary.com", "Password - " + code);
                }
                else if (culture == "ru")
                {
                    await EmailService.SendEmailAsync(email, "Новый пароль в diary.com", "Пароль - " + code);
                }
                else
                {
                    await EmailService.SendEmailAsync(email, "Новий пароль для diary.com", "Пароль - " + code);
                }

                user.Password = CryptoService.HashingPassword(code);

                unitWork.Users.Update(user);
                unitWork.SaveChanges();

                TempData["NewPass"] = "password";
                return RedirectToAction("login", "account");
            }
            ViewBag.Error = "error";
            return View();
        }
        [Authorize]
        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("login", "account");
        }
        [HttpGet]
        public IActionResult helper()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SetLanguage(string language, string returnUrl)
        {
            SetUserCulture(language);

            return LocalRedirect(returnUrl);
        }

        public void SetUserCulture(string lang)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
        }
        private string GetUserCultureFromCookie()
        {
            string culture;
            if (Request.Cookies.TryGetValue(".AspNetCore.Culture", out culture))
            {
                var uk = culture.IndexOf("uk");
                var en = culture.IndexOf("en");
                var ru = culture.IndexOf("ru");

                if (en >= 0)
                {
                    return "en";
                }
                else if (ru >= 0)
                {
                    return "ru";
                }
            }
            return "uk";
        }
        private async Task Authenticate(User user)
        {
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