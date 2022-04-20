using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Repository;

namespace MyCardCollection.Controllers
{
    public class DecksController : Controller
    {
        private readonly IDeckRepository _deckRepository;
        public DecksController(IDeckRepository deckRepository)
        {
            _deckRepository = deckRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
