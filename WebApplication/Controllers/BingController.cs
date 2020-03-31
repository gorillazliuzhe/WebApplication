using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication.Business;

namespace WebApplication.Controllers
{
    public class BingController : Controller
    {
        private readonly IBusiness _metro;

        public BingController(IBusiness metro)
        {
            _metro = metro;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _metro.BingWalles();
            return View(data);
        }
    }
}