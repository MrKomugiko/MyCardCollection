using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using Newtonsoft.Json;
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
        public async Task<PartialViewResult> Overview(int deckId)
        {
            return PartialView("_DeckOverview", deckId);
        }

        [HttpGet]
        [Route("api/DeckStatistics/{deckId}")]
        public async Task<JsonResult> StatisticsData(int deckId)
        {
            var deck = await _deckRepository.GetDeckById(deckId);

            if (deck == null) return Json(NotFound());

            var manaCountGroup = deck.Content
                .GroupBy(x => x.CardData.CMC)
                .Select(g => new
                {
                    Key = g.Key,
                    Value = g.Count()
                });

            int[] manaCurveArray = new int[15];

            foreach(var group in manaCountGroup)
            {
                manaCurveArray[(Int32)group.Key] = group.Value;
            }

            // Mana curve
            var objectx = new
            {
                id = deckId,
                owner = deck.AppUserId,
                name = deck.Name,
                size = deck.Content.Sum(x=>x.Quantity),
                ManaCurve = manaCurveArray,
                TypeDistribution = new Dictionary<string,int>
                    {
                        {"Artifact",20 },
                        {"Creature",20 },
                        {"Enchantment",20 },
                        {"Instant",20 },
                        {"Sorcery",20 },
                        {"Land",20 },
                    },
                Cards = deck.Content
                    .Select(x => new {
                        x.Quantity,
                        x.CardData
                    })
            };

            return Json(objectx);
        }

    }
}
