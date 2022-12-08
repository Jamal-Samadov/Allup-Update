using Microsoft.AspNetCore.Mvc;

namespace Allup.Areas.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
