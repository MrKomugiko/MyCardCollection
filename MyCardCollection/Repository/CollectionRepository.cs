using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyCardCollection.Data;
using MyCardCollection.Models;
using MyCardCollection.Repository;

namespace MyCardCollection.Services
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public CollectionRepository(ApplicationDbContext context, ICacheService cacheService)
        {
            this._context = context;
            _cacheService = cacheService;
        }

        //public async Task<CollectionStatistic> GetFullStatistics_v2(string _userId)
        //{
        //    var fullData = context.Collection.Where(x => x.UserId == _userId);

        //    Dictionary<string, int> setsdict = await fullData
        //        .GroupBy(x => x.CardData.SetCode)
        //        .Select(g => new
        //        {
        //            g.Key,
        //            SUM = g.Sum(s => s.Quantity)
        //        })
        //        .ToDictionaryAsync(k => k.Key, v => v.SUM);

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

        public async Task ClearCollectionAsync(string collectionOwnerId)
        {
            var deletedCards = _context.Collection.Where(x => x.UserId == collectionOwnerId);
            var userDecks = _context.DecksCollections.Where(x => x.UserId == collectionOwnerId);

            _context.RemoveRange(userDecks.Where(x => deletedCards.Any(c => c.CardId == x.CardId)));

            _context.Collection.RemoveRange(deletedCards);

            foreach (var deckName in userDecks.Include(x => x.Deck).Select(x => x.Deck.Name).Distinct())
            {
                var selected_deck_cacheKey = collectionOwnerId + "Deck" + deckName;

                if (_cacheService.TryGetValue(selected_deck_cacheKey, out List<CardsCollection> deckcards))
                {
                    _cacheService.Remove(selected_deck_cacheKey);
                }
            }

            var cacheKey = collectionOwnerId + "Collection";

            if (_cacheService.TryGetValue(cacheKey, out List<CardsCollection> cachedAllCards))
            {
                _cacheService.Remove(cacheKey);
            }

            await _context.SaveChangesAsync();
        }
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
            var cacheKey = collectionOwnerId + "Collection";
            //checks if cache entries exists
            if (!_cacheService.TryGetValue(cacheKey, out List<CardsCollection> cachedAllCards))
            {
                //calling the server
                List<CardsCollection> allCardsInCollection = await _context.Collection
                    .Where(x => x.UserId == collectionOwnerId)
                    .Include(x => x.CardData)
                    .ToListAsync();

                //setting cache entries
                _cacheService.Set(cacheKey, allCardsInCollection);
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

        public async Task<CardData> Get(string set, int number)
        {
            return await _context.CardsDatabase.FirstOrDefaultAsync(x => x.SetCode == set && x.CollectionNumber == number);
        }

        public async Task<List<CardsCollection>> GetCardsFromCollection(string _userId) =>
            await _context.Collection
                   .Where(x => x.UserId == _userId)
                   .Include(x => x.CardData)
                   .ToListAsync();

        public async Task<Dictionary<string,int>> GetSetCardCountGroupped(string _userId)
        {
            string cacheKey = _userId + "Collection";
            if (! _cacheService.TryGetValue<List<CardsCollection>> (cacheKey, out var data))
            {
                data = await GetCardsFromCollection(_userId);
                _cacheService.Set(cacheKey, data);
            }

            return data.GroupBy(x => x.CardData.SetCode)
                .Select(group => new {
                    Key = group.Key,
                    Value = group.Count()
                })
                .ToDictionary(x=>x.Key, x=>x.Value);
        }

        public async Task<IEnumerable<string>> GetCollectedCardIdFromSet(string _userId, string set)
        {
            string cacheKey = _userId + "Collection";
            if (!_cacheService.TryGetValue<List<CardsCollection>>(cacheKey, out var data))
            {
                data = await GetCardsFromCollection(_userId);
                _cacheService.Set(cacheKey, data);
            }
            return data.Where(x=>x.CardData.SetCode == set)
                .Select(x => x.CardId);
        }

        //-------------------------------------------------------------------



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

        //    public readonly string[] RarityNames = new string[4] { "common", "uncommon", "rare", "mythic" };
        //    public readonly string[] TypeNames = new string[7] { "Artifact", "Creature", "Enchantment", "Instant", "Land", "Planeswalker", "Sorcery" };

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
