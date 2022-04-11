using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Data;
using MyCardCollection.Services;
using Newtonsoft.Json;

namespace MyCardCollection.Infrastructure
{
    public class StatisticsChartsViewComponent : ViewComponent
    {
        private readonly ICollectionService _collection;
        public StatisticsChartsViewComponent(ICollectionService collection)
        {
            _collection = collection;
        }

        public async Task<IViewComponentResult> InvokeAsync(string _userId)
        {
            //var stats = await _collection.GetFullStatistics_v2(_userId);
            //var rarityArr = new int[]{
            //    stats.RarityCount.common,
            //    stats.RarityCount.uncommon,
            //    stats.RarityCount.rare,
            //    stats.RarityCount.mythic
            //};
            //var typeArr = new int[]{
            //    stats.MainCardTypesCount.Artifact,
            //    stats.MainCardTypesCount.Creature,
            //    stats.MainCardTypesCount.Enchantment,
            //    stats.MainCardTypesCount.Instant,
            //    stats.MainCardTypesCount.Land,
            //    stats.MainCardTypesCount.Planeswalker,
            //    stats.MainCardTypesCount.Sorcery
            //};

            //ViewBag.RarityNamesArray = JsonConvert.SerializeObject(stats.RarityNames);
            //ViewBag.RarityCountArray = JsonConvert.SerializeObject(rarityArr);

            //ViewBag.TypeNamesArray = JsonConvert.SerializeObject(stats.TypeNames);
            //ViewBag.TypeCountArray = JsonConvert.SerializeObject(typeArr);

            //ViewBag.SetsNamesArray = JsonConvert.SerializeObject(stats.SetDict.Keys);
            //ViewBag.SetsCountArray = JsonConvert.SerializeObject(stats.SetDict.Values);

            return View();
        }
    }
}
