using MyCardCollection.Models;

namespace MyCardCollection.Repository
{
    public interface ICommentsRepository
    {
        Task<List<Comment>> GetCommentsByDeckId(int _deckId);
    }
}
