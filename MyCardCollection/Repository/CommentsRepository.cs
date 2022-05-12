using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyCardCollection.Data;
using MyCardCollection.Models;


namespace MyCardCollection.Repository
{
    public class CommentsRepository : ICommentsRepository
    {
        private readonly ApplicationDbContext _context;
        public CommentsRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Comment>> GetCommentsByDeckId(int _deckId)
        {
            var result = await _context.Comments
                .Where(x => x.DeckId == _deckId)
                .Include(x => x.Replies.Where(x => x.Depth == 1))
                    .ThenInclude(x => x.ChildReplies.Where(x => x.Depth == 2))
                        .ThenInclude(x => x.ChildReplies.Where(x => x.Depth == 3))
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .ToListAsync();
            //Select(x=>new AppUser { Id = x.Author.Id, AvatarImage = x.Author.AvatarImage, UserName = x.Author.UserName})
            return result;
        }

        public async Task<GetCommentPOST_return> GetCommentPOSTreturn(int _commentId)
        {
            var result = await _context.Comments
                .Where(x => x.Id == _commentId)
                .Select(x => new GetCommentPOST_return()
                {
                    Id = x.Id,
                    Content = x.Content,
                    Author = new AuthorSimple
                    {
                        Id = x.Author.Id,
                        UserName = x.Author.UserName,
                        AvatarImage = x.Author.AvatarImage
                    },
                    Created = x.Created
                })
                .AsNoTracking()
                .SingleOrDefaultAsync();

            return result;
        }
        public async Task<GetReplyPOST_return> GetReplyPOSTreturn(int _replyId)
        {
            var result = await _context.Comment_Replies
                 .Where(x => x.Id == _replyId)
                 .Select(x => new GetReplyPOST_return()
                 {
                     Id = x.Id,
                     Content = x.Content,
                     Author = new AuthorSimple
                     {
                         Id = x.Author.Id,
                         UserName = x.Author.UserName,
                         AvatarImage = x.Author.AvatarImage
                     },
                     Created = x.Created
                 })
                 .AsNoTracking()
                 .SingleOrDefaultAsync();

            return result;
        }

        public async Task<int> AddComment(string userId, int deckId, string content)        
        {
            Comment newComment = new() {
                AuthorId = userId,
                Content = content,
                DeckId = deckId,
                Created = DateTime.Now.ToUniversalTime(),
                Updated = DateTime.Now.ToUniversalTime()
            };

            await _context.Comments.AddAsync(newComment);
            
            if (_context.SaveChanges() > 0)
                return newComment.Id;    
            else
                return -1;
        }
        public async Task<int> AddReply(string userId, int commentId, int? replyTo, string content, int depth)
        {
            CommentReply newReply = new()
            {
                AuthorId = userId,
                Content = content,
                Created = DateTime.Now.ToUniversalTime(),
                Updated = DateTime.Now.ToUniversalTime(),
                CommentId = commentId,
                ReplyTo = replyTo,
                Depth = depth
            };

            await _context.Comment_Replies.AddAsync(newReply);

            if (_context.SaveChanges() > 0)
                return newReply.Id;
            else
                return -1;
        }

        public class GetCommentPOST_return
        {
            public int Id { get; set; }
            public string Content { get; set; }
            public AuthorSimple Author { get; set; }
            public DateTime Created { get; set; }
        }
        public class GetReplyPOST_return
        {
            public int Id { get; set; }
            public string Content { get; set; }
            public AuthorSimple Author { get; set; }
            public DateTime Created { get; set; }
        }
        public class AuthorSimple
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string AvatarImage { get; set; }
        }
    }

}

/*
 SELECT c."Id", c."AuthorId", c."Content", c."Created", c."DeckId", c."Updated", t1."Id", t1."AuthorId", t1."CommentId", t1."Content", t1."Created", t1."Depth", t1."ReplyTo", t1."Updated", t1."Id0", t1."AuthorId0", t1."CommentId0", t1."Content0", t1."Created0", t1."Depth0", t1."ReplyTo0", t1."Updated0", t1."Id00", t1."AuthorId00", t1."CommentId00", t1."Content00", t1."Created00", t1."Depth00", t1."ReplyTo00", t1."Updated00"
      FROM "Comments" AS c
      LEFT JOIN (
          SELECT c0."Id", c0."AuthorId", c0."CommentId", c0."Content", c0."Created", c0."Depth", c0."ReplyTo", c0."Updated", t0."Id" AS "Id0", t0."AuthorId" AS "AuthorId0", t0."CommentId" AS "CommentId0", t0."Content" AS "Content0", t0."Created" AS "Created0", t0."Depth" AS "Depth0", t0."ReplyTo" AS "ReplyTo0", t0."Updated" AS "Updated0", t0."Id0" AS "Id00", t0."AuthorId0" AS "AuthorId00", t0."CommentId0" AS "CommentId00", t0."Content0" AS "Content00", t0."Created0" AS "Created00", t0."Depth0" AS "Depth00", t0."ReplyTo0" AS "ReplyTo00", t0."Updated0" AS "Updated00"
          FROM "Comment_Replies" AS c0
          LEFT JOIN (
              SELECT c1."Id", c1."AuthorId", c1."CommentId", c1."Content", c1."Created", c1."Depth", c1."ReplyTo", c1."Updated", t."Id" AS "Id0", t."AuthorId" AS "AuthorId0", t."CommentId" AS "CommentId0", t."Content" AS "Content0", t."Created" AS "Created0", t."Depth" AS "Depth0", t."ReplyTo" AS "ReplyTo0", t."Updated" AS "Updated0"
              FROM "Comment_Replies" AS c1
              LEFT JOIN (
                  SELECT c2."Id", c2."AuthorId", c2."CommentId", c2."Content", c2."Created", c2."Depth", c2."ReplyTo", c2."Updated"
                  FROM "Comment_Replies" AS c2
                  WHERE c2."Depth" = 3
              ) AS t ON c1."Id" = t."ReplyTo"
              WHERE c1."Depth" = 2
          ) AS t0 ON c0."Id" = t0."ReplyTo"
          WHERE c0."Depth" = 1
      ) AS t1 ON c."Id" = t1."CommentId"
      WHERE c."DeckId" = @___deckId_0
      ORDER BY c."Id", t1."Id", t1."Id0"
 */