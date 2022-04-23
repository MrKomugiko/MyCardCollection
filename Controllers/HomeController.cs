using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using MyCardCollection.Services;
using MyCardCollection.Services.Scryfall;
using MyCardCollection.ViewModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyCardCollection.Controllers
{
    public class HomeController : Controller
    {
        private readonly IScryfallService _scryfall;
        private readonly ICacheService _cacheService;
        public HomeController(IScryfallService scryfall, ICacheService cacheService)
        {
            _scryfall = scryfall;
            _cacheService = cacheService;
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

            var model = listdata.Skip((page - 1) * PageSize)
                .Take(PageSize);

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