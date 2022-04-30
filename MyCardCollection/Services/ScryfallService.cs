using Microsoft.Extensions.Caching.Memory;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using MyCardCollection.Services.Scryfall;
using MyCardCollection.Services.Scryfall.Card;
using MyCardCollection.ViewModel;
using Newtonsoft.Json;

namespace MyCardCollection.Services
{
    public class ScryfallService : IScryfallService
    {
        private readonly IMemoryCache _memoryCache;
        public ScryfallService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<Root> FindCard(string set = "mid", int number = 305)
        {
            Root responseData = new Root();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.scryfall.com");

                var response = await client.GetAsync($"/cards/{set}/{number}");
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsStringAsync().Result;
                    responseData = JsonConvert.DeserializeObject<Root>(readTask);
                }
            }

            return responseData;
        }

        public async Task<Root> FindCard(string id)
        {
            Root responseData = new Root();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.scryfall.com");

                var response = await client.GetAsync($"/cards/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsStringAsync().Result;
                    responseData = JsonConvert.DeserializeObject<Root>(readTask);
                }
            }

            return responseData;
        }

        public async Task<IEnumerable<CardData>> GetCardsListBySet(string set)
        {
            CardList responseData = new CardList();
            List<CardData> cards_content = new List<CardData>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.scryfall.com");
                int page = 1;
                while (true)
                {
                    var response = await client.GetAsync($"cards/search?format=json&include_extras=false&include_multilingual=false&order=collector_number&page={page}&q=set={set}&unique=cards");
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = response.Content.ReadAsStringAsync().Result;
                        responseData = JsonConvert.DeserializeObject<CardList>(readTask);
                        cards_content.AddRange(responseData.data.Select(x => new CardData(x)));
                    }
                    page++;
                    if (responseData.has_more == false)
                        break;
                }
            }

            return cards_content;
        }

        public async Task<IEnumerable<SetData>> GetSetsList()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.scryfall.com");

                var response = await client.GetAsync($"/sets");
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<SetsList>(readTask).data.Where(x => x.set_type == "expansion" || x.set_type == "core" );
                }
            }
            return null;
        }

        public async Task CacheAllSetsDataCards(List<SetListViewModel> listdata)
        {
            if(!_memoryCache.TryGetValue(listdata.First().setcode, out var cards))
            {
                // check if first set is loaded
                MemoryCacheEntryOptions _cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    Priority = CacheItemPriority.Low,
                    SlidingExpiration = TimeSpan.FromSeconds(3600)
                };
                foreach(var set in listdata)
                {
                    if (_memoryCache.TryGetValue(set.setcode, out var c))
                    {
                        return;
                        // all sets loaded before
                    }

                    var setCards = await GetCardsListBySet(set.setcode);
                    _memoryCache.Set(set.setcode, setCards, _cacheExpiryOptions);
                    Console.WriteLine(set.setcode+" SAVED IN MEMORY");
                }
            }
        }
    }
}
