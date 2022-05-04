using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyCardCollection.Data;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using MyCardCollection.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;

namespace MyCardCollection.Controllers
{
    public partial class DeckBuilderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IDeckRepository _deckRepository;
        private readonly ICacheService _cacheService;

        private string _userId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string all_cacheKey => _userId + "Collection";
        private MemoryCacheEntryOptions cacheExpiryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddSeconds(3600),
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromSeconds(3600)
        };

        public DeckBuilderController(ApplicationDbContext context, ICollectionRepository collectionService, 
            IDeckRepository deckRepository, ICacheService cacheService)
        {
            this._context = context;
            this._collectionRepository = collectionService;
            this._deckRepository = deckRepository;
            _cacheService = cacheService;
        }
        public async Task<PartialViewResult> SearchCards(string searchText, int page = 1)
        {
            int pageSize = 10;
            var (cardsOnPage, totalMatches) = await _collectionRepository.SearchCardsFromCollection(_userId, searchText, page, pageSize);

            ViewBag.PageNumber = page;
            ViewBag.PageRange = pageSize > 6 ? 6:pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(((decimal)((int)totalMatches)) / pageSize);

            ViewBag.CurrentDeckID = HttpContext.Session.GetInt32("CurrentSelectedDeck"+ _userId) ?? -1;

            return PartialView("_GridView", cardsOnPage);
        }
        public async Task<PartialViewResult> SearchCardsInDeck(string searchText, int page = 1, int? deckId = -1)
        {
           // var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int pageSize = 10;
            
            HttpContext.Session.SetInt32("lastDeckPage", page);

            var (cardsOnPage, totalMatches) = await _deckRepository.SearchCardsFromDeck(_userId, searchText, page, pageSize, deckId: deckId);

            ViewBag.DeckPageNumber = page;
            ViewBag.PageRange = pageSize > 6 ? 6 : pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(((decimal)((int)totalMatches)) / pageSize);

            ViewBag.CurrentDeckID = HttpContext.Session.GetInt32("CurrentSelectedDeck"+ _userId) ?? -1;

            return PartialView("_DeckView", cardsOnPage);
        }

        [HttpPost]
        public async Task<ActionResult> AddToDeck(string id, int itemsPerPage = 10)
        {
            int pageSize = itemsPerPage;
            int lastDeckPage = HttpContext.Session.GetInt32("lastDeckPage") ?? 1;


            int deckId = HttpContext.Session.GetInt32("CurrentSelectedDeck"+ _userId) ?? -1;
            ViewBag.CurrentDeckID = deckId;

            await _deckRepository.AddCardToDeckAsync(_userId, id, deckId);
            
            var (cardsOnPage, totalMatches) = await _deckRepository.SearchCardsFromDeck(_userId, null, lastDeckPage, pageSize, deckId);

            ViewBag.DeckPageNumber = lastDeckPage;
            ViewBag.RecentlyAddedCard = id;
            ViewBag.PageRange = pageSize > 6 ? 6 : pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(((decimal)((int)totalMatches)) / pageSize);

            return PartialView("_DeckView", cardsOnPage);
        }

        [HttpPost]
        public async Task<JsonResult> SaveDeckAsync([FromBody] DeckModel deck)
        {
            if (deck.deckId == -1)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { IsCreated = false, ErrorMessage = "Deck name cannot be empty." });
            }
            await _deckRepository.Update(deck, deck.userId);
           
            return Json("success");
        }

        public JsonResult ChangeQuantity(string id, int qtChange, int deckId = -1)
        {
           // var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Change current quantity from deck
            (string cardId, int? updatedQuantity, int? cardLeftInCollection, string response) = _deckRepository
                .UpdateQuantityFromDeck(_userId, id, deckId, qtChange);

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

        [HttpPost]
        public JsonResult RevertLocalChangesCardsQuantity()
        {
            // var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? currentDeckID = HttpContext.Session.GetInt32("CurrentSelectedDeck"+ _userId) ?? -1;
            var deck_cacheKey = _userId + "Deck" + currentDeckID;

            _cacheService.Remove(deck_cacheKey);
            _cacheService.Remove(all_cacheKey);

            return Json("Succesfully reverted.");
        }
        public async Task<JsonResult> GetFirstDrawUrlsAsync(int? deckid = -1)
        {
           // var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allcardsfromdeck = await _deckRepository.GetAll_SearchCardsFromDeck(_userId, null, deckId: deckid);
            var ungruppedListCardsFromDeck = new List<CardsCollection>();
            allcardsfromdeck.ForEach(x => ungruppedListCardsFromDeck.AddRange(Enumerable.Repeat(x, x.Quantity)));
            ungruppedListCardsFromDeck.Shuffle();

            return Json(ungruppedListCardsFromDeck);
        }

        public async Task<IActionResult> Index()
        {
          //  var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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


            // selecting deck:id/name
            Dictionary<int,string> userDecks = await _deckRepository.GetDeckNames(_userId);

            List<SelectListItem> data = new();
            data.Add(new SelectListItem { Value = "-1", Text = "- not selected -" });
            for (int i = 0; i < userDecks.Count; i++)
            {
                KeyValuePair<int, string> deck = userDecks.ElementAt(i);
                data.Add(new SelectListItem { Value = deck.Key.ToString(), Text = deck.Value });
            }

            int? currentDeckID = HttpContext.Session.GetInt32("CurrentSelectedDeck"+ _userId) ?? -1;
            if(currentDeckID != -1 && !data.Any(x => x.Value == currentDeckID.ToString()))
            {
                data.Add(new SelectListItem { Value = currentDeckID.ToString(), Text = userDecks[(int)currentDeckID].Trim() });
            }

            ViewBag.MyDecks = data;
            ViewBag.CurrentDeckID = currentDeckID;
            ViewBag.CurrentDeckName = currentDeckID != -1?userDecks[(int)currentDeckID]: "- not selected -";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewDeckName(string deckTitle, string owner)
        {
            ViewBag.CurrentDeckName = deckTitle;

            var allCards = await _collectionRepository.GetCardsFromCollection(owner);

            _cacheService.Set(all_cacheKey, allCards);

            await _deckRepository.CreateNewDeck(deckTitle, owner);

            Dictionary<int, string> userDecks = await _deckRepository.GetDeckNames(_userId);
            HttpContext.Session.SetInt32("CurrentSelectedDeck"+ _userId, userDecks.First(x=>x.Value == deckTitle).Key);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> LoadDeck(string deckid)
        {
            Dictionary<int, string> userDecks = await _deckRepository.GetDeckNames(_userId);
            HttpContext.Session.SetInt32("CurrentSelectedDeck"+ _userId, Int32.Parse(deckid));

            List<CardsCollection> cardsInDeck = new();
            var deck_cacheKey = _userId + "Deck" + deckid;

            if (!_cacheService.TryGetValue(deck_cacheKey, out List<CardsCollection> cachedCurrentDeck))
            {
                cardsInDeck = await _deckRepository.GetDeckDataFromDatabase(_userId, Int32.Parse(deckid));
            }
            else
            {
                // istnieje juz jakis aktualnie tworzony deck = AKTUALIZACJA 
                cardsInDeck = _cacheService.Get<List<CardsCollection>>(deck_cacheKey);

            }
            var allCards = await _context.Collection
                    .Where(x => x.UserId == _userId)
                    .Include(x => x.CardData)
                    .ToListAsync();

            foreach (var card in allCards.Where(x => cardsInDeck.Any(c => c.CardId == x.CardId)))
            {
                card.Quantity -= cardsInDeck.First(x=>x.CardId == card.CardId).Quantity;
            }

            _cacheService.Set(all_cacheKey, allCards);
            _cacheService.Set(deck_cacheKey, cardsInDeck);

            return RedirectToAction("Index");
        }
     
        public async Task<IActionResult> Clear()
        {
            int? currentDeck = HttpContext.Session.GetInt32("CurrentSelectedDeck"+ _userId) ?? null;    

            if(currentDeck != null && currentDeck != -1)
            {
                await _deckRepository.ClearDeck((int)currentDeck,_userId);

                var deck_cacheKey = _userId + "Deck" + currentDeck;

                _cacheService.Remove(deck_cacheKey);
                _cacheService.Remove(all_cacheKey);
            }

            return RedirectToAction("Index");
        }
    }
}
