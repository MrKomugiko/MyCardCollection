using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCardCollection.Models;

namespace MyCardCollection.Data
{
    public class ApplicationDbContext : IdentityDbContext
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
    }
}