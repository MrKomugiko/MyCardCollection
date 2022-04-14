using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyCardCollection.Data;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using MyCardCollection.Services;
using MyCardCollection.Services.Scryfall.Card;
using System.Security.Claims;

namespace MyCardCollection.Controllers
{
    public class CollectionController : Controller
    {
        private readonly IScryfallService _mtgApi;
        private readonly ICollectionRepository _collectionService;
        private readonly ICacheService _cacheService;
        private readonly ApplicationDbContext _context;

        string _userId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string collection_cacheKey => _userId + "Collection";
        public CollectionController(ApplicationDbContext context, IScryfallService mtgApi, ICollectionRepository collectionService, ICacheService cacheService)
        {
            _context = context;
            _mtgApi = mtgApi;
            _collectionService = collectionService;
            _cacheService = cacheService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _collectionService.GetAll_SearchCardsFromCollection(_userId);
            int? count = HttpContext.Session.GetInt32("count") ?? null;
            if (count == null)
            {
                count = result.Sum(x => x.Quantity);
                HttpContext.Session.SetInt32("count", (int)count);
            }

            ViewBag.Count = count;
            ViewBag.DistinctCount = result.Count();

            return View();
        }
        public async Task<PartialViewResult> SearchCards(string searchText, int page = 1, string eventName = "")
        {
            int pageSize = 15;
            var result = await _collectionService.SearchCardsFromCollection(_userId, searchText, page, pageSize);

            ViewBag.PageNumber = page;
            ViewBag.PageRange = pageSize > 6 ? 6 : pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(((decimal)((int)result.totalMatches)) / pageSize);
            ViewBag.clickEventName = eventName;

            return PartialView("_FullGridView", result.cardsOnPage);
        }
        public async Task<IActionResult> Add(string set, int number)
        { 
            var cardData = _context.CardsDatabase.Where(x => x.SetCode == set && x.CollectionNumber == number).FirstOrDefault();
            if (cardData == null)
            {
                Root _card = await _mtgApi.FindCard(set: set, number: number);
                cardData = new CardData(_card);
                _context.CardsDatabase.Add(new CardData(_card));
            }

            var match = _context.Collection.Where(x => x.UserId == _userId && x.CardId == cardData.CardId).FirstOrDefault();

            if (match == null)
            {
                _context.Collection.Add(new CardsCollection(_userId, cardData.CardId, 1));
            }
            else
            {
                match.Quantity += 1;
            }

            _context.SaveChanges();
            //dodanie karty spowoduje zaktualizowanie kart w cache


            //checks if cache entries exists
            if (_cacheService.TryGetValue(collection_cacheKey, out List<CardsCollection> cachedAllCards))
            {
                //setting cache entries
                if(cachedAllCards.Any(c=>c.CardId == cardData.CardId))
                {
                    cachedAllCards.Where(c => c.CardId == cardData.CardId).First().Quantity += 1;
                }
                else
                {
                    var collection = new CardsCollection(_userId, cardData.CardId, 1);
                        collection.CardData = cardData;
                    cachedAllCards.Add(collection);

                }
                _cacheService.Set(collection_cacheKey, cachedAllCards);
            }

            int count = HttpContext.Session.GetInt32("count") ?? 0;
            HttpContext.Session.SetInt32("count", (int)count+1);

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if(file == null) return Redirect(Request.Headers["Referer"].ToString());

            Root _card = null;
            string data = "";

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                // Upload the file if less than 2 MB
                if (Path.GetExtension(file.FileName).Contains("txt") == false)
                {
                    return Redirect(Request.Headers["Referer"].ToString());
                };

                if (memoryStream.Length < 2097152)
                {
                    byte[] content = memoryStream.ToArray();

                    data = System.Text.Encoding.UTF8.GetString(content);
                }
                else
                {
                    return Redirect(Request.Headers["Referer"].ToString());
                }
            }

            string[] dataArr = data.Trim().Split(',');

            var cardsListToImport = new List<CardsCollection>();
            var newCardsListToAdd = new List<CardData>();
            int counter = 0;

            int count = HttpContext.Session.GetInt32("count") ?? 0;
            foreach (var carddata in dataArr)
            {
                counter++;
                if (counter == 24)
                {
                    counter = 0;
                    await _context.Collection.AddRangeAsync(cardsListToImport);
                    await _context.CardsDatabase.AddRangeAsync(newCardsListToAdd);
                    await _context.SaveChangesAsync();

                    cardsListToImport.Clear();
                    newCardsListToAdd.Clear();

                    // po imporcie cache zostanie wyczyszczony, zeby po odswiezeniu miec aktualne dane
                    _cacheService.Remove(collection_cacheKey);
                    HttpContext.Session.SetInt32("count", (int)count + cardsListToImport.Sum(x=>x.Quantity));
                }

                string[] card = carddata.Trim().Split(" ").Where(x => x != "").ToArray();
                _card = await _mtgApi.FindCard(set: card[1].ToLower(), number: Int32.Parse(card[2]));

                var _cardData = new CardData(_card);

                if (_context.Collection.Any(x => x.CardId == _cardData.CardId && x.UserId == _userId))
                {
                    _context.Collection.Where(x => x.CardId == _cardData.CardId && x.UserId == _userId).First().Quantity += Int32.Parse(card[0]);
                }
                else
                {
                    if (cardsListToImport.Any(x => x.CardId == _cardData.CardId && x.UserId == _userId))
                    {
                        cardsListToImport.Where(x => x.CardId == _cardData.CardId && x.UserId == _userId).First().Quantity += Int32.Parse(card[0]);             
                    }
                    else
                    {
                        if (_context.CardsDatabase.Any(x => x.CardId == _cardData.CardId) == false)
                        {
                            newCardsListToAdd.Add(_cardData);
                        }
                        cardsListToImport.Add(new CardsCollection(_userId, _cardData.CardId, Int32.Parse(card[0])));
                    }
                }
            }

            await _context.Collection.AddRangeAsync(cardsListToImport);
            await _context.CardsDatabase.AddRangeAsync(newCardsListToAdd);

            _context.SaveChanges();
            // po imporcie cache zostanie wyczyszczony, po przekierowaniu zostanie ponownie zapelniony danymi uzupelnionymi o wczesniej zaimportowane
            _cacheService.Remove(collection_cacheKey);

            return Redirect(Request.Headers["Referer"].ToString());
        }
        [Authorize]
        public async Task<IActionResult> Clear()
        {
            await _collectionService.ClearCollectionAsync(_userId);
    
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult Increase(string cardid)
        {
            var _card = _context.Collection.Where(x => x.CardId == cardid && x.UserId == _userId).First();
            var updatedCardID = _card.CardId;
            _context.SaveChanges();

            //update cache
            if (_cacheService.TryGetValue(collection_cacheKey, out List<CardsCollection> cachedAllCards))
            {
                //setting up cache options
                var cardToUpdate = cachedAllCards.FirstOrDefault(c => c.CardId == updatedCardID);
                if (cardToUpdate != null)
                {
                    cachedAllCards.First(c => c.CardId == cardToUpdate.CardId).Quantity += 1;
                    _cacheService.Set(collection_cacheKey, cachedAllCards);
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        public IActionResult Decrease(string cardid)
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var _card = _context.Collection.Where(x => x.CardId == cardid && x.UserId == _userId).First();
            var updatedCardID = _card.CardId;
            bool deleteflag = false;

            if (_card.Quantity > 1)
                _card.Quantity -= 1;
            else
            {
                _context.Collection.Remove(_card);
                deleteflag = true;
            }

            _context.SaveChanges();

            //update cache
            if (_cacheService.TryGetValue(collection_cacheKey, out List<CardsCollection> cachedAllCards))
            {
                //setting up cache options
                var cardToUpdate = cachedAllCards.FirstOrDefault(c => c.CardId == updatedCardID);
                if(cardToUpdate != null)
                {
                    if (deleteflag)
                        cachedAllCards.Remove(cardToUpdate);
                    else
                        cachedAllCards.First(c => c.CardId == cardToUpdate.CardId).Quantity -= 1;

                  _cacheService.Set(collection_cacheKey, cachedAllCards);
                }
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
        public IActionResult Remove(string cardid)
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var _card = _context.Collection.Where(x => x.CardId == cardid && x.UserId == _userId).First();
            var removedCardID = _card.CardId;

            if (_card.Quantity >= 0)
            {
                _context.Collection.Remove(_card);
            }
            _context.SaveChanges();

            //update cache
            if (_cacheService.TryGetValue(collection_cacheKey, out List<CardsCollection> cachedAllCards))
            {
                //setting up cache options
                var cardToDelete = cachedAllCards.FirstOrDefault(c=>c.CardId==removedCardID);
                if(cardToDelete!=null)
                {
                    cachedAllCards.Remove(cardToDelete);
                    _cacheService.Set(collection_cacheKey, cachedAllCards);
                }
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
        public IActionResult GetTopCards(string id)
        {
            var x = HttpContext.Request.Headers["X-Requested-With"];
            if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
                return Redirect(Request.Headers["Referer"].ToString());

            return ViewComponent("CardsTop", new { userId = id });
        }
        public IActionResult GetStatisticsComponent()
        {
            return ViewComponent("StatisticsCharts", new { _userId = _userId });
        }
        public IActionResult GetStatisticsComponent_v2()
        {
            return ViewComponent("StatisticsCharts", new { _userId = _userId });
        }
    }
}
