using Microsoft.AspNetCore.Mvc;

namespace ColocationAppBackend.Data
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
