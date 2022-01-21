using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.ServiceInterfaces;
using AutoFixture.NUnit3;
using FixtureShared;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Models.Activity;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    internal class FavoriteControllerTests
    {
        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void CreateFavoriteActivity_Successfull(
            [Frozen] Mock<IFavoritesService> favoriteServiceMock,
            FavoriteActivityCreate activity,
            [Greedy] FavoriteController sut)
        {
            // Arrange
            favoriteServiceMock.Setup(x => x.CreateFavoriteAsync(activity))
               .Returns(Task.CompletedTask);

            // Act
            var res = sut.CreateFavoriteActivity(activity);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RemoveFavoriteActivity_Successfull(
            [Frozen] Mock<IFavoritesService> favoriteActivityService,
            FavoriteActivityRemove activity,
            [Greedy] FavoriteController sut)
        {
            // Arrange
            favoriteActivityService.Setup(x => x.RemoveFavoriteAsync(activity))
               .Returns(Task.CompletedTask);

            // Act
            var res = sut.RemoveFavoriteActivity(activity);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetFavoriteActivitiesForUser_Successfull(
            [Frozen] Mock<IFavoritesService> favoriteActivityService,
            List<FavoriteActivityReturn> favoriteActivities,
            int id,
            [Greedy] FavoriteController sut)
        {
            // Arrange
            favoriteActivityService.Setup(x => x.GetAllFavoritesForUserAsync(id))
               .ReturnsAsync(favoriteActivities);

            // Act
            var res = sut.GetFavoriteActivitiesForUser(id);

            // Assert
            res.Result.Should().Equal(favoriteActivities);
        }
    }
}
