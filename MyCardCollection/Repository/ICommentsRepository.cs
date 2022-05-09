using MyCardCollection.Models;

namespace MyCardCollection.Repository
{
    public interface ICommentsRepository
    {
        Task<IEnumerable<Comment>> GetCommentsByDeckId(int _deckId);
    }
}
