using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organizer.Business;
using Organizer.Models;

namespace Organizer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class adminController : Controller
    {
        private ModelContext context;
        private UnitOfWork unitWork;
        private List<User> recordsUser;
        private PagedList<User> pagedList;

        public adminController(ModelContext context)
        {
            this.context = context;
            unitWork = new UnitOfWork(context);
        }

        public IActionResult home()
        {
            return View();
        }
        public ActionResult next()
        {
            PagedList<Record>.currentPage++;
            return RedirectToAction("users");
        }

        public ActionResult previous()
        {
            PagedList<Record>.currentPage--;
            return RedirectToAction("users");
        }
        [HttpGet]
        public IActionResult users()
        {
            pagedList = new PagedList<User>(unitWork.Users.GetAll().ToList(), 10);
            recordsUser = pagedList.GetPageObjects();

            return View(recordsUser);
        }
        [HttpGet]
        public IActionResult user_banned(int id)
        {
            User user = unitWork.Users.Get(id);
            user.IsBanned = user.IsBanned == true ? false : true;

            unitWork.Users.Update(user);
            unitWork.SaveChanges();

            return RedirectToAction("users");
        }
    }
}