using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static MyCardCollection.Repository.CommentsRepository;

namespace MyCardCollection.Models
{
    public class CommentReply
    {
        /*
            => Deck                                             [ DECK ]
                - Comment                                       [ Comment #1 ]
                    * CommentReply                              [ Reply #1 to Comment #1 - depth:1]
                        ** CommentReply [depth:2]               [ Reply #2 to Reply #1  - depth:2]
                          *** CommentReply [depth:2]            [ Reply #2 to Reply #1  - depth:3]
                          *** CommentReply [depth:2]            [ Reply #2 to Reply #1  - depth:3]
                        ** CommentReply [depth:2]               [ Reply #3 to Reply #1  - depth:2]
                    *CommentReply [depth:1]                     [ Reply #4 to Comment #1 - depth:1]
                - Comment                                       [ Comment #2 ]
                    * CommentReply [depth:1]                    [ Reply #5 to Comment #2 - depth:1]
                - Comment                                       [ Comment #3 ]      
        */

        [Key] public int Id { get; set; }
        [Required] [MaxLength(255)] public string Content { get; set; }
        [Required] public string AuthorId { get; set; }
        [NotMapped] public AuthorRespond Author { get; set; }
        [ForeignKey("AuthorId")] public AppUser AppUser { get; set; }
        public DateTime Created { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime Updated { get; set; } = DateTime.Now.ToUniversalTime();
        [Required] public int CommentId { get; set; }


        public int Depth { get; set; } // max 3 docelowo
        public int? ReplyTo { get; set; }
        [ForeignKey("ReplyTo")] public virtual ICollection<CommentReply>? ChildReplies { get; set; }
    }
}
