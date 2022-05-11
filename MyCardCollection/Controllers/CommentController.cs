using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using System.Text;

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

            return PartialView("_Comments",data);
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