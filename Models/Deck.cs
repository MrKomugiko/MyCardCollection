using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCardCollection.Models
{
    public class Deck
    {
        public Deck(){
        }
        public Deck(string name, string owner)
        {
            this.Name = name;
            this.AppUserId = owner;
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CardsNumber { get; set; } = 0;
        public float TotalValue { get; set; } = 0;
        public bool IsValid { get; set; } = false;
        public DateTime Created { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime? Updated { get; set; } = DateTime.Now.ToUniversalTime();


        public string? AppUserId { get; set; }
        [ForeignKey("AppUserId")] public AppUser? AppUser { get; set; }

        public ICollection<DecksCollection> Content { get; set; }
    }
}
