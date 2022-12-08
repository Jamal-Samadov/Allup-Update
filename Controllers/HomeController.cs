using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Allup.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
     
    }
}