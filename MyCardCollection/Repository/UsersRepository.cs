using Microsoft.EntityFrameworkCore;
using MyCardCollection.Controllers;
using MyCardCollection.Data;
using MyCardCollection.Enums;
using MyCardCollection.Models;
using Npgsql;

namespace MyCardCollection.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _context;

        public UsersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(AppUser user)
        {
            throw new NotImplementedException();
        }

        public bool Delete(AppUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<AppUser> GetUserByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<int> GetCountUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(AppUser user)
        {
            _context.Update(user);
            return Save();
        }
        public async Task<IEnumerable<AppUser>> GetUsersDataAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<AppUser>> GetUsersAsyncByCategory(CollectionersSortCategory category)
        {
            switch(category)
            {
                case CollectionersSortCategory.alphabetical: return await _context.Users.OrderBy(x=>x.UserName).AsNoTracking().ToListAsync();
                case CollectionersSortCategory.oldest:       return await _context.Users.OrderBy(x=>x.Created).AsNoTracking().ToListAsync();
                case CollectionersSortCategory.newest:       return await _context.Users.OrderByDescending(x => x.Created).AsNoTracking().ToListAsync();
                case CollectionersSortCategory.biggest:      return await _context.Users.OrderByDescending(x => x.TotalCards).AsNoTracking().ToListAsync();
                case CollectionersSortCategory.value:        return await _context.Users.OrderByDescending(x => x.TotalValue).AsNoTracking().ToListAsync();
            }
            return await _context.Users.AsNoTracking().ToListAsync();
        }
        public void UpdatePlayerStatistics(string userId)
        {
            using (var cn = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING")))
            {
                NpgsqlCommand cmd = new("CALL public.updateusersstats(:userid)", cn);
                cmd.Parameters.AddWithValue("userid", NpgsqlTypes.NpgsqlDbType.Varchar).Value = userId;
                cmd.CommandType = System.Data.CommandType.Text;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
            /*
             CREATE OR REPLACE PROCEDURE updateusersstats(_userid varchar)
                LANGUAGE plpgsql AS  
                $$  
                BEGIN         
                  UPDATE "AspNetUsers" AS "users" 
                  SET 
                  "DecksCreated"="stats"."Decks",
                  "TotalCards" = "stats"."TotalCards",
                  "TotalValue" = "stats"."TotalValue",
                  "UniqueCards" = "stats"."UniqueCards"
                  FROM (SELECT "Collection"."UserId", 
                          Sum("CardsDatabase"."Price_USD") as "TotalValue", 
                          Count("Collection"."CardId") as "UniqueCards", 
                          Sum("Collection"."Quantity") as "TotalCards",
                          (SELECT Count("AppUserId") FROM "Decks" WHERE "Decks"."AppUserId" = "UserId" ) as "Decks"
                      FROM "Collection"
                      JOIN "CardsDatabase"
                      ON "Collection"."CardId" = "CardsDatabase"."CardId"
                      GROUP BY "Collection"."UserId") AS "stats"
                  WHERE "users"."Id" = _userid;
                END  
                $$;  
             */
        }
        public void UpdateAllPlayersStatistics()
        {
            using (var cn = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING")))
            {
                NpgsqlCommand cmd = new("CALL public.UpdateAllUsersStats()", cn);
                cmd.CommandType = System.Data.CommandType.Text;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
            //_context.Users.FromSqlRaw("CALL public.UpdateUserStats('57205f84-ab04-4e2f-9efb-9d5383df803b')");
            //_context.Users.FromSqlRaw("CALL public.UpdateUserStats('57205f84-ab04-4e2f-9efb-9d5383df803b');");
            //_context.Users.FromSqlRaw("CALL public.UpdateAllUsersStats()");



            /*
             CREATE OR REPLACE PROCEDURE updateallusersstats()
                LANGUAGE plpgsql AS  
                $$  
                BEGIN         
                  UPDATE "AspNetUsers" AS "users" 
                  SET 
                  "DecksCreated"="stats"."Decks",
                  "TotalCards" = "stats"."TotalCards",
                  "TotalValue" = "stats"."TotalValue",
                  "UniqueCards" = "stats"."UniqueCards"
                  FROM (SELECT "Collection"."UserId", 
                          Sum("CardsDatabase"."Price_USD") as "TotalValue", 
                          Count("Collection"."CardId") as "UniqueCards", 
                          Sum("Collection"."Quantity") as "TotalCards",
                          (SELECT Count("AppUserId") FROM "Decks" WHERE "Decks"."AppUserId" = "UserId" ) as "Decks"
                      FROM "Collection"
                      JOIN "CardsDatabase"
                      ON "Collection"."CardId" = "CardsDatabase"."CardId"
                      GROUP BY "Collection"."UserId") AS "stats"
                  WHERE "users"."Id" = "stats"."UserId";
                END  
                $$;  
             */
        }
    }
}
