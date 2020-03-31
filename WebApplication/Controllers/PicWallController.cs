using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    public class PicWallController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}