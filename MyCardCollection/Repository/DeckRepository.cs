using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyCardCollection.Controllers;
using MyCardCollection.Data;
using MyCardCollection.Models;
using MyCardCollection.ViewModel;

namespace MyCardCollection.Repository
{
    public class DeckRepository : IDeckRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private MemoryCacheEntryOptions cacheExpiryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddSeconds(3600),
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromSeconds(3600)
        };
        public DeckRepository(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
       
        public async Task<(List<CardsCollection> cardsOnPage, int totalMatches)> SearchCardsFromDeck(string collectionOwnerId, string? searchQuery, int page = 1, int itemsPerPage = 10, int? deckId = null)
        {
            List<CardsCollection> allCardsInDeck = new();
            List<CardsCollection> cardsInDeck = new();

            int pageSize = itemsPerPage;
            searchQuery = searchQuery?.ToLower();

            if (deckId == null)
                allCardsInDeck = await GetAll_SearchCardsFromDeck(collectionOwnerId, searchQuery, deckId);
            else
                allCardsInDeck = await GetAll_SearchCardsFromDeck(collectionOwnerId, searchQuery, deckId);

            int totalMatches = allCardsInDeck.Count;

            cardsInDeck = allCardsInDeck
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToList();

            return (cardsInDeck, totalMatches);
        }
        public async Task<List<CardsCollection>> GetAll_SearchCardsFromDeck(string collectionOwnerId, string? searchQuery, int? deckId)
        {
            List<CardsCollection> cardInDeck = new();

            var deck_cacheKey = collectionOwnerId + "Deck" + deckId;

            if (!_memoryCache.TryGetValue(deck_cacheKey, out List<CardsCollection> cachedAllCardsFromDeck))
            {
                cachedAllCardsFromDeck = await GetDeckDataFromDatabase(collectionOwnerId, deckId);

                _memoryCache.Set(deck_cacheKey, cachedAllCardsFromDeck, cacheExpiryOptions);
            }
            cardInDeck = cachedAllCardsFromDeck;
          
            searchQuery = searchQuery == null ? null : searchQuery.ToLower();

            if (searchQuery == null)
            {
                return cardInDeck;
            }
            else
            {
                return cardInDeck
                    .Where(x => x.CardData.Name.ToLower().Contains(searchQuery) ||
                        x.CardData.Rarity.ToLower().Contains(searchQuery) ||
                        x.CardData.Type.ToLower().Contains(searchQuery))
                    .ToList();
            }
        }
        public async Task<List<CardsCollection>> GetDeckDataFromDatabase(string collectionOwnerId, int? deckId)
        {
            List<CardsCollection> cachedAllCardsFromDeck = new();
            await _context.DecksCollections
                .Where(x => x.UserId == collectionOwnerId && x.DeckId == deckId)
                .Include(x => x.CardData)
                .ForEachAsync(x =>
                {
                    var cc = new CardsCollection(x.UserId, x.CardId, x.Quantity);
                    cc.CardData = x.CardData;
                    cachedAllCardsFromDeck.Add(cc);
                });

            return cachedAllCardsFromDeck;
        }
        public async Task<bool> CreateNewDeck(string decktitle, string _userId)
        {
            Deck deck = new Deck(name:decktitle, owner:_userId);

            await _context.Decks.AddAsync(deck);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> Update(DeckModel deck, string _userId)
        {
            var currentDeckData = _context.DecksCollections
                .Where(x => x.UserId == _userId && x.DeckId == deck.deckId);

            List<DecksCollection> updatedDeckData = deck.cardInfos
                .Select(card => new DecksCollection(_userId, card.cardId, card.quantity, deck.deckId))
                    .ToList();

            _context.RemoveRange(currentDeckData);
            await _context.AddRangeAsync(updatedDeckData);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateSingle(DeckEditViewModel editdeck)
        {
           
            var deck = await _context.Decks.FirstOrDefaultAsync(x=>x.Id == editdeck.DeckId);
            if (deck == null) return false;

            deck.Name = editdeck.Name;
            deck.BackgroundImage = editdeck.BackgroundImage;
            deck.Description = editdeck.Description;
            deck.Updated = DateTime.Now.ToUniversalTime();

            if(await _context.SaveChangesAsync() >= 0)
            {
                return true;
            }
            return false;
        }

        public async Task<Dictionary<int,string>> GetDeckNames(string userId) => 
            await _context.Decks
                .Where(x => x.AppUserId == userId)
                .Distinct()
                .ToDictionaryAsync(x => x.Id, x => x.Name);
        public async Task ClearDeck(int deckId, string userId)
        {
            _context.DecksCollections.RemoveRange(_context.DecksCollections
                .Where(x => x.UserId == userId && x.DeckId == deckId));
            await _context.SaveChangesAsync();
        }
        public async Task<List<Deck>> GetUserDecksWithoutContent(string userId) => 
            await _context.Decks.Where(x => x.AppUserId == userId)
                .AsNoTracking().ToListAsync();
        public async Task<List<Deck>> GetUserDecks(string userId) =>
           await _context.Decks.Where(x => x.AppUserId == userId)
            .Include(x=>x.Content)
                .ThenInclude(card => card.CardData)
            .AsNoTracking()
            .ToListAsync();


        // memory, objects manimulation, no impact to db
        public (string cardId, int? updatedQuantity, int? cardLeftInCollection, string response) UpdateQuantityFromDeck(string userId, string id, int deckId, int qtChange)
        {
            var all_cacheKey = userId + "Collection";
            var deck_cacheKey = userId + "Deck" + deckId;

            List<CardsCollection> memoryCollection = null;
            List<CardsCollection> memoryDeck = null;
            _memoryCache.TryGetValue<List<CardsCollection>>(all_cacheKey, out memoryCollection);
            _memoryCache.TryGetValue<List<CardsCollection>>(deck_cacheKey, out memoryDeck);

            CardsCollection cardFromDeck = memoryDeck.SingleOrDefault(x => x.CardId == id);

            if (cardFromDeck == null)
            {
                return (id, null, null, "Not found this card in deck.");
            }

            if ((cardFromDeck.Quantity == 4 || memoryDeck.Where(x => x.CardData.Name.Contains(cardFromDeck.CardData.Name)).Sum(x => x.Quantity) == 4) && !cardFromDeck.CardData.Type.Contains("Land") && qtChange > 0)
            {
                // nie Landy stackują sie max do 4 szt
                return (cardFromDeck.CardId, null, null, "Maximum card(non land) stacks is 4.");
            }
            else if (cardFromDeck.Quantity == 0 && qtChange < 0)
            {
                return (cardFromDeck.CardId, null, null, "Your deck cannot contain negative numbers of card.");
            }
            else if (cardFromDeck.Quantity < Math.Abs(qtChange) && qtChange < 1)
            {
                // zmiana qtchange na maksymalna dozwolona wartosc
                qtChange = cardFromDeck.Quantity;
                // usunięcie całkowite karty z decku
                cardFromDeck.Quantity -= qtChange;
            }
            else
            {
                cardFromDeck.Quantity += qtChange;
            }

            if (cardFromDeck.Quantity == 0)
            {
                memoryDeck.Remove(cardFromDeck);
            }

            CardsCollection cardFromCollection = memoryCollection.SingleOrDefault(x => x.CardId == id);

            if (cardFromCollection != null)
            {
                // in this case = workin on cached data its no problem to show quantity below 0
                /// just for information purpose
                cardFromCollection.Quantity -= qtChange;
            }

            _memoryCache.Set(all_cacheKey, memoryCollection, cacheExpiryOptions);
            _memoryCache.Set(deck_cacheKey, memoryDeck, cacheExpiryOptions);

            return (cardFromDeck.CardId, cardFromDeck.Quantity, cardFromCollection.Quantity, "Ok");

        }
        private bool TryAddCardToStack(List<CardsCollection> _cardStack, CardsCollection _card)
        {
            var existingCopyInDeck = _cardStack.SingleOrDefault(x => x.CardId == _card.CardId);
            if (existingCopyInDeck == null)
            {
                _cardStack.Add(_card);
            }
            else
            {
                var currentQty = _cardStack
                    .Where(x => x.CardData.Name == _card.CardData.Name)
                    .Sum(x => x.Quantity);

                // more than 4 cards supplication in deck, this card is not a land, cant be stacked > 4x
                if (currentQty == 4 && !_card.CardData.Type.Contains("Land"))
                {
                    return false;
                }

                existingCopyInDeck.Quantity += 1;
            }

            return true;
        }
        public async Task AddCardToDeckAsync(string collectionOwnerId, string cardId, int deckId)
            {
                List<CardsCollection> cardsInDeck = new();
                var deck_cacheKey = collectionOwnerId + "Deck" + deckId;
                var all_cacheKey = collectionOwnerId + "Collection";

                var allCards = _memoryCache.Get<List<CardsCollection>>(all_cacheKey);
                var cardToAdd = allCards.SingleOrDefault(x => x.CardId == cardId).Clone() as CardsCollection;
                cardToAdd.Quantity = 1;

                if (!_memoryCache.TryGetValue(deck_cacheKey, out List<CardsCollection> cachedCurrentDeck))
                {
                    cardsInDeck = await GetDeckDataFromDatabase(collectionOwnerId, deckId);
                }
                else
                {
                    // istnieje juz jakis aktualnie tworzony deck = AKTUALIZACJA 
                    cardsInDeck = _memoryCache.Get<List<CardsCollection>>(deck_cacheKey);
                }

                if (TryAddCardToStack(cardsInDeck, cardToAdd) == false)
                    return;

                allCards.First(x => x.CardId == cardId).Quantity -= 1;

                _memoryCache.Set(all_cacheKey, allCards, cacheExpiryOptions);
                _memoryCache.Set(deck_cacheKey, cardsInDeck, cacheExpiryOptions);

      
            }
            public async Task<Deck> GetDeckById(int deckId)
            {
            var deck = _context.Decks.Where(x => x.Id == deckId)
                .Include(x => x.Content)
                    .ThenInclude(x=>x.CardData)
                .AsNoTracking()
                .FirstOrDefault();

                if(deck == null) return null;

                return deck;
            }
    }
}
