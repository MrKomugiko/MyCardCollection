using Microsoft.AspNetCore.Identity;

namespace MyCardCollection.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Deck> Decks { get; set; }
    }
}
