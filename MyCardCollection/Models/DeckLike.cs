using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCardCollection.Models
{
    public class DeckLike
    {
        [Key] public int Id { get; set; }

        public int? DeckId { get; set; }
        [ForeignKey("DeckId")] public Deck? Deck { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")] public AppUser? User{ get; set; }

        public DateTime Date { get; set; } = DateTime.Now.ToUniversalTime();
    }
}
