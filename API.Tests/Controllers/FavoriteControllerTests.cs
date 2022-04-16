﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using AutoFixture.NUnit3;
using FixtureShared;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
            FavoriteActivityBase activity,
            [Greedy] FavoriteController sut)
        {
            // Arrange
            favoriteServiceMock.Setup(x => x.ResolveFavoriteActivityAsync(activity))
               .Returns(Task.CompletedTask);

            // Act
            var res = sut.ResolveFavoriteActivity(activity);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResolveFavoriteActivity_Successfull(
            [Frozen] Mock<IFavoritesService> favoriteActivityService,
            FavoriteActivityBase activity,
            [Greedy] FavoriteController sut)
        {
            // Arrange
            favoriteActivityService.Setup(x => x.ResolveFavoriteActivityAsync(activity))
               .Returns(Task.CompletedTask);

            // Act
            var res = sut.ResolveFavoriteActivity(activity);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetFavoriteActivitiesForUser_Successfull(
            [Frozen] Mock<IFavoritesService> favoriteActivityService,
            List<FavoriteActivityIdReturn> favoriteActivities,
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
