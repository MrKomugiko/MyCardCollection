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
            return View(userDecks.Where(x=>x.Content.Count > 0));
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
                    Value = g.Sum(x=>x.Quantity)
                })
                .OrderByDescending(x=>x.Value)
                .ToDictionary(x => x.Key, x => x.Value);

            var manaCountGroup = deck.Content
                .Where(x=>x.CardData.Type.Contains("Land") == false )
                .GroupBy(x => x.CardData.CMC)
                .Select(g => new
                {
                    Key = g.Key,
                    Value = g.Sum(x => x.Quantity)
                });

            int[] manaCurveArray = new int[15];
            foreach (var group in manaCountGroup)
            {
                manaCurveArray[(Int32)group.Key] = group.Value;
            }

            var setDistribution = deck.Content
                .GroupBy(x => x.CardData.SetCode)
                .Select(g => new
                {
                    Key = g.Key,
                    Value = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);

            char black = Char.Parse("B");
            char red = Char.Parse("R");
            char white = Char.Parse("W");
            char green = Char.Parse("G");
            char blue = Char.Parse("U");


            var colorDistribution = new Dictionary<string, int>
            {
                { "B", deck.Content.Where(x=>x.CardData.Type.Contains("Land") == false ).Sum(x=>x.CardData.Mana_Cost.Count(c=>c==black))},
                { "R", deck.Content.Where(x=>x.CardData.Type.Contains("Land") == false ).Sum(x=>x.CardData.Mana_Cost.Count(c=>c==red))},
                { "U", deck.Content.Where(x=>x.CardData.Type.Contains("Land") == false ).Sum(x=>x.CardData.Mana_Cost.Count(c=>c==blue))},
                { "G", deck.Content.Where(x=>x.CardData.Type.Contains("Land") == false ).Sum(x=>x.CardData.Mana_Cost.Count(c=>c==green))},
                { "W", deck.Content.Where(x=>x.CardData.Type.Contains("Land") == false ).Sum(x=>x.CardData.Mana_Cost.Count(c=>c==white))}
            };

            var model = new DeckStatisticsViewModel
            {
                DeckId = deckId,
                DeckName = deck.Name,
                OwnerId = deck.AppUserId,
                Size = deck.Content.Sum(x => x.Quantity),
                ManaCurve = manaCurveArray,
                TypeDistribution = typeCountGroup,
                SetDistribution = setDistribution,
                ColorDistribution = colorDistribution,
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

        [HttpPost]
        public async Task<JsonResult> EditDeckAsync(DeckEditViewModel deck)
        {
            // TODO, autorization, check user id and compare with deck owner id

            if(await _deckRepository.UpdateSingle(deck))
            {
                return Json(Ok());
            }

            return Json(new { Message = "Error, saving failed." });
        }
    }
}
