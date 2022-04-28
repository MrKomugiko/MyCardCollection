using Microsoft.EntityFrameworkCore;
using MyCardCollection.Data;
using MyCardCollection.Models;

namespace MyCardCollection.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _context;

        public UsersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(AppUser user)
        {
            throw new NotImplementedException();
        }

        public bool Delete(AppUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<int> GetCountUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(AppUser user)
        {
            _context.Update(user);
            return Save();
        }
        public async Task<IEnumerable<AppUser>> GetFullUsersDataAsync()
        {
            return await _context.Users
                .Include(x=>x.Decks)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
