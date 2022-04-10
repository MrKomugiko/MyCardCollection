namespace MyCardCollection.Services.Scryfall
{
    public class SetData
    {
        public string @object { get; set; }
        public string id { get; set; }
        public string code { get; set; }
        public string mtgo_code { get; set; }
        public string arena_code { get; set; }
        public int tcgplayer_id { get; set; }
        public string name { get; set; }
        public string uri { get; set; }
        public string scryfall_uri { get; set; }
        public string search_uri { get; set; }
        public string released_at { get; set; }
        public string set_type { get; set; }
        public int card_count { get; set; }
        public bool digital { get; set; }
        public bool nonfoil_only { get; set; }
        public bool foil_only { get; set; }
        public string block_code { get; set; }
        public string block { get; set; }
        public string icon_svg_uri { get; set; }
    }
}
