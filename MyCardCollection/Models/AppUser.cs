using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCardCollection.Models
{
    public class AppUser : IdentityUser
    {
        // config
        public string? BackgroundProfileImage { get; set; } = "https://c1.scryfall.com/file/scryfall-cards/art_crop/front/3/e/3eeef24b-b937-408e-a32e-1a546d3e7b0f.jpg?1634346590";
        public string? AvatarImage { get; set; } = "https://c1.scryfall.com/file/scryfall-cards/art_crop/front/3/e/3eeef24b-b937-408e-a32e-1a546d3e7b0f.jpg?1634346590";

        public string? Name { get; set; }
        public string? Lastname { get; set; }
        public string? City { get; set; }
        public string? CountryCode { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }

        public int UniqueCards { get; set; } = 0;
        public int TotalCards { get; set; } = 0;
        public float TotalValue { get; set; } = 0.0f;
        public int DecksCreated { get; set; } = 0;


        public virtual PrivacySettings PrivacySettings { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public ICollection<Deck> Decks { get; set; }
        public ICollection<CardsCollection> Cards { get; set; }


        public string? GetLocationString()
        {
            bool haveCity = !String.IsNullOrWhiteSpace(this.City);
            bool haveCountry = !String.IsNullOrWhiteSpace(this.CountryCode);
            bool allowCity = this.PrivacySettings.AllowCity;
            bool allowCountry = this.PrivacySettings.AllowCountry;

            string? result = (haveCity, haveCountry, allowCity, allowCountry) switch
            {
                (true, true, true, true) => $"{this.City}, {this.CountryCode}",  // Gdańsk, Poland
                (true, false, true, _) => $"{this.City}",                      // Gdańsk
                (true, _, true, false) => $"{this.City}",                      // Gdańsk
                (false, true, _, true) => $"{this.CountryCode}",               // Poland
                (_, true, false, true) => $"{this.CountryCode}",               // Poland
                (_, _, _, _) => null
            };
            return result;
        }
    }
}
