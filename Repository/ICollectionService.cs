
using Microsoft.AspNetCore.Mvc.Rendering;
using MyCardCollection.Models;
using static MyCardCollection.Services.CollectionService;

namespace MyCardCollection.Services
{
    public interface ICollectionService
    {
        //Task<CollectionStatistic> GetFullStatistics_v2(string _userId);
        
        Task<List<CardsCollection>> GetAll_SearchCardsFromCollection(string collectionOwnerId, string? searchQuery = null);
        Task<List<CardsCollection>> GetAll_SearchCardsFromDeck(string collectionOwnerId, string? searchQuery, string? deckName = null);

        Task AddCardToDeckAsync(string collectionOwnerId, string cardId, string? deckName = null);
        Task<(List<CardsCollection> cardsOnPage, int totalMatches)> SearchCardsFromCollection(string collectionOwnerId, string? searchQuery=null, int page = 1, int itemsPerPage = 10);
        Task<(List<CardsCollection> cardsOnPage, int totalMatches)> SearchCardsFromDeck(string collectionOwnerId, string? searchQuery, int page = 1, int itemsPerPage = 10, string? deckName = null);
        (string cardId, int? updatedQuantity, int? cardLeftInCollection, string response) UpdateQuantityFromDeck(string userId, string id, string? deckName, int qtChange);
        Task<List<SelectListItem>> GetPlayerDecksNames(string userId);
    }
}