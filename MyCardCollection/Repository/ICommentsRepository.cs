using MyCardCollection.Models;
using static MyCardCollection.Repository.CommentsRepository;

namespace MyCardCollection.Repository
{
    public interface ICommentsRepository
    {
        Task<List<Comment>> GetCommentsByDeckId(int _deckId);
        Task<int> AddComment(string userId, int deckId, string content);
        Task<int> AddReply(string userId, int commentId, int? replyTo, string content, int depth);
        Task<GetCommentPOST_return> GetCommentPOSTreturn(int _commentId);
        Task<GetReplyPOST_return> GetReplyPOSTreturn(int _replyId);
    }
}
