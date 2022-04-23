
using Microsoft.AspNetCore.Mvc.Rendering;
using MyCardCollection.Models;
using static MyCardCollection.Services.CollectionRepository;

namespace MyCardCollection.Services
{
    public interface ICollectionRepository
    {
        //Task<CollectionStatistic> GetFullStatistics_v2(string _userId);
        
        Task<List<CardsCollection>> GetAll_SearchCardsFromCollection(string collectionOwnerId, string? searchQuery = null);
        Task<(List<CardsCollection> cardsOnPage, int totalMatches)> SearchCardsFromCollection(string collectionOwnerId, string? searchQuery=null, int page = 1, int itemsPerPage = 10);
        Task ClearCollectionAsync(string collectionOwnerId);
        Task<CardData> Get(string set, int number);
        Task<List<CardsCollection>> GetCardsFromCollection(string _userId);
        Task<Dictionary<string, int>> GetSetCardCountGroupped(string _userId);
    }
}