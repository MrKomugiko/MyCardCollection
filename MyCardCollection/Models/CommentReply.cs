﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCardCollection.Models
{
    public class CommentReply
    {
        [Key] public int Id { get; set; }
        [Required] [MaxLength(255)] public string Content { get; set; }
        /* 
          255 chars example:
          Lorem ipsum dolor sit amet, nonummy ligula volutpat hac integer
            nonummy. Suspendisse ultricies, congue etiam tellus, erat libero, nulla
            eleifend, mauris pellentesque. Suspendisse integer praesent vel, integer
            gravida mauris, fringilla vehicula lacinia non
         */
        [Required] public string AuthorId { get; set; }
        [ForeignKey("AuthorId")] public AppUser? Author { get; set; }
        public DateTime Created { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime Updated { get; set; } = DateTime.Now.ToUniversalTime();
        [Required] public int CommentId { get; set; }
        [ForeignKey("CommentId")] public Comment Comment { get; set; }
    }
}
