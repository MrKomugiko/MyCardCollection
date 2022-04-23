using MyCardCollection.Services.Scryfall;
using MyCardCollection.Services.Scryfall.Card;

namespace MyCardCollection.Services
{
    public interface IScryfallService
    {
        Task<List<Root>> GetCardsListBySet(string set);
        Task<Root> FindCard(string set = "mid", int number = 305);
        Task<Root> FindCard(string id = null);
        Task<IEnumerable<SetData>> GetSetsList();
    }
}