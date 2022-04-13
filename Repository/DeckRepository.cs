using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyCardCollection.Controllers;
using MyCardCollection.Data;
using MyCardCollection.Models;

namespace MyCardCollection.Repository
{
    public class DeckRepository : IDeckRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        public DeckRepository(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        // Manipulate data in memory, dont change database ~ UI purpose
        public async Task AddCardToDeckAsync(string collectionOwnerId, string cardId, string deckName)
        {
            List<CardsCollection> cardsInDeck = new();
            var deck_cacheKey = collectionOwnerId + "Deck" + deckName;
            var all_cacheKey = collectionOwnerId + "Collection";

            var allCards = _memoryCache.Get<List<CardsCollection>>(all_cacheKey);
            var cardToAdd = allCards.SingleOrDefault(x => x.CardId == cardId).Clone() as CardsCollection;
            cardToAdd.Quantity = 1;

            if (!_memoryCache.TryGetValue(deck_cacheKey, out List<CardsCollection> cachedCurrentDeck))
            {
                cardsInDeck = await GetDeckDataFromDatabase(collectionOwnerId, deckName);
            }
            else
            {
                // istnieje juz jakis aktualnie tworzony deck = AKTUALIZACJA 
                cardsInDeck = _memoryCache.Get<List<CardsCollection>>(deck_cacheKey);
            }

            if (TryAddCardToStack(cardsInDeck, cardToAdd) == false)
                return;

            allCards.First(x => x.CardId == cardId).Quantity -= 1;
            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(3600),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromSeconds(3600)
            };

            _memoryCache.Set(all_cacheKey, allCards, cacheExpiryOptions);
            _memoryCache.Set(deck_cacheKey, cardsInDeck, cacheExpiryOptions);

      
        }
       
        public async Task<(List<CardsCollection> cardsOnPage, int totalMatches)> SearchCardsFromDeck(string collectionOwnerId, string? searchQuery, int page = 1, int itemsPerPage = 10, string? deckName = null)
        {
            List<CardsCollection> allCardsInDeck = new();
            List<CardsCollection> cardsInDeck = new();

            int pageSize = itemsPerPage;
            searchQuery = searchQuery?.ToLower();

            if (deckName == null)
                allCardsInDeck = await GetAll_SearchCardsFromDeck(collectionOwnerId, searchQuery, deckName);
            else
                allCardsInDeck = await GetAll_SearchCardsFromDeck(collectionOwnerId, searchQuery, deckName);

            int totalMatches = allCardsInDeck.Count;
            cardsInDeck = allCardsInDeck
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToList();
            return (cardsInDeck, totalMatches);
        }
        // jezeli deck = null to znaczy ze pracuje sie na tymczasowym deckiem nie zapisalo sie go jeszcze 
        public async Task<List<CardsCollection>> GetAll_SearchCardsFromDeck(string collectionOwnerId, string? searchQuery, string? deckName = null)
        {
            List<CardsCollection> cardInDeck = new();


            //if (deckName == null)
            //{
            //    var deck_cacheKey = collectionOwnerId + "Deck"+deckName;

            //    if (!_memoryCache.TryGetValue(deck_cacheKey, out List<CardsCollection> cachedAllCardsFromDeck))
            //    {
            //        cachedAllCardsFromDeck = new List<CardsCollection>();

            //        var cacheExpiryOptions = new MemoryCacheEntryOptions
            //        {
            //            AbsoluteExpiration = DateTime.Now.AddSeconds(3600),
            //            Priority = CacheItemPriority.High,
            //            SlidingExpiration = TimeSpan.FromSeconds(3600)
            //        };

            //        _memoryCache.Set(deck_cacheKey, cachedAllCardsFromDeck, cacheExpiryOptions);
            //    }

            //    cardInDeck = cachedAllCardsFromDeck;
            //}
            //else
            //{
                var deck_cacheKey = collectionOwnerId + "Deck" + deckName.Trim();

                if (!_memoryCache.TryGetValue(deck_cacheKey, out List<CardsCollection> cachedAllCardsFromDeck))
                {
                    cachedAllCardsFromDeck = await GetDeckDataFromDatabase(collectionOwnerId, deckName);

                    var cacheExpiryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddSeconds(3600),
                        Priority = CacheItemPriority.High,
                        SlidingExpiration = TimeSpan.FromSeconds(3600)
                    };

                    _memoryCache.Set(deck_cacheKey, cachedAllCardsFromDeck, cacheExpiryOptions);
                }

                cardInDeck = cachedAllCardsFromDeck;
            //}

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
        public async Task<List<CardsCollection>> GetDeckDataFromDatabase(string collectionOwnerId, string? deckName)
        {
            List<CardsCollection> cachedAllCardsFromDeck = new();
            await _context.DecksCollections
                .Where(x => x.UserId == collectionOwnerId && x.DeckName == deckName)
                .Include(x => x.CardData)
                .ForEachAsync(x =>
                {
                    var cc = new CardsCollection(x.UserId, x.CardId, x.Quantity);
                    cc.CardData = x.CardData;
                    cachedAllCardsFromDeck.Add(cc);
                });

            return cachedAllCardsFromDeck;
        }
        
        public (string cardId, int? updatedQuantity, int? cardLeftInCollection, string response) UpdateQuantityFromDeck(string userId, string id, string deckName, int qtChange)
        {
            // get card by id from deck

            var all_cacheKey = userId + "Collection";

            var deck_cacheKey = userId + "Deck" + deckName.Trim();
            

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

            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(3600),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromSeconds(3600)
            };
            _memoryCache.Set(all_cacheKey, memoryCollection, cacheExpiryOptions);
            _memoryCache.Set(deck_cacheKey, memoryDeck, cacheExpiryOptions);

            return (cardFromDeck.CardId, cardFromDeck.Quantity, cardFromCollection.Quantity, "Ok");

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
        public async Task<string[]> GetDeckNames(string userId) =>
           await _context.DecksCollections
                .Where(x => x.UserId == userId)
                .Select(x => x.DeckName)
                .Distinct()
                .ToArrayAsync();

        // Trying to add card into collection, adding or changing quantity if possible
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

        public async Task ClearDeck(string currentDeck, string userId)
        {
            _context.DecksCollections.RemoveRange(_context.DecksCollections.Where(x => x.UserId == userId && x.DeckName == currentDeck));
            await _context.SaveChangesAsync();
        }
    }
}
