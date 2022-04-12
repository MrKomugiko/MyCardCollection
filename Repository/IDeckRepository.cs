using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Controllers;

namespace MyCardCollection.Repository
{
    public interface IDeckRepository
    {
        Task Update(DeckModel deck, string userId);
        Task<string[]> GetUserDecks(string userId);
    }
}
