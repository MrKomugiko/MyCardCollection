
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCardCollection.Models
{
    public class DecksCollection
    {
        public DecksCollection(string userId, string cardId, int quantity, string deckName)
        {
            this.UserId = userId;
            this.CardId = cardId;
            this.Quantity = quantity;
            this.DeckName = deckName;
        }
        public DecksCollection(string userId, string deckName)
        {
            this.UserId = userId;
            this.CardId = "";
            this.Quantity = 0;
            this.DeckName = deckName;
        }

        [Key] public int Id { get; set; }
        [Required] public string UserId { get; set; }
        [Required] public string DeckName { get; set; }
        [Required] public string CardId { get; set; }
        [Required] public int Quantity { get; set; }

        [ForeignKey("CardId")] public virtual CardData CardData { get; set; }

    }
}
