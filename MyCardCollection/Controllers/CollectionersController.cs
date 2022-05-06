using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyCardCollection.Enums;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using MyCardCollection.Services;
using MyCardCollection.ViewModel;

namespace MyCardCollection.Controllers
{
    public class CollectionersController : Controller
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ICollectionRepository _collectionRepository;

        public CollectionersController(IUsersRepository usersRepository, ICollectionRepository collectionRepository)
        {
            _usersRepository = usersRepository;
            _collectionRepository = collectionRepository;
        }

        public async Task<IActionResult> Index(int category = 0, int page = 1, int pageSize = 6)
        {
            IEnumerable<AppUser> users = null;

            if(page==1 && category==0)
                _usersRepository.UpdateAllPlayersStatistics();

            bool valid_category = Enum.IsDefined(typeof(CollectionersSortCategory), category);
            if (category != 0 && valid_category)
                users = await _usersRepository.GetUsersAsyncByCategory((CollectionersSortCategory)category, includePrivacy: true);
            else if(category==0 || !valid_category)
                users = await _usersRepository.GetUsersDataAsync(includePrivacy:true);
            

            CollectionersViewModel model = new()
            {
                Users = users.Skip((page-1)*pageSize).Take(pageSize),
                TotalUsers = users.Count(),
                Category = GetSelectList((CollectionersSortCategory)category),
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(users.Count() / (decimal)pageSize)
            };

            return View(model);
        }
        
        [Route("Collectioner/{userId}")]
        public async Task<IActionResult> Profile(string userId)
        {
            CollectionerProfileViewModel model = new()
            {
                AppUser = await _usersRepository.GetUserByIdIncludeDecksAsync(userId),
                TopCards = await _collectionRepository.GetTopValuableCards(userId,take:8)
            };
            return View(model);
        }

        private IEnumerable<SelectListItem> GetSelectList<T>(T selectedCategory)
        {
            var values =Enum.GetValues(typeof(T)).Cast<T>();
            IEnumerable<SelectListItem> items =
                values.Select(value => new SelectListItem
                {
                    Text = value.ToString(),
                    Value = ((int)(object)value).ToString(),
                    Selected = Equals(value,selectedCategory)
                });
            return items;
        }
        
    }
}
