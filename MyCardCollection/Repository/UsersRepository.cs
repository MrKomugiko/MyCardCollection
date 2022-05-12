using Microsoft.EntityFrameworkCore;
using MyCardCollection.Areas.Identity.Pages.Account.Manage;
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

        public async Task<AppUser?> GetUserByIdIncludeDecksAsync(string id)
        {
            return await _context.Users
               .Where(x => x.Id == id)
               .Select(x => new AppUser
               {
                   Id = x.Id,
                   BackgroundProfileImage = x.BackgroundProfileImage,
                   AvatarImage = x.AvatarImage,
                   Name = x.Name,
                   UserName = x.UserName,
                   Lastname = x.Lastname,
                   City = x.City,
                   CountryCode = x.CountryCode,
                   UniqueCards = x.UniqueCards,
                   TotalCards = x.TotalCards,
                   TotalValue = x.TotalValue,
                   Decks = x.Decks.Select(x => new Deck
                   {
                       Id = x.Id,
                       BackgroundImage = x.BackgroundImage,
                       Name = x.Name,
                       Description = x.Description,
                       CardsNumber = x.CardsNumber,
                       TotalValue = x.TotalValue,
                       Comments = x.Comments.Select(x=> new Comment 
                        { Id = x.Id }).ToList()
                   }).ToList()
               })
               .AsNoTracking()
               .SingleOrDefaultAsync();
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
        public async Task<IEnumerable<AppUser>> GetUsersDataAsync(bool includePrivacy)
        {
            
            if(includePrivacy)
            {
                return await _context.Users
                    .Include(x=>x.PrivacySettings)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
                return await _context.Users
                    .AsNoTracking()
                    .ToListAsync();

        }
        public async Task<IEnumerable<AppUser>> GetUsersAsyncByCategory(CollectionersSortCategory category, bool includePrivacy)
        {
            IQueryable _userQuery;
            switch(category)
            {
                case CollectionersSortCategory.alphabetical: _userQuery = _context.Users.OrderBy(x => x.UserName); break;
                case CollectionersSortCategory.oldest:       _userQuery = _context.Users.OrderBy(x => x.Created);  break;
                case CollectionersSortCategory.newest:       _userQuery = _context.Users.OrderByDescending(x => x.Created); break;
                case CollectionersSortCategory.biggest:      _userQuery = _context.Users.OrderByDescending(x => x.TotalCards); break;
                case CollectionersSortCategory.value:        _userQuery = _context.Users.OrderByDescending(x => x.TotalValue); break;
                default: _userQuery = _context.Users; break;
            }
            if (includePrivacy)
            {
                return await _userQuery.OfType<AppUser>().Include(x=>x.PrivacySettings).AsNoTracking().ToListAsync();
            }
            else
                return await _userQuery.OfType<AppUser>().AsNoTracking().ToListAsync();
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

        public async Task<PrivacySettings> GetPrivacyDataByUser(AppUser user) => await _context.UserPrivacySettings.AsNoTracking().FirstAsync(x => x.UserId == user.Id);

        public async Task<bool> UpdateUserData(AppUser _user, IndexModel.InputModel input)
        {
            var user = await _context.Users.Where(x => x.Id == _user.Id).FirstAsync();
            var userPrivacy = await _context.UserPrivacySettings.Where(x=>x.UserId == _user.Id).FirstAsync();

            user.UserName = input.UserName;
            user.NormalizedUserName = input.UserName.ToUpper();

            user.AvatarImage = input.AvatarImage;
            user.Name = input.Name??"";
            user.Lastname = input.Lastname??"";
            user.City = input.City??"";
            user.CountryCode = input.CountryCode ?? "";
            user.Description = input.Description ?? "";
            user.Birthday = ((DateTime)(input.Birthday)).ToUniversalTime();
            user.Website = input.Website ?? "";

            userPrivacy.AllowEmail = input.Privacy.AllowEmail;
            userPrivacy.AllowFirstName = input.Privacy.AllowFirstName;
            userPrivacy.AllowLastName = input.Privacy.AllowLastName;
            userPrivacy.AllowCity = input.Privacy.AllowCity;
            userPrivacy.AllowCountry = input.Privacy.AllowCountry;
            userPrivacy.AllowBirthday = input.Privacy.AllowBirthday;
            userPrivacy.AllowWebsite = input.Privacy.AllowWebsite;

            _context.Update(user);
            _context.Update(userPrivacy);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
