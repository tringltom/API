using System;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.ServiceInterfaces;
using AutoFixture.NUnit3;
using DAL.RepositoryInterfaces;
using Domain;
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
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            FavoriteActivityBase activity,
            int userId, FavoritesService sut)
        {

            //Arrange

            var favoriteActivity = new UserFavoriteActivity() { ActivityId = activity.ActivityId, UserId = userId };

            activity.Favorite = true;

            userAccessorMock.Setup(urm => urm.GetUserIdFromAccessToken())
                .Returns(userId);

            favoriteRepoMock.Setup(frm => frm.AddFavoriteActivityAsync(favoriteActivity))
                .Returns(Task.CompletedTask);

            favoriteRepoMock.Setup(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId))
                .ReturnsAsync(true);

            //Act
            Func<Task> methodInTest = async () => await sut.ResolveFavoriteActivityAsync(activity);

            //Assert
            methodInTest.Should().NotThrow<Exception>();
            userAccessorMock.Verify(urm => urm.GetUserIdFromAccessToken(), Times.Once);
            favoriteRepoMock.Verify(frm => frm.AddFavoriteActivityAsync(It.IsAny<UserFavoriteActivity>()), Times.Once);
            favoriteRepoMock.Verify(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResolveFavoriteAsync_AddFailed([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            FavoriteActivityBase activity,
            int userId, FavoritesService sut)
        {

            //Arrange

            var favoriteActivity = new UserFavoriteActivity() { ActivityId = activity.ActivityId, UserId = userId };

            activity.Favorite = true;

            userAccessorMock.Setup(urm => urm.GetUserIdFromAccessToken())
                .Returns(userId);

            favoriteRepoMock.Setup(frm => frm.AddFavoriteActivityAsync(It.IsAny<UserFavoriteActivity>()))
                .Throws(new DbUpdateException());

            favoriteRepoMock.Setup(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId))
                .ReturnsAsync(true);

            //Act
            Func<Task> methodInTest = async () => await sut.ResolveFavoriteActivityAsync(activity);

            //Assert
            methodInTest.Should().Throw<RestException>();
            userAccessorMock.Verify(urm => urm.GetUserIdFromAccessToken(), Times.Once);
            favoriteRepoMock.Verify(frm => frm.AddFavoriteActivityAsync(It.IsAny<UserFavoriteActivity>()), Times.Once);
            favoriteRepoMock.Verify(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResolveFavoriteAsync_RemoveSuccessfull([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            FavoriteActivityBase activity,
            int userId, FavoritesService sut)
        {

            //Arrange

            var favoriteActivity = new UserFavoriteActivity() { ActivityId = activity.ActivityId, UserId = userId };

            activity.Favorite = false;

            userAccessorMock.Setup(urm => urm.GetUserIdFromAccessToken())
               .Returns(userId);

            favoriteRepoMock.Setup(frm => frm.AddFavoriteActivityAsync(favoriteActivity))
                .Returns(Task.CompletedTask);

            favoriteRepoMock.Setup(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId))
                .ReturnsAsync(true);

            //Act
            Func<Task> methodInTest = async () => await sut.ResolveFavoriteActivityAsync(activity);

            //Assert
            methodInTest.Should().NotThrow<Exception>();
            userAccessorMock.Verify(urm => urm.GetUserIdFromAccessToken(), Times.Once);
            favoriteRepoMock.Verify(frm => frm.AddFavoriteActivityAsync(favoriteActivity), Times.Never);
            favoriteRepoMock.Verify(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResolveFavoriteAsync_RemoveFailed([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
           [Frozen] Mock<IUserAccessor> userAccessorMock,
           FavoriteActivityBase activity,
           int userId, FavoritesService sut)
        {

            //Arrange

            var favoriteActivity = new UserFavoriteActivity() { ActivityId = activity.ActivityId, UserId = userId };

            activity.Favorite = false;

            userAccessorMock.Setup(urm => urm.GetUserIdFromAccessToken())
              .Returns(userId);

            favoriteRepoMock.Setup(frm => frm.AddFavoriteActivityAsync(favoriteActivity))
                .Returns(Task.CompletedTask);

            favoriteRepoMock.Setup(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId))
                .ReturnsAsync(false);

            //Act
            Func<Task> methodInTest = async () => await sut.ResolveFavoriteActivityAsync(activity);

            //Assert
            methodInTest.Should().Throw<RestException>();
            userAccessorMock.Verify(urm => urm.GetUserIdFromAccessToken(), Times.Once);
            favoriteRepoMock.Verify(frm => frm.AddFavoriteActivityAsync(favoriteActivity), Times.Never);
            favoriteRepoMock.Verify(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, favoriteActivity.ActivityId), Times.Once);
        }
    }
}
