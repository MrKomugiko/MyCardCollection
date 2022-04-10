namespace MyCardCollection.Services.Scryfall
{
    public class SetsList
    {
        public string @object { get; set; }
        public bool has_more { get; set; }
        public List<SetData> data { get; set; }
        public string? next_page { get; set; }
    }
}
