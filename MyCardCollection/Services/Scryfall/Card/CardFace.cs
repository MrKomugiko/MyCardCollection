namespace MyCardCollection.Services.Scryfall.Card
{
    public class CardFace
    {
        public string @object { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string printed_name { get; set; }
        public string printed_text { get; set; }
        public string flavor_text { get; set; }
        public string mana_cost { get; set; }
        public string type_line { get; set; }
        public string oracle_text { get; set; }
        public string watermark { get; set; }
        public List<string> colors { get; set; }
        public string? power { get; set; }
        public string? toughness { get; set; }
        public string artist { get; set; }
        public string artist_id { get; set; }
        public string loyality { get; set; }
        public string illustration_id { get; set; }
        public ImageUris image_uris { get; set; }
        public List<string> color_indicator { get; set; }
    }
}
