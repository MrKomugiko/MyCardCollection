using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyCardCollection.Data;
using MyCardCollection.Models;

namespace MyCardCollection.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ApplicationDbContext context;
        private readonly IMemoryCache _memoryCache;
        public CollectionService(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            this.context = context;
            _memoryCache = memoryCache;
        }

        //public async Task<CollectionStatistic> GetFullStatistics_v2(string _userId)
        //{
        //    var fullData = context.Collection.Where(x => x.UserId == _userId);

        //    Dictionary<string, int> setsdict = await fullData
        //        .GroupBy(x => x.CardData.SetCode)
        //        .Select(g => new {
        //            g.Key,
        //            SUM = g.Sum(s => s.Quantity)
        //         })
        //        .ToDictionaryAsync(k=>k.Key, v=>v.SUM);

        //    var rawModel = await context.Collection_Details_Stats
        //        .FromSqlRaw("EXEC GetUserCollectionStatistics @_userId = {0}", _userId).ToListAsync();

        //    CollectionDetails models = rawModel.FirstOrDefault();

        //    var data = new CollectionStatistic()
        //    {
        //        CardCount = fullData.Sum(x => x.Quantity),
        //        DisctinctCardCount = fullData.Count(),
        //        RarityCount = new RarityTypes()
        //        {
        //            common = models.Common_Count ?? 0,
        //            uncommon = models.Uncommon_Count ?? 0,
        //            rare = models.Rare_Count ?? 0,
        //            mythic = models.Mythic_Count ?? 0
        //        },
        //        MainCardTypesCount = new MainCardTypes()
        //        {
        //            Creature = models.Creature_Count ?? 0,
        //            Instant = models.Instant_Count ?? 0,
        //            Land = models.Land_Count ?? 0,
        //            Artifact = models.Artifact_Count ?? 0,
        //            Enchantment = models.Enchantment_Count ?? 0,
        //            Planeswalker = models.Planeswalker_Count ?? 0,
        //            Sorcery = models.Sorcery_Count ?? 0
        //        },
        //        SetDict = setsdict
        //    };

        //    return data;
        //}

        // -------------------------------------------------------------------
        public async Task<(List<CardsCollection> cardsOnPage, int totalMatches)> SearchCardsFromCollection(string collectionOwnerId, string? searchQuery, int page = 1, int itemsPerPage = 10)
        {
            List<CardsCollection> cardsInCollection = new();
            int pageSize = itemsPerPage;

            searchQuery = searchQuery?.ToLower();

            var allCardsInCollection = await GetAll_SearchCardsFromCollection(collectionOwnerId, searchQuery);
            
            cardsInCollection = allCardsInCollection
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToList(); ;

            int totalMatches = allCardsInCollection.Count;
        
            return (cardsInCollection, totalMatches);
        }
        public async Task<List<CardsCollection>> GetAll_SearchCardsFromCollection(string collectionOwnerId, string? searchQuery)
        {
           
            var cacheKey = collectionOwnerId+"Collection";
            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheKey, out List<CardsCollection> cachedAllCards))
            {
                //calling the server
                List<CardsCollection> allCardsInCollection = await context.Collection
                    .Where(x=>x.UserId == collectionOwnerId)
                    .Include(x => x.CardData)
                    .ToListAsync();

                //setting up cache options
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(3600),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromSeconds(3600)
                };
                //setting cache entries
                _memoryCache.Set(cacheKey, allCardsInCollection, cacheExpiryOptions);
                cachedAllCards = allCardsInCollection;
            }

            searchQuery = searchQuery == null ? null : searchQuery.ToLower();

            if (searchQuery == null)
            {
                return cachedAllCards;
            }
            else
            {
                return cachedAllCards
                    .Where(x => x.CardData.Name.ToLower().Contains(searchQuery) ||
                        x.CardData.Rarity.ToLower().Contains(searchQuery) ||
                        x.CardData.Type.ToLower().Contains(searchQuery))
                    .ToList();
            }
        }

        // -------------------------------------------------------------------
        //public async Task AddCardToDeckAsync(string collectionOwnerId, string cardId, string? deckName = null)
        //{
        //    List<CardsCollection> cardsInDeck = new();
        //    var deck_cacheKey = collectionOwnerId + "DeckX";
        //    var selected_deck_cacheKey = collectionOwnerId + "Deck" + (deckName != null ? deckName.Trim() : "");
        //    var all_cacheKey = collectionOwnerId + "Collection";

        //    var allCards = _memoryCache.Get<List<CardsCollection>>(all_cacheKey);
        //    var cardToAdd = allCards.SingleOrDefault(x => x.CardId == cardId).Clone() as CardsCollection;
        //    cardToAdd.Quantity = 1;

        //    if (deckName != null)
        //    {
        //        deck_cacheKey = selected_deck_cacheKey;
        //    }

        //    if (!_memoryCache.TryGetValue(deck_cacheKey, out List<CardsCollection> cachedCurrentDeck))
        //    {
        //        if(deckName != null)
        //        {
        //            cardsInDeck = await GetDeckDataFromDatabase(collectionOwnerId, deckName);
        //        }
        //    }
        //    else
        //    {
        //        // istnieje juz jakis aktualnie tworzony deck = AKTUALIZACJA 
        //        cardsInDeck = _memoryCache.Get<List<CardsCollection>>(deck_cacheKey);
        //    }

        //    if (ValidateIncremention(cardsInDeck) == false) return;

        //    allCards.First(x => x.CardId == cardId).Quantity -= 1;
        //    var cacheExpiryOptions = new MemoryCacheEntryOptions
        //    {
        //        AbsoluteExpiration = DateTime.Now.AddSeconds(3600),
        //        Priority = CacheItemPriority.High,
        //        SlidingExpiration = TimeSpan.FromSeconds(3600)
        //    };

        //    _memoryCache.Set(all_cacheKey, allCards, cacheExpiryOptions);
        //    _memoryCache.Set(deck_cacheKey, cardsInDeck, cacheExpiryOptions);

        //    bool ValidateIncremention(List<CardsCollection> _cardsInDeck)
        //    {
        //        var existingCopyInDeck = _cardsInDeck.SingleOrDefault(x => x.CardId == cardToAdd.CardId);
        //        if (existingCopyInDeck == null)
        //        {
        //            _cardsInDeck.Add(cardToAdd);
        //        }
        //        else
        //        {
        //            var currentQty = _cardsInDeck.Where(x => x.CardData.Name == cardToAdd.CardData.Name).Sum(x => x.Quantity);
        //            if (currentQty == 4 && !cardToAdd.CardData.Type.Contains("Land"))
        //            {
        //                return false;
        //            }
        //            existingCopyInDeck.Quantity += 1;
        //        }
        //        return true;
        //    }
        //}
        //public async Task<(List<CardsCollection> cardsOnPage, int totalMatches)> SearchCardsFromDeck(string collectionOwnerId, string? searchQuery, int page = 1, int itemsPerPage = 10, string? deckName = null)
        //{
        //    List<CardsCollection> allCardsInDeck = new();
        //    List<CardsCollection> cardsInDeck = new();

        //    int pageSize = itemsPerPage;
        //    searchQuery = searchQuery?.ToLower();

        //    if (deckName == null)
        //        allCardsInDeck = await GetAll_SearchCardsFromDeck(collectionOwnerId, searchQuery, deckName);
        //    else
        //        allCardsInDeck = await GetAll_SearchCardsFromDeck(collectionOwnerId, searchQuery, deckName);
             
        //    int totalMatches = allCardsInDeck.Count;
        //        cardsInDeck = allCardsInDeck
        //            .Skip((page - 1) * pageSize)
        //            .Take(pageSize).ToList(); 
        //        return (cardsInDeck, totalMatches);
        //}
        //// jezeli deck = null to znaczy ze pracuje sie na tymczasowym deckiem nie zapisalo sie go jeszcze 
        //public async Task<List<CardsCollection>> GetAll_SearchCardsFromDeck(string collectionOwnerId, string? searchQuery, string? deckName = null)
        //{
        //    List<CardsCollection> cardInDeck = new();

        //    if (deckName == null)
        //    {
        //        var deck_cacheKey = collectionOwnerId + "DeckX";

        //        if (!_memoryCache.TryGetValue(deck_cacheKey, out List<CardsCollection> cachedAllCardsFromDeck))
        //        {
        //            cachedAllCardsFromDeck = new List<CardsCollection>();

        //            var cacheExpiryOptions = new MemoryCacheEntryOptions
        //            {
        //                AbsoluteExpiration = DateTime.Now.AddSeconds(3600),
        //                Priority = CacheItemPriority.High,
        //                SlidingExpiration = TimeSpan.FromSeconds(3600)
        //            };

        //            _memoryCache.Set(deck_cacheKey, cachedAllCardsFromDeck, cacheExpiryOptions);
        //        }

        //        cardInDeck = cachedAllCardsFromDeck;
        //    }
        //    else
        //    {
        //        var deck_cacheKey = collectionOwnerId + "Deck" + deckName.Trim();
                
        //        if (!_memoryCache.TryGetValue(deck_cacheKey, out List<CardsCollection> cachedAllCardsFromDeck))
        //        {
        //            cachedAllCardsFromDeck = await GetDeckDataFromDatabase(collectionOwnerId, deckName);

        //            var cacheExpiryOptions = new MemoryCacheEntryOptions
        //            {
        //                AbsoluteExpiration = DateTime.Now.AddSeconds(3600),
        //                Priority = CacheItemPriority.High,
        //                SlidingExpiration = TimeSpan.FromSeconds(3600)
        //            };

        //            _memoryCache.Set(deck_cacheKey, cachedAllCardsFromDeck, cacheExpiryOptions);
        //        }

        //        cardInDeck = cachedAllCardsFromDeck;
        //    }

        //    searchQuery = searchQuery == null ? null : searchQuery.ToLower();

        //    if (searchQuery == null)
        //    {
        //        return cardInDeck;
        //    }
        //    else
        //    {
        //        return cardInDeck
        //            .Where(x => x.CardData.Name.ToLower().Contains(searchQuery) ||
        //                x.CardData.Rarity.ToLower().Contains(searchQuery) ||
        //                x.CardData.Type.ToLower().Contains(searchQuery))
        //            .ToList();
        //    }
        //}
        //private async Task<List<CardsCollection>> GetDeckDataFromDatabase(string collectionOwnerId, string? deckName)
        //{
        //    List<CardsCollection> cachedAllCardsFromDeck = new();
        //    await context.DecksCollections
        //        .Where(x => x.UserId == collectionOwnerId && x.DeckName == deckName)
        //        .Include(x => x.CardData)
        //        .ForEachAsync(x =>
        //        {
        //            var cc = new CardsCollection(x.UserId, x.CardId, x.Quantity);
        //            cc.CardData = x.CardData;
        //            cachedAllCardsFromDeck.Add(cc);
        //        });

        //    return cachedAllCardsFromDeck;
        //}
        //public (string cardId, int? updatedQuantity, int? cardLeftInCollection, string response) UpdateQuantityFromDeck(string userId, string id, string? deckName, int qtChange)
        //{
        //    // get card by id from deck
        //    var deck_cacheKey = userId + "DeckX";
        //    var all_cacheKey = userId + "Collection";

        //    if (deckName != null)
        //    {
        //        deck_cacheKey = userId + "Deck" + deckName.Trim();
        //    }

        //    List<CardsCollection> cardsInCollection = null;
        //        List<CardsCollection> cardsInDeck = null;
        //        _memoryCache.TryGetValue<List<CardsCollection>>(all_cacheKey, out cardsInCollection);
        //        _memoryCache.TryGetValue<List<CardsCollection>>(deck_cacheKey, out cardsInDeck);

        //        CardsCollection cardFromDeck = cardsInDeck.SingleOrDefault(x => x.CardId == id);

        //        if (cardFromDeck == null)
        //        {
        //            return (id, null, null, "Not found this card in deck.");
        //        }
                
        //        if((cardFromDeck.Quantity == 4 || cardsInDeck.Where(x=>x.CardData.Name.Contains(cardFromDeck.CardData.Name)).Sum(x=>x.Quantity) == 4 ) && !cardFromDeck.CardData.Type.Contains("Land") && qtChange > 0)
        //        {
        //            // nie Landy stackują sie max do 4 szt
        //            return (cardFromDeck.CardId, null, null, "Maximum card(non land) stacks is 4.");
        //        }
        //        else if(cardFromDeck.Quantity == 0 && qtChange<0)
        //        {
        //            return (cardFromDeck.CardId, null, null, "Your deck cannot contain negative numbers of card.");
        //        }
        //        else if(cardFromDeck.Quantity < Math.Abs(qtChange) && qtChange<1)
        //        {
        //            // zmiana qtchange na maksymalna dozwolona wartosc
        //            qtChange = cardFromDeck.Quantity;
        //            // usunięcie całkowite karty z decku
        //            cardFromDeck.Quantity -= qtChange;
        //        }
        //        else
        //        {
        //            cardFromDeck.Quantity += qtChange;
        //        }

        //        if(cardFromDeck.Quantity == 0)
        //        {
        //            cardsInDeck.Remove(cardFromDeck);
        //        }

        //        CardsCollection cardFromCollection = cardsInCollection.SingleOrDefault(x => x.CardId == id);

        //        if (cardFromCollection != null)
        //        {
        //            // in this case = workin on cached data its no problem to show quantity below 0
        //            /// just for information purpose
        //            cardFromCollection.Quantity -= qtChange;
        //        }

        //        var cacheExpiryOptions = new MemoryCacheEntryOptions
        //        {
        //            AbsoluteExpiration = DateTime.Now.AddSeconds(3600),
        //            Priority = CacheItemPriority.High,
        //            SlidingExpiration = TimeSpan.FromSeconds(3600)
        //        };
        //        _memoryCache.Set(all_cacheKey, cardsInCollection, cacheExpiryOptions);
        //        _memoryCache.Set(deck_cacheKey, cardsInDeck, cacheExpiryOptions);

        //        return (cardFromDeck.CardId,cardFromDeck.Quantity, cardFromCollection.Quantity, "Ok");

        //}
        //public async Task<List<SelectListItem>> GetPlayerDecksNames(string userId)
        //{
        //    var x = context.DecksCollections.Where(x => x.UserId == userId).Select(x => x.DeckName).Distinct().ToList();
        //    List<SelectListItem> data = new();
            
        //    data.Add(new SelectListItem { Value = null, Text = "- not selected -" });
        //    x.ForEach(x=>data.Add(new SelectListItem { Value=x.ToString(), Text=x.ToString() }));

        //    return data;
        //}

        //[Keyless]
        //public class CollectionDetails
        //{
        //    public int? Enchantment_Count { get; set; }
        //    public int? Instant_Count { get; set; }
        //    public int? Land_Count { get; set; }
        //    public int? Creature_Count { get; set; }
        //    public int? Planeswalker_Count { get; set; }
        //    public int? Sorcery_Count { get; set; }
        //    public int? Artifact_Count { get; set; }
        //    public int? Common_Count { get; set; }
        //    public int? Uncommon_Count { get; set; }
        //    public int? Rare_Count { get; set; }
        //    public int? Mythic_Count { get; set; }

        //}
        //public class CollectionStatistic
        //{
        //    public int CardCount { get; set; }
        //    public int DisctinctCardCount { get; set; }
        //    public RarityTypes RarityCount { get; set; }
        //    public MainCardTypes MainCardTypesCount { get; set; }
        //    public Dictionary<string, int> SetDict { get; internal set; }

        //    public readonly string[] RarityNames = new string [4]{ "common", "uncommon", "rare", "mythic" };
        //    public readonly string[] TypeNames = new string[7] { "Artifact", "Creature", "Enchantment","Instant","Land","Planeswalker","Sorcery" };

        //    public class RarityTypes
        //    {
        //        public int common { get; set; } = 0;
        //        public int uncommon { get; set; } = 0;
        //        public int rare { get; set; } = 0;
        //        public int mythic { get; set; } = 0;
        //    }
        //    public class MainCardTypes
        //    {
        //        public int Creature { get; set; } = 0;
        //        public int Instant { get; set; } = 0;
        //        public int Land { get; set; } = 0;
        //        public int Artifact { get; set; } = 0;
        //        public int Enchantment { get; set; } = 0;
        //        public int Planeswalker { get; set; } = 0;
        //        public int Sorcery { get; set; } = 0;
        //    }
        //}
        //public enum CardTypesEnum
        //{
        //    Creature,
        //    Instant,
        //    Land,
        //    Artifact,
        //    Enchantment,
        //    Sorcery,
        //    Planeswalker
        //}
    }
}
