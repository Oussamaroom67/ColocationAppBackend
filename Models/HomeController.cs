using Microsoft.AspNetCore.Mvc;

namespace ColocationAppBackend.Models
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
