using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCardCollection.Data;
using System.Security.Claims;

namespace MyCardCollection.Infrastructure
{
    public class CardsTopViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext context;
        public CardsTopViewComponent(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IViewComponentResult Invoke(string _userId)
        {
            int DisplayedCardsNumber = 6;

            var MyCards = context.Collection
                .Where(x => x.UserId == _userId)
                .Include(x => x.CardData)
                .OrderByDescending(x => x.CardData.Price_USD)
                .Select(x => x.CardData)
                .Where(x=>x.Price_USD > 0);

            ViewBag.Sum = (float)MyCards.Sum(x => x.Price_USD);
            ViewBag.Count = DisplayedCardsNumber;

            if (MyCards.Count() == 0)
            {
                return View(MyCards.Take(0));
            }
            else
            {
                return View(MyCards.Take(DisplayedCardsNumber)); // default: shared/components/CardsTop/default
            }
        }

    }
}
