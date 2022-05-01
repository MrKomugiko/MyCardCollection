using MyCardCollection.Controllers;
using MyCardCollection.Enums;
using MyCardCollection.Models;

namespace MyCardCollection.Repository
{
    public interface IUsersRepository
    {
        bool Add(AppUser user);
        bool Delete(AppUser user);
        Task<int> GetCountUsersAsync();
        Task<AppUser> GetUserByIdAsync(string id);
        bool Save();
        bool Update(AppUser user);
        void UpdatePlayerStatistics(string userId);
        void UpdateAllPlayersStatistics();
        Task<IEnumerable<AppUser>> GetUsersDataAsync();
        Task<IEnumerable<AppUser>> GetUsersAsyncByCategory(CollectionersSortCategory category);
    }
}