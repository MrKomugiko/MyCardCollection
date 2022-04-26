using MyCardCollection.Services.Scryfall.Card;
using System.ComponentModel.DataAnnotations;

namespace MyCardCollection.Models
{
    public class CardData
    {
        public CardData()
        {

        }
        public CardData(Root CardObject)
        {
   
            CardId = CardObject.Id;
                
            CollectionNumber = int.TryParse(CardObject.collector_number, out int res)?res:-1;
            SetCode = CardObject.set;
            CMC = CardObject.cmc;
                if(CardObject.prices != null)
                {
                    Price_USD = CardObject.prices.usd != null ? float.Parse(CardObject.prices.usd.Replace(".", ",").Trim()) : null;
                }
                    
            Rarity = CardObject.rarity;

            if (CardObject.power == "*")
            {
                CardObject.power = "0";
            }
            if (CardObject.toughness == "*")
            {
                CardObject.toughness = "0";
            }

            if (CardObject.card_faces == null)
            {
                Name = CardObject.Name;
                Type = CardObject.type_line;
                try
                {
                    Health = CardObject.toughness != null ? int.Parse(CardObject.toughness) : null;
                    
                }
                catch (Exception)
                {
                    // ex. MID 177
                    // card atk, hp depends on effect */*+1
               
                }
                Power = CardObject.power != null ? int.Parse(CardObject.power) : null;
                Mana_Cost = CardObject.mana_cost;
                ImageURL = CardObject.image_uris != null?CardObject.image_uris.normal: "image not found";
                ImageURLCropped = CardObject.image_uris.art_crop;
                Description = CardObject.oracle_text;
                FlavorDescription = CardObject.flavor_text;
            }
            else
            {
                if (CardObject.card_faces[0].power == "*")
                {
                    CardObject.card_faces[0].power = "0";
                }
                if (CardObject.card_faces[1].power == "*")
                {
                    CardObject.card_faces[1].power = "0";
                }
                if (CardObject.card_faces[0].toughness == "*")
                {
                    CardObject.card_faces[0].toughness = "0";
                }
                if (CardObject.card_faces[1].toughness == "*")
                {
                    CardObject.card_faces[1].toughness = "0";
                }
                
                HasTransform = true;
               
                //front
                Name = CardObject.card_faces[0].Name;
                Type = CardObject.card_faces[0].type_line;
                Health = CardObject.card_faces[0].toughness != null ? int.Parse(CardObject.card_faces[0].toughness) : null;
                Power = CardObject.card_faces[0].power != null ? int.Parse(CardObject.card_faces[0].power) : null;
                Mana_Cost = CardObject.card_faces[0].mana_cost;
                ImageURL = CardObject.card_faces[0].image_uris != null ? CardObject.card_faces[0].image_uris.normal : null;
                ImageURLCropped = CardObject.card_faces[0].image_uris != null ? CardObject.card_faces[0].image_uris.art_crop : null;
                Description = CardObject.card_faces[0].oracle_text;
                FlavorDescription = CardObject.card_faces[0].flavor_text;

                //back
                Transform_Name = CardObject.card_faces[1].Name;
                Transform_Type = CardObject.card_faces[1].type_line;
                Transform_Health = CardObject.card_faces[1].toughness != null ? int.Parse(CardObject.card_faces[1].toughness) : null;
                Transform_Power = CardObject.card_faces[1].power != null ? int.Parse(CardObject.card_faces[1].power) : null;
                Transform_ImageURL = CardObject.card_faces[1].image_uris != null ?  CardObject.card_faces[1].image_uris.normal : null;
                Transform_ImageURLCropped = CardObject.card_faces[1].image_uris != null ?  CardObject.card_faces[1].image_uris.art_crop : null;
                Transform_Description = CardObject.card_faces[1].oracle_text;
                Transform_FlavorDescription = CardObject.card_faces[1].flavor_text;
            }

            CardDataUpdated = DateTime.UtcNow;
        }

        [Key]
        public string CardId { get; set; }
        public int CollectionNumber { get; set; }
        public string Name { get; set; }
        public string SetCode { get; set; }
        public float? Price_USD { get; set; }
        public string Rarity { get; set; }
        public string? Mana_Cost { get; set; }
        public float? CMC { get; set; }
        public string? Type { get; set; }
        public int? Health { get; set; }
        public int? Power { get; set; }
        public string? ImageURL { get; set; }
        public string? ImageURLCropped { get; set; }

        public string? Description { get; set; }
        public string? FlavorDescription { get; set; }

        public bool HasTransform { get; set; } = false;
        public string? Transform_Name { get; set; }
        public string? Transform_Type { get; set; }
        public int? Transform_Health { get; set; }
        public int? Transform_Power { get; set; }
        public string? Transform_ImageURL { get; set; }
        public string? Transform_ImageURLCropped { get; set; }
        public string? Transform_Description { get; set; }
        public string? Transform_FlavorDescription { get; set; }

        public DateTime CardDataUpdated { get; set; }
    }
}
