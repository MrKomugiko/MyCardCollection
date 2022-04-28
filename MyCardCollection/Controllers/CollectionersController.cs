using Microsoft.AspNetCore.Mvc;

namespace MyCardCollection.Controllers
{
    public class CollectionersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
