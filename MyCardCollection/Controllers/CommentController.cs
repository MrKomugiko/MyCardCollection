using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static MyCardCollection.Repository.CommentsRepository;

namespace MyCardCollection.Controllers
{
    public partial class CommentController : Controller
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
            IEnumerable<AppUser> users = await _usersRepository.GetUsersDataAsync();
            Dictionary<string, AuthorRespond> usersDict = users.ToDictionary(x => x.Id, x => new AuthorRespond() { Id = x.Id, AvatarImage = x.AvatarImage, UserName = x.UserName });

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

            return Json(Ok(data));
        }

        [HttpPost]
        [Route("api/Comments/AddComment")]
        public async Task<JsonResult> AddComment([FromBody] AddCommentRequest data)
        {
            if (ModelState.IsValid)
            {
                string _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                int createdCommentId = await _commentRepository.AddComment(_userId, data.DeckId, data.Content);
                if (createdCommentId > 0)
                {
                    GetCommentRespond comment = await _commentRepository.GetCommentPOSTreturn(createdCommentId);
                    return Json(Ok(comment));
                }
                else
                    return Json(new { msg = "Error, adding comment failed." });
            }
            return Json(BadRequest());
        }

        [HttpPost]
        [Route("api/Comments/AddReply")]
        public async Task<JsonResult> AddReply([FromBody] AddReplyRequest data)
        {
            if (ModelState.IsValid)
            {
                string _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                int createdReplyId = await _commentRepository.AddReply(_userId, data.CommentId, data.ReplyTo > 0 ? data.ReplyTo : null, data.Content, data.Depth); ;
                if (createdReplyId > 0)
                {
                    GetReplyRespond reply = await _commentRepository.GetReplyPOSTreturn(createdReplyId);
                    return Json(Ok(reply));
                }
                else
                    return Json(new { msg = "Error, adding comment failed." });
            }
            return Json(BadRequest());
        }

        [HttpDelete]
        [Route("api/Comments/Delete/{commentId}")]
        public JsonResult Delete(int commentId, [FromBody] int category)
        {
            var result = _commentRepository.Delete(commentId, (CommentType)category);
            result.Wait();

            if(result.IsCompletedSuccessfully)
            {
                return Json(Ok());
            }

            return Json(NotFound());
        }

        public enum CommentType {
            Main = 0,
            Reply = 1,
        }

        [HttpPut]
        [Route("api/Comments/Edit/Comment/{commentId}")]
        public async Task<JsonResult> EditComment(int commentId, [FromBody] string data)
        {
            return Json(Ok());
        }


        [HttpGet]
        public async Task<PartialViewResult> LoadCommentByDeck(int deckId)
        {
            List<Comment> data = await _commentRepository.GetCommentsByDeckId(deckId);
            IEnumerable<AppUser> users = await _usersRepository.GetUsersDataAsync();
            Dictionary<string, AuthorRespond> usersDict = users.ToDictionary(x => x.Id, x => new AuthorRespond() { Id=x.Id, AvatarImage = x.AvatarImage, UserName = x.UserName} );
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
