namespace MyCardCollection.Controllers
{
    public partial class CommentController
    {
        public class AddReplyRequest
        {
            public int CommentId { get; set; }
            public int ReplyTo { get; set; }
            public int Depth { get; set; }
            public string Content { get; set; }

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