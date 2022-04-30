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