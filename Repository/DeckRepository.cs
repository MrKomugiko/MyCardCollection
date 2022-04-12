using Microsoft.EntityFrameworkCore;
using MyCardCollection.Controllers;
using MyCardCollection.Data;
using MyCardCollection.Models;

namespace MyCardCollection.Repository
{
    public class DeckRepository : IDeckRepository
    {
        private readonly ApplicationDbContext _context;
        public DeckRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Update(DeckModel deck, string _userId)
        {

            var currentDeckData = _context.DecksCollections
                .Where(x => x.UserId == _userId && x.DeckName == deck.deckName);

            List<DecksCollection> updatedDeckData = deck.cardInfos
                .Select(card => new DecksCollection(_userId, card.cardId, card.quantity, deck.deckName))
                    .ToList();

            _context.RemoveRange(currentDeckData);
            await _context.AddRangeAsync(updatedDeckData);
            await _context.SaveChangesAsync();
        }

        public async Task<string[]> GetUserDecks(string userId) =>
           await _context.DecksCollections
                .Where(x => x.UserId == userId)
                .Select(x => x.DeckName)
                .Distinct()
                .ToArrayAsync();

    }
}
