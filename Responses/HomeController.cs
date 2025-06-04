using Microsoft.AspNetCore.Mvc;

namespace ColocationAppBackend.Responses
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
