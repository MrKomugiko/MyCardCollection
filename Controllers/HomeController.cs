using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Models;
using MyCardCollection.Services;
using MyCardCollection.ViewModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyCardCollection.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IScryfallService _scryfall;

        public HomeController(ILogger<HomeController> logger, IScryfallService scryfall = null)
        {
            _logger = logger;
            _scryfall = scryfall;
        }

        public async Task<IActionResult> Index()
        {
            var respond = await _scryfall.GetSetsList();
            List<SetListViewModel> listdata = new List<SetListViewModel>();

            foreach(var set in respond)
            {
                listdata.Add(
                    new SetListViewModel()
                    {
                        card_count = set.card_count,
                        icon_svg_uri = set.icon_svg_uri,
                        name = set.name,
                        released_at = @DateTime.Parse(set.released_at),
                        setcode = set.code
                    }
                );
            }
            return View(listdata);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}