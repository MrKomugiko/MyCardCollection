using MyCardCollection.Models;

namespace MyCardCollection.ViewModel
{
    public class DeckStatisticsViewModel
    {
        public int DeckId { get; set; }
        public string DeckName { get; set; }
        public string OwnerId { get; set; }
        public int Size { get; set; }
        public int[] ManaCurve { get; set; }
        public Dictionary<string, int> TypeDistribution { get; set; }
        public IEnumerable<(int Quantity, CardData data)> Cards { get; set; }
    }
}
