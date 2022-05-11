using MyCardCollection.Models;
using static MyCardCollection.Repository.CommentsRepository;

namespace MyCardCollection.Repository
{
    public interface ICommentsRepository
    {
        Task<List<Comment>> GetCommentsByDeckId(int _deckId);
        Task<int> AddComment(string userId, int deckId, string content);
        Task<GetCommentPOST_return> GetCommentPOSTreturn(int _commentId);
    }
}
