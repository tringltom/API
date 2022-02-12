using System;
using System.Threading.Tasks;
using Application.Errors;
using Application.RepositoryInterfaces;
using Application.ServiceInterfaces;
using AutoFixture.NUnit3;
using Domain.Entities;
using FixtureShared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Models.Activity;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class FavoriteServiceTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResolveFavoriteAsync_CreateSuccessfull([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
            [Frozen] Mock<IUserRepository> userRepoMock, FavoriteActivityBase activity,
            int userId, FavoritesService sut)
        {

            //Arrange

            var favoriteActivity = new UserFavoriteActivity() { ActivityId = activity.ActivityId, UserId = userId };

            activity.Favorite = true;

            userRepoMock.Setup(urm => urm.GetUserIdUsingToken())
                .Returns(userId);

            favoriteRepoMock.Setup(frm => frm.AddFavoriteActivityAsync(favoriteActivity))
                .Returns(Task.CompletedTask);

            favoriteRepoMock.Setup(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId))
                .ReturnsAsync(true);

            //Act
            Func<Task> methodInTest = async () => await sut.ResolveFavoriteActivityAsync(activity);

            //Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(urm => urm.GetUserIdUsingToken(), Times.Once);
            favoriteRepoMock.Verify(frm => frm.AddFavoriteActivityAsync(favoriteActivity), Times.Once);
            favoriteRepoMock.Verify(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResolveFavoriteAsync_AddFailed([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
            [Frozen] Mock<IUserRepository> userRepoMock,
            UserFavoriteActivity favoriteActivity, FavoriteActivityBase activity,
            int userId, FavoritesService sut)
        {

            //Arrange

            activity.Favorite = true;

            userRepoMock.Setup(urm => urm.GetUserIdUsingToken())
                .Returns(userId);

            favoriteRepoMock.Setup(frm => frm.AddFavoriteActivityAsync(favoriteActivity))
                .Throws(new DbUpdateException());

            favoriteRepoMock.Setup(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId))
                .ReturnsAsync(true);

            //Act
            Func<Task> methodInTest = async () => await sut.ResolveFavoriteActivityAsync(activity);

            //Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(urm => urm.GetUserIdUsingToken(), Times.Once);
            favoriteRepoMock.Verify(frm => frm.AddFavoriteActivityAsync(favoriteActivity), Times.Once);
            favoriteRepoMock.Verify(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResolveFavoriteAsync_RemoveSuccessfull([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
            [Frozen] Mock<IUserRepository> userRepoMock,
            UserFavoriteActivity favoriteActivity, FavoriteActivityBase activity,
            int userId, FavoritesService sut)
        {

            //Arrange

            activity.Favorite = false;

            userRepoMock.Setup(urm => urm.GetUserIdUsingToken())
               .Returns(userId);

            favoriteRepoMock.Setup(frm => frm.AddFavoriteActivityAsync(favoriteActivity))
                .Returns(Task.CompletedTask);

            favoriteRepoMock.Setup(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId))
                .ReturnsAsync(true);

            //Act
            Func<Task> methodInTest = async () => await sut.ResolveFavoriteActivityAsync(activity);

            //Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(urm => urm.GetUserIdUsingToken(), Times.Once);
            favoriteRepoMock.Verify(frm => frm.AddFavoriteActivityAsync(favoriteActivity), Times.Never);
            favoriteRepoMock.Verify(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResolveFavoriteAsync_RemoveFailed([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
           [Frozen] Mock<IUserRepository> userRepoMock,
           UserFavoriteActivity favoriteActivity, FavoriteActivityBase activity,
           int userId, FavoritesService sut)
        {

            //Arrange

            activity.Favorite = false;

            userRepoMock.Setup(urm => urm.GetUserIdUsingToken())
              .Returns(userId);

            favoriteRepoMock.Setup(frm => frm.AddFavoriteActivityAsync(favoriteActivity))
                .Returns(Task.CompletedTask);

            favoriteRepoMock.Setup(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId))
                .ReturnsAsync(false);

            //Act
            Func<Task> methodInTest = async () => await sut.ResolveFavoriteActivityAsync(activity);

            //Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(urm => urm.GetUserIdUsingToken(), Times.Once);
            favoriteRepoMock.Verify(frm => frm.AddFavoriteActivityAsync(favoriteActivity), Times.Never);
            favoriteRepoMock.Verify(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId), Times.Once);
        }
    }
}
