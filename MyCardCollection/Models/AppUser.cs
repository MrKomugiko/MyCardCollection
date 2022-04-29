using Microsoft.AspNetCore.Identity;

namespace MyCardCollection.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; } = "";
        public string Lastname { get; set; } = "";
        public string City { get; set; } = "";
        public string CountryCode { get; set; } = "";

        public int UniqueCards { get; set; } = 0;
        public int TotalCards { get; set; } = 0;
        public float TotalValue { get; set; } = 0.0f;
        public int DecksCreated { get; set; } = 0;

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public ICollection<Deck> Decks { get; set; }    
        public ICollection<CardsCollection> Cards { get; set; }
    }
}
