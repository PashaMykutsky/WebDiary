using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Organizer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class adminController : Controller
    {
        public IActionResult home()
        {
            return View();
        }
    }
}