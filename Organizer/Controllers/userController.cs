using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Organizer.Business;
using Organizer.Models;
using Organizer.Models.DTO;

namespace Organizer.Controllers
{
    [Authorize(Roles = "User")]
    public class userController : Controller
    {
        private ModelContext context;
        private UnitOfWork unitWork;
        private List<Record> recordsUser;
        private PagedList<Record> pagedList;
        private User user;

        public userController(ModelContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            unitWork = new UnitOfWork(context);
            user = PageInit(httpContextAccessor);
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
        public ActionResult next()
        {
            PagedList<Record>.currentPage++;
            return RedirectToAction("records");
        }

        public ActionResult previous()
        {
            PagedList<Record>.currentPage--;
            return RedirectToAction("records");
        }
        [HttpGet]
        public IActionResult records(string sort)
        {
            if (unitWork.Records.CheckRecordsUser(user.Id))
            {
                pagedList = new PagedList<Record>(unitWork.Records.GetAllByUser(user.Id).ToList(), 5);

                recordsUser = pagedList.GetPageObjects();

                ViewBag.DateSortParm = sort == "date" ? "date_desc" : "date";

                switch (sort)
                {
                    case "date":
                        recordsUser = recordsUser.OrderBy(r => r.CreateTime).ToList();
                        break;
                    case "date_desc":
                        recordsUser = recordsUser.OrderByDescending(r => r.CreateTime).ToList();
                        break;
                }

                return View(recordsUser);
            }

            return View();
        }

        [HttpGet]
        public IActionResult delete_record(int id)
        {
            unitWork.Records.Delete(id);
            unitWork.SaveChanges();

            return RedirectToAction("records");
        }
        [HttpGet]
        public IActionResult show_record(int id)
        {
            Record record = unitWork.Records.Get(id);

            if (record.User != null && record.User.Id == user.Id)
                return View(new RecordDTO { Title = record.Title, Text = record.Text });

            return RedirectToAction("records");
        }
        [HttpGet]
        public IActionResult edit_record(int id)
        {
            Record record = unitWork.Records.Get(id);

            if (record.User != null && record.User.Id == user.Id)
                return View(new RecordDTO
                {
                    Id = record.Id,
                    CreateTime = record.CreateTime,
                    Title = record.Title,
                    Text = record.Text
                });

            return RedirectToAction("records");
        }
        [HttpPost]
        public IActionResult edit_record(RecordDTO record)
        {
            unitWork.Records.Update(new Record
            {
                Id = record.Id,
                Title = record.Title,
                Text = record.Text,
                CreateTime = record.CreateTime,
                User = user
            });

            unitWork.SaveChanges();

            return RedirectToAction("records");
        }
        [HttpGet]
        public IActionResult new_record()
        {
            return View();
        }
        [HttpPost]
        public IActionResult new_record(RecordDTO record)
        {
            unitWork.Records.Create(new Record
            {
                Title = record.Title,
                Text = record.Text,
                CreateTime = DateTime.Now,
                User = user
            });
            unitWork.SaveChanges();

            return RedirectToAction("records");
        }
        [HttpGet]
        public IActionResult settings()
        {
            return View(new SettingsDTO
            {
                Name = user.Name,
                Surname = user.Surname
            });
        }
        [HttpPost]
        public async Task<IActionResult> settings(SettingsDTO settings)
        {
            user.Name = settings.Name == null ? user.Name : settings.Name;
            user.Surname = settings.Surname == null ? user.Surname : settings.Surname;
            user.Password = settings.Password == null ? user.Password : CryptoService.HashingPassword(settings.Password);

            unitWork.Users.Update(user);
            unitWork.SaveChanges();

            await UpdateNameClaim(user);

            TempData["Success"] = "true";
            return RedirectToAction("settings");
        }

        private User PageInit(IHttpContextAccessor httpContextAccessor)
        {
            string login = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email)).Value;
            return unitWork.Users.GetByEmail(login);
        }

        private async Task UpdateNameClaim(User user)
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