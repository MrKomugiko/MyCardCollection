
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCardCollection.Models
{
    public class DecksCollection
    {

        public DecksCollection(string userId, string cardId, int quantity, int deckID)
        {
            this.UserId = userId;
            this.CardId = cardId;
            this.Quantity = quantity;
            this.DeckId = deckID;
        }
        public DecksCollection(string userId, int deckID)
        {
            this.UserId = userId;
            this.CardId = "";
            this.Quantity = 0;
            this.DeckId = deckID;
        }

        public DecksCollection()
        {
        }

        [Key] public int Id { get; set; }

        [Required] public string? UserId { get; set; }
        [ForeignKey("UserId")] public AppUser? AppUser { get; set; }

        [Required] public int DeckId { get; set; }
        [ForeignKey("DeckId")] public Deck? Deck { get; set; }

        [Required] public int Quantity { get; set; }

        [Required] public string CardId { get; set; }
        [ForeignKey("CardId")] public CardData? CardData { get; set; }

    }
}
