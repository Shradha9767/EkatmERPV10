using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace EkatmERPV10.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/api/Home/index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
