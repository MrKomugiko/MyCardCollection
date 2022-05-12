using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static MyCardCollection.Repository.CommentsRepository;

namespace MyCardCollection.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentsRepository _commentRepository;
        private readonly IUsersRepository _usersRepository;

        public CommentController(ICommentsRepository commentRepository, IUsersRepository usersRepository)
        {
            _commentRepository = commentRepository;
            _usersRepository = usersRepository;
        }

        [HttpGet]
        [Route("api/Comments/{deckId}")]
        public async Task<JsonResult> GetComments(int deckId)
        {
            IEnumerable<Comment> data = await _commentRepository.GetCommentsByDeckId(deckId);

            var users = await _usersRepository.GetUsersDataAsync();
            Dictionary<string, AppUser> usersDict = _usersRepository.GetUsersDataAsync().Result.ToDictionary(x => x.Id, x => new AppUser { Id=x.Id, UserName = x.UserName, AvatarImage = x.AvatarImage });

            foreach(var comment in data)
            {
                comment.Author = usersDict[comment.AuthorId];
                foreach (var reply_1 in comment.Replies)
                {
                    reply_1.Author = usersDict[reply_1.AuthorId];
                    foreach (var reply_2 in reply_1.ChildReplies)
                    {
                        reply_2.Author = usersDict[reply_2.AuthorId];
                        foreach (var reply_3 in reply_2.ChildReplies)
                        {
                            reply_3.Author = usersDict[reply_3.AuthorId];
                        }
                    }
                }
            }
            
            return Json(Ok(data));
        }

        [HttpPost]
        [Route("api/Comments/AddComment")]
        public async Task<JsonResult> AddComment([FromBody] AddCommentInput data)
        {
            if(ModelState.IsValid)
            {
                string _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
                int createdCommentId = await _commentRepository.AddComment(_userId, data.DeckId, data.Content);
                if(createdCommentId > 0)
                {
                    GetCommentPOST_return comment = await _commentRepository.GetCommentPOSTreturn(createdCommentId);
                    return Json(Ok(comment));
                }
                else
                    return Json(new { msg = "Error, adding comment failed." });
            }
            return Json(BadRequest());
        }
        public class AddCommentInput
        {
            [Required] public int DeckId { get; set; }
            [Required][MaxLength(255)] public string Content { get; set; }
        }

        [HttpPost]
        [Route("api/Comments/AddReply")]
        public async Task<JsonResult> AddReply([FromBody] AddReplyInput data)
        {
            if (ModelState.IsValid) 
            {
                string _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                int createdReplyId = await _commentRepository.AddReply(_userId, data.CommentId, data.ReplyTo > 0 ? data.ReplyTo : null, data.Content, data.Depth); ;
                if (createdReplyId > 0)
                {
                    GetReplyPOST_return reply = await _commentRepository.GetReplyPOSTreturn(createdReplyId);
                    return Json(Ok(reply));
                }
                else
                    return Json(new { msg = "Error, adding comment failed." });
            }
            return Json(BadRequest());
        }
        public class AddReplyInput
        {
            public int CommentId { get; set; }
            public int ReplyTo { get; set; }
            public int Depth { get; set; }
            public string Content { get; set; }

        }
        [HttpGet]
        public async Task<PartialViewResult> LoadCommentByDeck(int deckId)
        {
            List<Comment> data = await _commentRepository.GetCommentsByDeckId(deckId);
            var users = await _usersRepository.GetUsersDataAsync();
            Dictionary<string, AppUser> usersDict = _usersRepository.GetUsersDataAsync().Result.ToDictionary(x => x.Id, x => new AppUser { Id = x.Id, UserName = x.UserName, AvatarImage = x.AvatarImage });

            foreach (var comment in data)
            {
                comment.Author = usersDict[comment.AuthorId];
                foreach (var reply_1 in comment.Replies)
                {
                    reply_1.Author = usersDict[reply_1.AuthorId];
                    foreach (var reply_2 in reply_1.ChildReplies)
                    {
                        reply_2.Author = usersDict[reply_2.AuthorId];
                        foreach (var reply_3 in reply_2.ChildReplies)
                        {
                            reply_3.Author = usersDict[reply_3.AuthorId];
                        }
                    }
                }
            }

            return PartialView("_Comments", new ValueTuple<int,List<Comment>>(deckId, data));
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