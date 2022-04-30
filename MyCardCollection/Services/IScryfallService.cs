using MyCardCollection.Models;
using MyCardCollection.Services.Scryfall;
using MyCardCollection.Services.Scryfall.Card;
using MyCardCollection.ViewModel;

namespace MyCardCollection.Services
{
    public interface IScryfallService
    {
        Task<IEnumerable<CardData>> GetCardsListBySet(string set);
        Task<Root> FindCard(string set = "mid", int number = 305);
        Task<Root> FindCard(string id = null);
        Task<IEnumerable<SetData>> GetSetsList();
        Task CacheAllSetsDataCards(List<SetListViewModel> listdata);
    }
}