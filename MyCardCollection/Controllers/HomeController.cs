using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using MyCardCollection.Services;
using MyCardCollection.Services.Scryfall;
using MyCardCollection.ViewModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

namespace MyCardCollection.Controllers
{
    public class HomeController : Controller
    {
        private readonly IScryfallService _scryfall;
        private readonly ICacheService _cacheService;
        private readonly ICollectionRepository _collectionRepository;

        public HomeController(IScryfallService scryfall, ICacheService cacheService, ICollectionRepository collectionRepository)
        {
            _scryfall = scryfall;
            _cacheService = cacheService;
            _collectionRepository = collectionRepository;
        }

        public async Task<IActionResult> Index(int page = 1)
        { 
            var PageSize = 12;
            if (! _cacheService.TryGetValue<List<SetListViewModel>>("Sets", out var listdata))
            {
                var respond = await _scryfall.GetSetsList();
            
                listdata = new List<SetListViewModel>();

                foreach (var set in respond)
                {
                    listdata.Add(
                        new SetListViewModel()
                        {
                            card_count = set.card_count,
                            icon_svg_uri = set.icon_svg_uri,
                            name = set.name,
                            released_at = DateTime.Parse(set.released_at),
                            setcode = set.code
                        }
                    );
                }
                _cacheService.Set("Sets", listdata);
            }

            ViewBag.TotalPages = (Int32)Math.Ceiling((double)listdata.Count() / PageSize);
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = PageSize;
            ViewBag.DisplayAll = true;

            ViewBag.CardsBySet = new Dictionary<string,int>();
            if (User != null && User.Identity.IsAuthenticated)
            {
                ViewBag.CardsBySet = await _collectionRepository.GetSetCardCountGroupped(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            var model = listdata.Skip((page - 1) * PageSize)
                .Take(PageSize).AsEnumerable();

            return View(model);
        }

        [Authorize]
        [Route("OwnedSets")]
        public async Task<IActionResult> Index(int page = 1, bool owned = true)
        {
            var PageSize = 12;
            if (!_cacheService.TryGetValue<List<SetListViewModel>>("Sets", out var listdata))
            {
                var respond = await _scryfall.GetSetsList();

                listdata = new List<SetListViewModel>();

                foreach (var set in respond)
                {
                    listdata.Add(
                        new SetListViewModel()
                        {
                            card_count = set.card_count,
                            icon_svg_uri = set.icon_svg_uri,
                            name = set.name,
                            released_at = DateTime.Parse(set.released_at),
                            setcode = set.code
                        }
                    );
                }
                _cacheService.Set("Sets", listdata);
            }

            var userSets = await _collectionRepository.GetSetCardCountGroupped(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            ViewBag.TotalPages = (Int32)Math.Ceiling((double)userSets.Count() / PageSize);
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = PageSize;
            ViewBag.DisplayAll = false;

            ViewBag.CardsBySet = userSets;

            IEnumerable<SetListViewModel> model;
            if(userSets.Count>PageSize)
            {
                model = listdata.Where(x=>userSets.ContainsKey(x.setcode)).Skip((page - 1) * PageSize)
                    .Take(PageSize).AsEnumerable();
            }
            else
            {
                model = listdata.Where(x => userSets.ContainsKey(x.setcode));
            }
            return View(model);
        }

        public async Task<IActionResult> List(string set = "mid")
        {
            var respond = await _scryfall.GetCardsListBySet(set);
            List<CardListViewModel> listCards = new List<CardListViewModel>();

            foreach (var card_raw in respond)
            {

                    listCards.Add (
                        new CardListViewModel()
                        {
                            Card = new CardData(card_raw)
                        });
  
            }
            return View(listCards);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}