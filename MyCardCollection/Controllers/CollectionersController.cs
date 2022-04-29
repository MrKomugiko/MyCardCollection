using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Repository;
using MyCardCollection.ViewModel;

namespace MyCardCollection.Controllers
{
    public class CollectionersController : Controller
    {
        private readonly IUsersRepository _usersRepository;
        public CollectionersController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<IActionResult> Index(int category = 0, int page = 1, int pageSize = 6)
        {
            if(page==1 && category==0)
            {
                _usersRepository.UpdateAllPlayersStatistics();
            }

            var users = await _usersRepository.GetFullUsersDataAsync();
            CollectionersViewModel model = new()
            {
                Users = users.Skip((page-1)*pageSize).Take(pageSize),
                TotalUsers = users.Count(),
                Category = category,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(users.Count() / (decimal)pageSize)
            };

            return View(model);
        }
    }
}
