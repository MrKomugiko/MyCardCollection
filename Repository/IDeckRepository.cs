using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Controllers;
using MyCardCollection.Models;

namespace MyCardCollection.Repository
{
    public interface IDeckRepository
    {
        Task Update(DeckModel deck, string userId);
        Task<string[]> GetDeckNames(string userId);
        (string cardId, int? updatedQuantity, int? cardLeftInCollection, string response) UpdateQuantityFromDeck(string userId, string id, string? deckName, int qtChange);
        Task<List<CardsCollection>> GetAll_SearchCardsFromDeck(string collectionOwnerId, string? searchQuery, string? deckName = null);
        Task<(List<CardsCollection> cardsOnPage, int totalMatches)> SearchCardsFromDeck(string collectionOwnerId, string? searchQuery, int page = 1, int itemsPerPage = 10, string? deckName = null);
        Task AddCardToDeckAsync(string collectionOwnerId, string cardId, string deckName);
        Task ClearDeck(string currentDeck, string userId);
        Task<List<CardsCollection>> GetDeckDataFromDatabase(string collectionOwnerId, string? deckName);
    }
}
