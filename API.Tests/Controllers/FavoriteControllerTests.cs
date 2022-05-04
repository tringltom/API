using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using AutoFixture;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    public class FavoriteControllerTests
    {
        private Mock<IFavoritesService> _favoriteServiceMock;
        private FavoriteController _sut;

        [SetUp]
        public void SetUp()
        {
            _favoriteServiceMock = new Mock<IFavoritesService>();
            _sut = new FavoriteController(_favoriteServiceMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetFavoriteActivity_SuccessfullAsync(UserFavoriteActivityReturn favoriteActivity)
        {
            // Arrange
            _favoriteServiceMock.Setup(x => x.GetFavoriteActivityAsync(It.IsAny<int>()))
               .ReturnsAsync(favoriteActivity);

            // Act
            var res = await _sut.GetFavoriteActivity(It.IsAny<int>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(favoriteActivity);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetOwnerFavoriteActivityIds_SuccessfullAsync(List<FavoriteActivityIdReturn> favoriteActivityIds)
        {
            // Arrange
            _favoriteServiceMock.Setup(x => x.GetAllOwnerFavoriteIdsAsync())
               .ReturnsAsync(favoriteActivityIds);

            // Act
            var res = await _sut.GetOwnerFavoriteActivityIds() as OkObjectResult;

            // Assert
            res.Value.Should().Be(favoriteActivityIds);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task RemoveFavouriteActivity_SuccessfullAsync()
        {
            // Arrange
            _favoriteServiceMock.Setup(x => x.RemoveFavoriteActivityAsync(It.IsAny<int>()))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.RemoveFavouriteActivity(It.IsAny<int>()) as NoContentResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.NoContent);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task CreateFavoriteActivity_SuccessfullAsync(UserFavoriteActivityReturn favoriteActivity)
        {
            // Arrange
            _favoriteServiceMock.Setup(x => x.AddFavoriteActivityAsync(It.IsAny<int>()))
               .ReturnsAsync(favoriteActivity);

            // Act
            var res = await _sut.CreateFavoriteActivity(It.IsAny<int>()) as CreatedAtRouteResult;

            // Assert
            res.Value.Should().Be(favoriteActivity);
        }
    }
}
