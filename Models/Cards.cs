using MyCardCollection.Services.Scryfall.Card;

namespace MyCardCollection.Models
{
    public class Cards
    {
        public Cards(int quantity, Root card)
        {
            Quantity = quantity;
            Card = card;
        }

        public Root Card { get; set; }
        public int Quantity { get; set; }
    }
}
