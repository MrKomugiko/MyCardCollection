using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCardCollection.Models
{
    public class Deck
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CardsNumber { get; set; }
        public int TotalValue { get; set; }
        public bool IsValid { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        [ForeignKey("Owner")]
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

    }
}
