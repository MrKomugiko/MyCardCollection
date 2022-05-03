using System.ComponentModel.DataAnnotations;

namespace MyCardCollection.ViewModel

{
    public class DeckEditViewModel
    {
        public int DeckId { get; set; } 
        public string Name { get; set; }  
        public string Description { get; set; }
        public string BackgroundImage { get; set; }
    }
}
