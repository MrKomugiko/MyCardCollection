using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using MyCardCollection.Data;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using MyCardCollection.Services;
using System;
using System.Net;
using System.Security.Claims;

namespace MyCardCollection.Controllers
{
    public partial class DeckBuilderController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly ICollectionRepository collectionService;
        private readonly IDeckRepository _deckRepository;
        private readonly IMemoryCache _memoryCache;

        public DeckBuilderController(ApplicationDbContext context, ICollectionRepository collectionService, IDeckRepository deckRepository, IMemoryCache memoryCache)
        {
            this.context = context;
            this.collectionService = collectionService;
            this._deckRepository = deckRepository;
            _memoryCache = memoryCache;
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

            var (cardsOnPage, totalMatches) = await _deckRepository.SearchCardsFromDeck(_userId, searchText, page, pageSize, deckName: deckName);

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

            await _deckRepository.AddCardToDeckAsync(_userId, id, deckName);
            
            var (cardsOnPage, totalMatches) = await _deckRepository.SearchCardsFromDeck(_userId, null, lastDeckPage, pageSize, deckName);

            ViewBag.DeckPageNumber = lastDeckPage;
            ViewBag.RecentlyAddedCard = id;
            ViewBag.PageRange = pageSize > 6 ? 6 : pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(((decimal)((int)totalMatches)) / pageSize);

            return PartialView("_DeckView", cardsOnPage);
        }

        [HttpPost]
        public async Task<JsonResult> SaveDeckAsync([FromBody] DeckModel deck)
        {
            deck.userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(deck.deckName))
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { IsCreated = false, ErrorMessage = "Deck name cannot be empty." });
            }
            await _deckRepository.Update(deck, deck.userId);
           
            return Json("success");
        }

        public JsonResult ChangeQuantity(string id, int qtChange, string? deckName)
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Change current quantity from deck
            (string cardId, int? updatedQuantity, int? cardLeftInCollection, string response) = _deckRepository.UpdateQuantityFromDeck(_userId, id, deckName, qtChange);
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
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allcardsfromdeck = await _deckRepository.GetAll_SearchCardsFromDeck(_userId, null, deckName: deckName);
            var ungruppedListCardsFromDeck = new List<CardsCollection>();
            allcardsfromdeck.ForEach(x => ungruppedListCardsFromDeck.AddRange(Enumerable.Repeat(x, x.Quantity)));
            ungruppedListCardsFromDeck.Shuffle();

            return Json(ungruppedListCardsFromDeck);
        }

        public async Task<IActionResult> Index()
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Load available decks names

            //var cardsInCollection = await collectionService.GetAll_SearchCardsFromCollection(_userId);

            //ViewBag.LiczbaKart = cardsInCollection.Sum(x => x.Quantity);
            //ViewBag.LiczbaWariacji = cardsInCollection.Count;
            //ViewBag.LiczbaUnikalnych = cardsInCollection.GroupBy(x => x.CardData.Name).Count();

            //var bezLadow = cardsInCollection.Where(x => x.CardData.Type.Contains("Land") == false);
            //ViewBag.LiczbaBezLadow = bezLadow.Count();
            //ViewBag.Commons = bezLadow.Where(x => x.CardData.Rarity == "common").Sum(x => x.Quantity);
            //ViewBag.Uncommons= bezLadow.Where(x=>x.CardData.Rarity == "uncommon").Sum(x=>x.Quantity);
            //ViewBag.Rares = bezLadow.Where(x=>x.CardData.Rarity == "rare").Sum(x=>x.Quantity);
            //ViewBag.Mythics = bezLadow.Where(x=>x.CardData.Rarity == "mythic").Sum(x=>x.Quantity);


            // selecting deck:
            var userDecks = await _deckRepository.GetDeckNames(_userId);
            List<SelectListItem> data = new();
            data.Add(new SelectListItem { Value = null, Text = "- not selected -" });
            for (int i = 0; i < userDecks.Length; i++)
            {
                data.Add(new SelectListItem { Value = userDecks[i].ToString(), Text = userDecks[i].ToString() });
            }
            string? currentDeck = HttpContext.Session.GetString("CurrentSelectedDeck") ?? "- not selected -";
            if(! data.Any(x=>x.Text == currentDeck) && currentDeck != "- not selected -")
            {
                data.Add(new SelectListItem { Value = currentDeck.Trim(), Text = currentDeck.Trim() });
            }
            ViewBag.MyDecks = data;
            HttpContext.Session.SetString("CurrentSelectedDeck", currentDeck);
            ViewBag.CurrentDeckName = currentDeck;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewDeckName(string deckTitle)
        {
            HttpContext.Session.SetString("CurrentSelectedDeck", deckTitle);
            ViewBag.CurrentDeckName = deckTitle;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> LoadDeck(string? deckName)
        {

            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            HttpContext.Session.SetString("CurrentSelectedDeck", deckName.Trim());

            return RedirectToAction("Index");
        }
     
        public async Task<IActionResult> Clear()
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? currentDeck = HttpContext.Session.GetString("CurrentSelectedDeck") ?? null;
            

            if(currentDeck != null && currentDeck != "- not selected -")
            {
                HttpContext.Session.Remove("CurrentSelectedDeck");
                await _deckRepository.ClearDeck(currentDeck,_userId);
                var deck_cacheKey = _userId + "Deck" + currentDeck.Trim();
                _memoryCache.Remove(deck_cacheKey);
            }

            return RedirectToAction("Index");
        }

    }
}
