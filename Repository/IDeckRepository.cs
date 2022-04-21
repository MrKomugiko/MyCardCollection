using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Controllers;
using MyCardCollection.Models;

namespace MyCardCollection.Repository
{
    public interface IDeckRepository
    {
        Task<Dictionary<int,string>> GetDeckNames(string userId);
        (string cardId, int? updatedQuantity, int? cardLeftInCollection, string response) UpdateQuantityFromDeck(string userId, string id, int deckId, int qtChange);
        Task<List<CardsCollection>> GetAll_SearchCardsFromDeck(string collectionOwnerId, string? searchQuery, int? deckId = null);
        Task<(List<CardsCollection> cardsOnPage, int totalMatches)> SearchCardsFromDeck(string collectionOwnerId, string? searchQuery, int page = 1, int itemsPerPage = 10, int? deckId = null);
        Task AddCardToDeckAsync(string collectionOwnerId, string cardId, int deckId);
        Task ClearDeck(int deckId, string userId);
        Task<List<CardsCollection>> GetDeckDataFromDatabase(string collectionOwnerId, int? deckId);
        Task<bool> CreateNewDeck(string decktitle, string _userId);
        Task<bool> Update(DeckModel deck, string userId);
        Task<List<Deck>> GetUserDecksWithoutContent(string userId);
        Task<List<Deck>> GetUserDecks(string userId);

    }
}
