using MyCardCollection.Models;

namespace MyCardCollection.Repository
{
    public interface IDeckLikeRepository
    {
        Task<DeckLike?> AddLike(int _deckId, string? _userId);
        Task<IEnumerable<DeckLike>> GetLikesByDeck(int _deckId);
        Task<IEnumerable<DeckLike>> GetLikesByUser(string? _userId);
        Task RemoveLike(int _likeId);
        Task RemoveLike(int _deckId, string? _userId);
    }
}