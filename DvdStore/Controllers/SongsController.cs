using Microsoft.AspNetCore.Mvc;

namespace DvdStore.Controllers
{
    public class SongsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
