using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyCardCollection.Data;
using MyCardCollection.Models;
using MyCardCollection.Services;
using System.Net;
using System.Security.Claims;

namespace MyCardCollection.Controllers
{
    public partial class DeckBuilderController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly ICollectionService collectionService;

        public DeckBuilderController(ApplicationDbContext context, ICollectionService collectionService)
        {
            this.context = context;
            this.collectionService = collectionService;
        }
        public async Task<PartialViewResult> SearchCards(string searchText, int page = 1)
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int pageSize = 10;
            var (cardsOnPage, totalMatches) = await collectionService.SearchCardsFromCollection(_userId, searchText, page, pageSize);

            ViewBag.PageNumber = page;
            ViewBag.PageRange = pageSize > 6 ? 6:pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(((decimal)((int)totalMatches)) / pageSize);

            string? currentDeck = HttpContext.Session.GetString("CurrentSelectedDeck") ?? null;
            ViewBag.CurrentDeckName = currentDeck;

            return PartialView("_GridView", cardsOnPage);
        }
        public async Task<PartialViewResult> SearchCardsInDeck(string searchText, int page = 1, string? deckName= null)
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int pageSize = 10;
            
            HttpContext.Session.SetInt32("lastDeckPage", page);

            var (cardsOnPage, totalMatches) = await collectionService.SearchCardsFromDeck(_userId, searchText, page, pageSize, deckName: deckName);

            ViewBag.DeckPageNumber = page;
            ViewBag.PageRange = pageSize > 6 ? 6 : pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(((decimal)((int)totalMatches)) / pageSize);

            string? currentDeck = HttpContext.Session.GetString("CurrentSelectedDeck") ?? null;
            ViewBag.CurrentDeckName = currentDeck;

            return PartialView("_DeckView", cardsOnPage);
        }

        [HttpPost]
        public async Task<ActionResult> AddToDeck(string id, int itemsPerPage = 10)
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int pageSize = itemsPerPage;
            int lastDeckPage = HttpContext.Session.GetInt32("lastDeckPage") ?? 1;

            string? currentDeck = HttpContext.Session.GetString("CurrentSelectedDeck") ?? null;
            ViewBag.CurrentDeckName = currentDeck;
            string? deckName = currentDeck;

            await collectionService.AddCardToDeckAsync(_userId, id, deckName);
            
            var (cardsOnPage, totalMatches) = await collectionService.SearchCardsFromDeck(_userId, null, lastDeckPage, pageSize, deckName);

            ViewBag.DeckPageNumber = lastDeckPage;
            ViewBag.RecentlyAddedCard = id;
            ViewBag.PageRange = pageSize > 6 ? 6 : pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(((decimal)((int)totalMatches)) / pageSize);

            return PartialView("_DeckView", cardsOnPage);
        }

        [HttpPost]
        public async Task<JsonResult> SaveDeckAsync([FromBody] DeckModel deck)
        {
            if(String.IsNullOrEmpty(deck.deckName))
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { IsCreated = false, ErrorMessage = "Deck name cannot be empty." });
            }

            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (_userId == null)
            {
                _userId = "625bb508-09c8-442f-aa96-f0fddcef4707"; // as default MrKomugiko
            }

            var currentDeckData = context.DecksCollections.Where(x => x.UserId == _userId && x.DeckName == deck.deckName);
            List<DecksCollection> updatedDeckData = deck.cardInfos.Select(card => new DecksCollection(_userId, card.cardId, card.quantity, deck.deckName)).ToList();

            context.RemoveRange(currentDeckData);
            await context.AddRangeAsync(updatedDeckData);
            await context.SaveChangesAsync();
           
            return Json("success");
        }

        public JsonResult ChangeQuantity(string id, int qtChange, string? deckName)
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Change current quantity from deck
            (string cardId, int? updatedQuantity, int? cardLeftInCollection, string response) = collectionService.UpdateQuantityFromDeck(_userId, id, deckName, qtChange);
            if(updatedQuantity.HasValue)
            {
                return Json (
                    new {
                        status = true,
                        cardId = cardId,
                        cardLeftInCollection = cardLeftInCollection,
                        result = updatedQuantity,
                        info = response

                    }
                );
            }
            else
            {
                return Json(
                    new
                    {
                        status = false,
                        info = response
                    }
                );
            }
        }
        public async Task<JsonResult> GetFirstDrawUrlsAsync(string? deckName)
        {
            //if(deckName == null)
            //{
            //    deckName = HttpContext.Session.GetString("CurrentSelectedDeck") ?? null;
            //}
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allcardsfromdeck = await collectionService.GetAll_SearchCardsFromDeck(_userId, null, deckName: deckName);
            var ungruppedListCardsFromDeck = new List<CardsCollection>();
            allcardsfromdeck.ForEach(x => ungruppedListCardsFromDeck.AddRange(Enumerable.Repeat(x, x.Quantity)));
            ungruppedListCardsFromDeck.Shuffle();

            return Json(ungruppedListCardsFromDeck);
        }

        public async Task<IActionResult> Index()
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cardsInCollection = await collectionService.GetAll_SearchCardsFromCollection(_userId);

            ViewBag.LiczbaKart = cardsInCollection.Sum(x => x.Quantity);
            ViewBag.LiczbaWariacji = cardsInCollection.Count;
            ViewBag.LiczbaUnikalnych = cardsInCollection.GroupBy(x => x.CardData.Name).Count();

            var bezLadow = cardsInCollection.Where(x => x.CardData.Type.Contains("Land") == false);
            ViewBag.LiczbaBezLadow = bezLadow.Count();
            ViewBag.Commons = bezLadow.Where(x => x.CardData.Rarity == "common").Sum(x => x.Quantity);
            ViewBag.Uncommons= bezLadow.Where(x=>x.CardData.Rarity == "uncommon").Sum(x=>x.Quantity);
            ViewBag.Rares = bezLadow.Where(x=>x.CardData.Rarity == "rare").Sum(x=>x.Quantity);
            ViewBag.Mythics = bezLadow.Where(x=>x.CardData.Rarity == "mythic").Sum(x=>x.Quantity);


            // selecting deck:
            List<SelectListItem> userDecks = await collectionService.GetPlayerDecksNames(_userId);
            ViewBag.MyDecks = userDecks;

            // string? currentDeck = HttpContext.Session.GetString("CurrentSelectedDeck") ?? null;
            string currentDeck = "deckTestowy_debug";
            HttpContext.Session.SetString("CurrentSelectedDeck", currentDeck);

            ViewBag.CurrentDeckName = currentDeck;


            return View();
        }



        [HttpPost]
        public async Task<IActionResult> LoadDeck(string? deckName)
        {
            HttpContext.Session.SetString("CurrentSelectedDeck", deckName.Trim());
            return RedirectToAction("Index");
        }
    }
}
