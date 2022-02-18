using System;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using AutoFixture.NUnit3;
using DAL;
using Domain;
using FixtureShared;
using FluentAssertions;
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
        public void ResolveFavoriteAsync_CreateSuccessfull([Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            FavoriteActivityBase activity,
            int userId, FavoritesService sut)
        {

            //Arrange

            var favoriteActivity = new UserFavoriteActivity() { ActivityId = activity.ActivityId, UserId = userId };

            activity.Favorite = true;

            userAccessorMock.Setup(urm => urm.GetUserIdFromAccessToken())
                .Returns(userId);

            uowMock.Setup(frm => frm.CompleteAsync())
                .ReturnsAsync(true);

            //Act
            Func<Task> methodInTest = async () => await sut.ResolveFavoriteActivityAsync(activity);

            //Assert
            methodInTest.Should().NotThrow<Exception>();
            userAccessorMock.Verify(urm => urm.GetUserIdFromAccessToken(), Times.Once);
            uowMock.Verify(frm => frm.UserFavorites.Add(It.IsAny<UserFavoriteActivity>()), Times.Once);
            uowMock.Verify(frm => frm.UserFavorites.Remove(It.IsAny<UserFavoriteActivity>()), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResolveFavoriteAsync_AddFailed([Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            FavoriteActivityBase activity,
            int userId, FavoritesService sut)
        {

            //Arrange

            var favoriteActivity = new UserFavoriteActivity() { ActivityId = activity.ActivityId, UserId = userId };

            activity.Favorite = true;

            userAccessorMock.Setup(urm => urm.GetUserIdFromAccessToken())
                .Returns(userId);

            uowMock.Setup(frm => frm.CompleteAsync())
                .Throws(new RestException(HttpStatusCode.BadRequest, new { FavoriteActivity = "Greška, korisnik i/ili aktivnost su nepostojeći." }));

            //Act
            Func<Task> methodInTest = async () => await sut.ResolveFavoriteActivityAsync(activity);

            //Assert
            methodInTest.Should().Throw<RestException>();
            userAccessorMock.Verify(urm => urm.GetUserIdFromAccessToken(), Times.Once);
            uowMock.Verify(frm => frm.UserFavorites.Add(It.IsAny<UserFavoriteActivity>()), Times.Once);
            uowMock.Verify(frm => frm.UserFavorites.Remove(It.IsAny<UserFavoriteActivity>()), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResolveFavoriteAsync_RemoveSuccessfull([Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            FavoriteActivityBase activity,
            int userId, FavoritesService sut)
        {

            //Arrange

            var favoriteActivity = new UserFavoriteActivity() { ActivityId = activity.ActivityId, UserId = userId };

            activity.Favorite = false;

            userAccessorMock.Setup(urm => urm.GetUserIdFromAccessToken())
               .Returns(userId);

            uowMock.Setup(frm => frm.CompleteAsync())
                .ReturnsAsync(true);

            //Act
            Func<Task> methodInTest = async () => await sut.ResolveFavoriteActivityAsync(activity);

            //Assert
            methodInTest.Should().NotThrow<Exception>();
            userAccessorMock.Verify(urm => urm.GetUserIdFromAccessToken(), Times.Once);
            uowMock.Verify(frm => frm.UserFavorites.Add(It.IsAny<UserFavoriteActivity>()), Times.Never);
            uowMock.Verify(frm => frm.UserFavorites.Remove(It.IsAny<UserFavoriteActivity>()), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResolveFavoriteAsync_RemoveFailed([Frozen] Mock<IUnitOfWork> uowMock,
           [Frozen] Mock<IUserAccessor> userAccessorMock,
           FavoriteActivityBase activity,
           int userId, FavoritesService sut)
        {

            //Arrange

            var favoriteActivity = new UserFavoriteActivity() { ActivityId = activity.ActivityId, UserId = userId };

            activity.Favorite = false;

            userAccessorMock.Setup(urm => urm.GetUserIdFromAccessToken())
              .Returns(userId);

            uowMock.Setup(frm => frm.CompleteAsync())
                .Throws(new RestException(HttpStatusCode.BadRequest, new { FavoriteActivity = "Greška, korisnik i/ili aktivnost su nepostojeći." }));

            //Act
            Func<Task> methodInTest = async () => await sut.ResolveFavoriteActivityAsync(activity);

            //Assert
            methodInTest.Should().Throw<RestException>();
            userAccessorMock.Verify(urm => urm.GetUserIdFromAccessToken(), Times.Once);
            uowMock.Verify(frm => frm.UserFavorites.Add(It.IsAny<UserFavoriteActivity>()), Times.Never);
            uowMock.Verify(frm => frm.UserFavorites.Remove(It.IsAny<UserFavoriteActivity>()), Times.Once);
        }
    }
}
