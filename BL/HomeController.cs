using Microsoft.AspNetCore.Mvc;

namespace ColocationAppBackend.BL
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
