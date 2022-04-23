using Microsoft.AspNetCore.Mvc;
using Moq;
using MyCardCollection.Controllers;
using MyCardCollection.Repository;
using MyCardCollection.Services;
using MyCardCollection.Services.Scryfall;
using MyCardCollection.Services.Scryfall.Card;
using MyCardCollection.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MyCardCollection.HomeControllerTests.Tests
{
    public class IndexTests
    {

        Mock<IScryfallService> _scryfallServiceMock = new Mock<IScryfallService>();
        Mock<ICacheService> _cacheService = new Mock<ICacheService>();
        Mock<ICollectionRepository> _collectionRepository = new Mock<ICollectionRepository>();

        HomeController _homeController;

        delegate void tryGetsVallback(string setcode, out List<SetListViewModel> setslist);
        delegate bool tryGetlistReturns(string setcode, out List<SetListViewModel> setslist);
        public IndexTests()
        {
            _homeController = new HomeController(_scryfallServiceMock.Object, _cacheService.Object, _collectionRepository.Object);

            _scryfallServiceMock.Setup(x => x.GetSetsList().Result)
                .Returns(new List<SetData>(){
                    new SetData()
                    {
                        @object = "set",
                        id = "df837242-8c15-42e4-b049-c933a02dc501",
                        code = "snc",
                        mtgo_code = "snc",
                        arena_code = "snc",
                        tcgplayer_id = 3026,
                        name = "Streets of New Capenna",
                        uri = "https://api.scryfall.com/sets/df837242-8c15-42e4-b049-c933a02dc501",
                        scryfall_uri = "https://scryfall.com/sets/snc",
                        search_uri = "https://api.scryfall.com/cards/search?order=set&q=e%3Asnc&unique=prints",
                        released_at = "2022-04-29",
                        set_type = "expansion",
                        card_count = 467,
                        digital = false,
                        nonfoil_only = false,
                        foil_only = false,
                        icon_svg_uri = "https://c2.scryfall.com/file/scryfall-symbols/sets/snc.svg?1650254400"
                    },
                    new SetData(){
                        @object = "set",
                        id = "59a2059f-5482-433f-8761-eb2e17859b71",
                        code = "neo",
                        mtgo_code = "neo",
                        arena_code = "neo",
                        tcgplayer_id = 2965,
                        name = "Kamigawa: Neon Dynasty",
                        uri = "https://api.scryfall.com/sets/59a2059f-5482-433f-8761-eb2e17859b71",
                        scryfall_uri = "https://scryfall.com/sets/neo",
                        search_uri = "https://api.scryfall.com/cards/search?order=set&q=e%3Aneo&unique=prints",
                        released_at = "2022-02-18",
                        set_type = "expansion",
                        card_count = 512,
                        digital = false,
                        nonfoil_only = false,
                        foil_only = false,
                        icon_svg_uri = "https://c2.scryfall.com/file/scryfall-symbols/sets/neo.svg?1650254400"
                    },
                });

            var setslist = new List<SetListViewModel>();
            _cacheService.Setup(x => x.TryGetValue<List<SetListViewModel>>("Sets", out setslist))
                .Callback(new tryGetsVallback((string setcode,out List <SetListViewModel> _setslist) => {
                    _setslist = setslist;
                }))
            .Returns(new tryGetlistReturns((string setcode,out List <SetListViewModel> _setslist) => { 
                    _setslist = setslist; return false; 
                }));

        }

        [Fact]
        public async Task HomeController_Index_ReturnView()
        {

            var result = await _homeController.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async Task HomeController_Index_ModelIsSetListVM(int page) 
        { 
            var result = await _homeController.Index(page);
            var Model = (result as ViewResult).Model as IEnumerable<SetListViewModel>;
            var expectedFirstOutput = new SetListViewModel()
            {
                card_count = 467,
                icon_svg_uri = "https://c2.scryfall.com/file/scryfall-symbols/sets/snc.svg?1650254400",
                name = "Streets of New Capenna",
                released_at = new DateTime(2022, 4, 29),
                setcode = "snc"
            };
            
            Assert.IsType<SetListViewModel>(Model.First());
            Assert.NotNull(Model);
            Assert.Equal("Streets of New Capenna", expectedFirstOutput.name);
            Assert.Equal(467, expectedFirstOutput.card_count);
            Assert.Equal(expectedFirstOutput.released_at, new DateTime(2022, 4, 29));

        }
    }
}