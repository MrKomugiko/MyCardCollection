using MyCardCollection.Models;

namespace MyCardCollection.ViewModel
{
    public class CollectionerProfileViewModel
    {
        public AppUser AppUser { get; set; }
        public IEnumerable<CardData> TopCards { get; set; }
    }
}