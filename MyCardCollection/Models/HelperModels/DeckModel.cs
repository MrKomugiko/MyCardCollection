namespace MyCardCollection.Controllers
{
        public class DeckModel
        {
            public string userId {get;set;}
            public string deckName {get;set;}
            public int deckId {get;set;}
            public CardInfo[] cardInfos { get; set; }

            public class CardInfo
            {
                public string cardId { get; set; }
                public int quantity { get; set; }
            }
        } 
    }
