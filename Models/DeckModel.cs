namespace MyCardCollection.Controllers
{
        public class DeckModel
        {
            public string userId {get;set;}
            public string deckName {get;set;}
            public CardInfo[] cardInfos { get; set; }

            public class CardInfo
            {
                public string cardId { get; set; }
                public int quantity { get; set; }
            }

            // example
            /*
                 {
                    "userId": "ada0f67b-2f8a-4edb-bb89-e20b8448c7f1",
                    "deckName":"mojtestowyDeck",
                    "cardInfos":[
                        {
                            "cardId": "00101358-0e89-4bd1-b1f2-e889645b616e",
                            "quantity": 1
                        },
                        {
                            "cardId": "00feb2af-b363-4377-98b1-6a07df7f1acd",
                            "quantity": 2
                        },
                        {
                            "cardId": "0162885d-3f55-4440-b40d-e38d378c4567",
                            "quantity": 3
                        },
                        {
                            "cardId": "017fb3fc-433e-4853-bbac-99fa04b40233",
                            "quantity":4
                        },
                        {
                            "cardId": "0193dfa3-8409-44be-b4be-6c3cad42d4a4",
                            "quantity": 2
                        }
                    ]
                }
             */
        } 
    }
