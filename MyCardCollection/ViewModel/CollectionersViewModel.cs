using MyCardCollection.Models;

namespace MyCardCollection.ViewModel
{
    public class CollectionersViewModel
    {
        public IEnumerable<AppUser> Users { get; set; }
        public int TotalUsers { get; set; }
        public int Category { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
