using Microsoft.AspNetCore.Mvc;
using Moq;
using MyCardCollection.Controllers;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using MyCardCollection.Services;
using MyCardCollection.Services.Scryfall;
using MyCardCollection.Services.Scryfall.Card;
using MyCardCollection.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MyCardCollection.HomeControllerTests.Tests
{
    public class ListTests
    {
        Mock<IScryfallService> _scryfallServiceMock = new Mock<IScryfallService>();
        Mock<ICacheService> _cacheService = new Mock<ICacheService>();
        Mock<ICollectionRepository> _collectionRepository = new Mock<ICollectionRepository>();
        HomeController _homeController;
        public ListTests()
        {
            _homeController = new HomeController(_scryfallServiceMock.Object, _cacheService.Object, _collectionRepository.Object);

            string setCode = "vow";
            _scryfallServiceMock.Setup(x => x.GetCardsListBySet(setCode).Result)
                .Returns(new List<Root>(){
                    new Root()
                    {
                        Name = "card 1",
                        collector_number = "1",
                        cmc = 5,
                        set = "vow",
                        image_uris = new ImageUris()
                        {
                            normal = "http://image.png"
                        }
                        /* simplified for test purpose */
                    },
                    new Root()
                    {
                        Name = "card 3",
                        collector_number = "3",
                        cmc = 3,
                        set = "vow",
                        image_uris = new ImageUris()
                        {
                            normal = "http://image.png"
                        }
                        /* simplified for test purpose */
                    },
                    new Root()
                    {
                        Name = "card 2",
                        collector_number = "2",
                        cmc = 0,
                        set = "vow",
                        image_uris = new ImageUris()
                        {
                            normal = "http://image.png"
                        }
                        /* simplified for test purpose */
                    }
                }); ;
        }
        
       
        [Fact]
        public async Task HomeController_List_ReturnsView()
        {
            var result = await _homeController.List("vow");
            Assert.IsType<ViewResult>(result);
        }

      

        [Fact]
        public async Task HomeController_List_ModelIsACardListVM()
        {
            var result = await _homeController.List("vow");
            var Model = (result as ViewResult).Model as List<CardListViewModel>;
            var expectedFirstOutput = new CardListViewModel()
            {
              Card = new CardData() { 
                Name = "card 1",
                CollectionNumber = 1,
                CMC = 5,
                ImageURL = "http://image.png",
                SetCode = "vow"
                /* simplified for test purpose */
              }
            };
            var expectedCount = 3;

            Assert.IsType<List<CardListViewModel>>(Model);
            Assert.NotNull(Model);
            Assert.Equal(expectedCount, Model.Count);
            Assert.Equal(expectedFirstOutput.Card.Name, "card 1");
            Assert.Equal(expectedFirstOutput.Card.CollectionNumber, 1);
            Assert.Equal(expectedFirstOutput.Card.ImageURL, "http://image.png");

        }

    }
}