using MyCardCollection.Services.Scryfall.Card;

namespace MyCardCollection.Services.Scryfall
{ 
    public class CardList
    {
        public string @object { get; set; }
        public int total_cards { get; set; }
        public bool has_more { get; set; }
        public string next_page { get; set; }
        public List<Root> data { get; set; }
    }
}
