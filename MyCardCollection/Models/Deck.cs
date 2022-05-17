using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace MyCardCollection.Models
{
    public class Deck
    {
        public Deck()
        { }
          
        public Deck(string name, string owner)
        {
            this.Name = name;
            this.AppUserId = owner;
        }

        [Key]
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        public string Description { get; set; } = "";
        [Required] public string BackgroundImage { get; set; } = "https://media-dominaria.cursecdn.com/attachments/165/391/636430808895507600.jpg";
        public int CardsNumber { get; set; } = 0;
        public float TotalValue { get; set; } = 0;
        public bool IsValid { get; set; } = false;
        public DateTime Created { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime? Updated { get; set; } = DateTime.Now.ToUniversalTime();

        public string? AppUserId { get; set; }
        [ForeignKey("AppUserId")] public AppUser? AppUser { get; set; }

        public ICollection<DecksCollection> Content { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<DeckLike> Likes { get; set; }

    }
}