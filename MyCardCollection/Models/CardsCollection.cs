
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCardCollection.Models
{
    public class CardsCollection : ICloneable
    {  
        public CardsCollection(string userId, string cardId, int quantity)
        {
            UserId = userId;
            CardId = cardId;
            Quantity = quantity;
        }

        [Key]                       
        public int Id { get; set; }

        [Required]                  
        public string UserId { get; set; }
        [ForeignKey("UserId")] 
        public  AppUser AppUser { get; set; }
        [Required]                  
        public int Quantity { get; set; }

        [Required]                  
        public string CardId { get; set; }
        [ForeignKey("CardId")]  
        public virtual CardData CardData { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
