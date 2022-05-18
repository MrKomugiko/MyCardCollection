using Microsoft.EntityFrameworkCore;
using MyCardCollection.Data;
using MyCardCollection.Models;

namespace MyCardCollection.Repository
{
    public class DeckLikesRepository : IDeckLikeRepository
    {
        private readonly ApplicationDbContext _context;
        public DeckLikesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DeckLike?> AddLike(int _deckId, string? _userId)
        {
            if(_context.DeckLikes.Any(x=>x.DeckId == _deckId && x.UserId == _userId))
            {
                // user already like it
                return null;
            }

            DeckLike like = new()
            {
                DeckId = _deckId,
                UserId = _userId
            };

            await _context.DeckLikes.AddAsync(like);

            await _context.SaveChangesAsync();

            return like;
        }
        public async Task RemoveLike(int _deckId, string? _userId)
        {
            DeckLike? like = _context.DeckLikes.SingleOrDefault(x => x.UserId == _userId && x.DeckId == _deckId);

            if (like != null)
            {
                _context.Remove(like);
                await _context.SaveChangesAsync();
            }
        }
        public async Task RemoveLike(int _likeId)
        {
            DeckLike? like = await _context.DeckLikes.FindAsync(_likeId);
            if(like != null)
            {
                _context.Remove(like);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<DeckLike>> GetLikesByUser(string? _userId)
        {
            List<DeckLike> UserLikes = await _context.DeckLikes.Where(x => x.UserId == _userId).ToListAsync();
            return UserLikes;
        }
        public async Task<IEnumerable<DeckLike>> GetLikesByDeck(int _deckId)
        {
            List<DeckLike> DeckLikes = await _context.DeckLikes.Where(x => x.DeckId == _deckId).ToListAsync();
            return DeckLikes;
        }
    }
}
