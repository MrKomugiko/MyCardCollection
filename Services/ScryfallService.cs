using MyCardCollection.Services.Scryfall;
using MyCardCollection.Services.Scryfall.Card;
using Newtonsoft.Json;

namespace MyCardCollection.Services
{
    public class ScryfallService : IScryfallService
    {
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

        public async Task<List<Root>> GetCardsListBySet(string set)
        {
            CardList responseData = new CardList();
            List<Root> cardList = new List<Root>();

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
                        cardList.AddRange(responseData.data);
                    }
                    page++;
                    if (responseData.has_more == false)
                        break;
                }
            }

            return cardList.OrderBy(x => x.collector_number.PadLeft(4, '0')).ToList();
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
                    return JsonConvert.DeserializeObject<SetsList>(readTask).data.Where(x => x.set_type == "expansion");
                }
            }
            return null;
        }
    }
}
