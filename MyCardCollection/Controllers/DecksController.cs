using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using System.Security.Claims;

namespace MyCardCollection.Controllers
{
    public class DecksController : Controller
    {
        private readonly IDeckRepository _deckRepository;
        public DecksController(IDeckRepository deckRepository)
        {
            _deckRepository = deckRepository;
        }
        private string _userId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<IActionResult> Index()
        {
            List<Deck> userDecks = await _deckRepository.GetUserDecks(_userId);
            return View(userDecks);
        }
    }
}
