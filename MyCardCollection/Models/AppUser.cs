using Microsoft.AspNetCore.Identity;

namespace MyCardCollection.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; } = "Kamil";
        public string Lastname { get; set; } = "Mąka";
        public string City { get; set; } = "Gdańsk";
        public string CountryCode { get; set; } = "PL";

        public int UniqueCards { get; set; } = 0;
        public int TotalCards { get; set; } = 0;
        public float TotalValue { get; set; } = 0.0f;
        public int DecksCreated { get; set; } = 0;

        public ICollection<Deck> Decks { get; set; }    
        public ICollection<CardsCollection> Cards { get; set; }
    }
}
