using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using System.Text;

namespace MyCardCollection.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentsRepository _commentRepository;
        public CommentController(ICommentsRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        [HttpGet]
        [Route("api/Comments/{deckId}")]
        public async Task<JsonResult> GetComments(int deckId)
        {
            IEnumerable<Comment> data = await _commentRepository.GetCommentsByDeckId(deckId);

            PrintComments(data.First());
           
            return Json(Ok(data));
        }

        private void PrintComments(Comment comment)
        {
            Console.WriteLine(comment.Content);
            GetReplies(comment.Replies);
        }

        private void GetReplies(IEnumerable<CommentReply> replies)
        {
            foreach(var reply in replies)
            {
                string space = new StringBuilder("\t".Length * reply.Depth).Insert(0, "\t", reply.Depth).ToString();
                Console.WriteLine(space+reply.Content);
            }
        }
    }

}

/*
 
Comment #1 MrKomugiko
    Reply #1 by Kmaka666 to Comment #1
        Reply #3 by MrKomugiko to Reply #1
            Reply #4 by UserX to Reply #3
            Reply #5 by Kmaka666 to Reply #3
    Reply #2 by User_FIRST to Comment #1
            Reply #7 by MrKomugiko to Reply #2
 
 */