using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using MyCardCollection.ViewModel;
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
            var deck = await _deckRepository.GetDeckById(deckId);
            if (deck == null) return PartialView("_DeckOverview", null);
            if (! deck.Content.Any()) return PartialView("_DeckOverview", null);


            var typeCountGroup = deck.Content
                .GroupBy(x => x.CardData.Type.Replace("Basic","").Replace("Legendary","").Trim().Split("—")[0].Trim())
                .Select(g => new
                {
                    Key = g.Key,
                    Value = g.Count()
                })
                .OrderByDescending(x=>x.Value)
                .ToDictionary(x => x.Key, x => x.Value);

            var manaCountGroup = deck.Content
                .Where(x=>x.CardData.Type.Contains("Land") == false )
                .GroupBy(x => x.CardData.CMC)
                .Select(g => new
                {
                    Key = g.Key,
                    Value = g.Count()
                });

            int[] manaCurveArray = new int[15];
            foreach (var group in manaCountGroup)
            {
                manaCurveArray[(Int32)group.Key] = group.Value;
            }

            var model = new DeckStatisticsViewModel
            {
                DeckId = deckId,
                DeckName = deck.Name,
                OwnerId = deck.AppUserId,
                Size = deck.Content.Sum(x => x.Quantity),
                ManaCurve = manaCurveArray,
                TypeDistribution = typeCountGroup,
                Cards = deck.Content
                    .Select(x=>(x.Quantity, x.CardData))
            };
            return PartialView("_DeckOverview", model);
        }

        [HttpGet]
        [Route("api/DeckStatistics/{deckId}")]
        public async Task<JsonResult> StatisticsData(int deckId)
        {
            var deck = await _deckRepository.GetDeckById(deckId);

            if (deck == null) return Json(NotFound());

            var typeCountGroup = deck.Content
                .GroupBy(x => x.CardData.Type.Split("—")[0])
                .Select(g => new
                {
                    Key = g.Key,
                    Value = g.Count()
                }).ToDictionary(x => x.Key, x => x.Value);

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
                TypeDistribution = typeCountGroup,
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
