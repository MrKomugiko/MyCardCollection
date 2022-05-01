using Microsoft.AspNetCore.Identity;

namespace MyCardCollection.Models
{
    public class AppUser : IdentityUser
    {
        // config
        public string? BackgroundProfileImage { get; set; } = "https://c1.scryfall.com/file/scryfall-cards/art_crop/front/3/e/3eeef24b-b937-408e-a32e-1a546d3e7b0f.jpg?1634346590";
        public string? AvatarImage { get; set; } = "https://c1.scryfall.com/file/scryfall-cards/art_crop/front/3/e/3eeef24b-b937-408e-a32e-1a546d3e7b0f.jpg?1634346590";
        public string? Description { get; set; } 

        public string? Name { get; set; }
        public string? Lastname { get; set; } 
        public string? City { get; set; }
        public string? CountryCode { get; set; }

        public int UniqueCards { get; set; } = 0;
        public int TotalCards { get; set; } = 0;
        public float TotalValue { get; set; } = 0.0f;
        public int DecksCreated { get; set; } = 0;

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public ICollection<Deck> Decks { get; set; }    
        public ICollection<CardsCollection> Cards { get; set; }
    }
}
