using MyCardCollection.Controllers;
using MyCardCollection.Models;

namespace MyCardCollection.Repository
{
    public interface IUsersRepository
    {
        bool Add(AppUser user);
        bool Delete(AppUser user);
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<int> GetCountUsersAsync();
        Task<AppUser> GetUserByIdAsync(string id);
        bool Save();
        bool Update(AppUser user);
        void UpdatePlayerStatistics(string userId);
        void UpdateAllPlayersStatistics();
        Task<IEnumerable<AppUser>> GetFullUsersDataAsync();
        Task<IEnumerable<AppUser>> GetFullUsersDataAsyncByCategory(CollectionersSortCategory category);
    }
}