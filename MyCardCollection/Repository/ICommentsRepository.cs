using MyCardCollection.Controllers;
using MyCardCollection.Models;
using static MyCardCollection.Repository.CommentsRepository;

namespace MyCardCollection.Repository
{
    public interface ICommentsRepository
    {
        Task<List<Comment>> GetCommentsByDeckId(int _deckId);
        Task<int> AddComment(string userId, int deckId, string content);
        Task<int> AddReply(string userId, int commentId, int? replyTo, string content, int depth);
        Task<GetCommentRespond> GetCommentPOSTreturn(int _commentId);
        Task<GetReplyRespond> GetReplyPOSTreturn(int _replyId);
        Task Delete(int commentId, CommentController.CommentType category);
    }
}
