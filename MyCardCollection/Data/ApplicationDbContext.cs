using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCardCollection.Models;

namespace MyCardCollection.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.UseSerialColumns();
        }

        public DbSet<CardData> CardsDatabase { get; set; }
        public DbSet<CardsCollection> Collection { get; set; }
        public DbSet<DecksCollection> DecksCollections { get; set; }
        public DbSet<PrivacySettings> UserPrivacySettings { get; set; }

        public DbSet<Deck> Decks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReply> Comment_Replies { get; set; }
    }
}