using System;
using System.Threading.Tasks;
using Application.RepositoryInterfaces;
using Application.ServiceInterfaces;
using AutoFixture.NUnit3;
using AutoMapper;
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
        public void CreateFavoriteAsync_Successfull([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
            [Frozen] Mock<IMapper> mapperMock, [Frozen] Mock<IUserRepository> userRepoMock, User user,
            UserFavoriteActivity favoriteActivity, FavoriteActivityBase activity,
            FavoritesService sut)
        {

            //Arrange
            activity.UserId = user.Id;

            userRepoMock.Setup(urm => urm.GetCurrentUsername())
                .Returns(user.UserName);

            userRepoMock.Setup(urm => urm.GetUserByUserNameAsync(user.UserName))
                .ReturnsAsync(user);

            mapperMock.Setup(mm => mm.Map<UserFavoriteActivity>(activity))
                .Returns(favoriteActivity);

            favoriteRepoMock.Setup(frm => frm.GetFavoriteActivityAsync(favoriteActivity))
                .ReturnsAsync((UserFavoriteActivity)null);

            favoriteRepoMock.Setup(frm => frm.AddFavoriteActivityAsync(favoriteActivity))
                .Returns(Task.CompletedTask);

            //Act
            Func<Task> methodInTest = async () => await sut.CreateFavoriteAsync(activity);

            //Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(urm => urm.GetUserByUserNameAsync(user.UserName), Times.Once);
            mapperMock.Verify(mm => mm.Map<UserFavoriteActivity>(activity), Times.Once);
            favoriteRepoMock.Verify(frm => frm.GetFavoriteActivityAsync(favoriteActivity), Times.Once);
            favoriteRepoMock.Verify(frm => frm.AddFavoriteActivityAsync(favoriteActivity), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void CreateFavoriteAsync_AddFailed([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
            [Frozen] Mock<IMapper> mapperMock, [Frozen] Mock<IUserRepository> userRepoMock, User user,
            UserFavoriteActivity favoriteActivity, FavoriteActivityBase activity,
            FavoritesService sut)
        {

            //Arrange
            activity.UserId = user.Id;

            userRepoMock.Setup(urm => urm.GetCurrentUsername())
                .Returns(user.UserName);

            userRepoMock.Setup(urm => urm.GetUserByUserNameAsync(user.UserName))
                .ReturnsAsync(user);

            mapperMock.Setup(mm => mm.Map<UserFavoriteActivity>(activity))
                .Returns(favoriteActivity);

            favoriteRepoMock.Setup(frm => frm.GetFavoriteActivityAsync(favoriteActivity))
                .ReturnsAsync((UserFavoriteActivity)null);

            favoriteRepoMock.Setup(frm => frm.AddFavoriteActivityAsync(favoriteActivity))
                .Throws(new DbUpdateException());

            //Act
            Func<Task> methodInTest = async () => await sut.CreateFavoriteAsync(activity);

            //Assert
            methodInTest.Should().Throw<Exception>();
            userRepoMock.Verify(urm => urm.GetUserByUserNameAsync(user.UserName), Times.Once);
            mapperMock.Verify(mm => mm.Map<UserFavoriteActivity>(activity), Times.Once);
            favoriteRepoMock.Verify(frm => frm.GetFavoriteActivityAsync(favoriteActivity), Times.Once);
            favoriteRepoMock.Verify(frm => frm.AddFavoriteActivityAsync(favoriteActivity), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void CreateFavoriteAsync_ActivityAlreadyExists([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
            [Frozen] Mock<IMapper> mapperMock, [Frozen] Mock<IUserRepository> userRepoMock, User user,
            UserFavoriteActivity favoriteActivity, FavoriteActivityBase activity,
            FavoritesService sut)
        {

            //Arrange
            activity.UserId = user.Id;

            userRepoMock.Setup(urm => urm.GetCurrentUsername())
                .Returns(user.UserName);

            userRepoMock.Setup(urm => urm.GetUserByUserNameAsync(user.UserName))
                .ReturnsAsync(user);

            mapperMock.Setup(mm => mm.Map<UserFavoriteActivity>(activity))
                .Returns(favoriteActivity);

            favoriteRepoMock.Setup(frm => frm.GetFavoriteActivityAsync(favoriteActivity))
                .ReturnsAsync(favoriteActivity);

            favoriteRepoMock.Setup(frm => frm.AddFavoriteActivityAsync(favoriteActivity))
                .Returns(Task.CompletedTask);

            //Act
            Func<Task> methodInTest = async () => await sut.CreateFavoriteAsync(activity);

            //Assert
            methodInTest.Should().Throw<Exception>();
            userRepoMock.Verify(urm => urm.GetUserByUserNameAsync(user.UserName), Times.Once);
            mapperMock.Verify(mm => mm.Map<UserFavoriteActivity>(activity), Times.Once);
            favoriteRepoMock.Verify(frm => frm.GetFavoriteActivityAsync(favoriteActivity), Times.Once);
            favoriteRepoMock.Verify(frm => frm.AddFavoriteActivityAsync(favoriteActivity), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void CreateFavoriteAsync_IncorrectUserIds([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
           [Frozen] Mock<IMapper> mapperMock, [Frozen] Mock<IUserRepository> userRepoMock, User user,
           UserFavoriteActivity favoriteActivity, FavoriteActivityBase activity,
           FavoritesService sut)
        {

            //Arrange
            activity.UserId = user.Id + 1;

            userRepoMock.Setup(urm => urm.GetCurrentUsername())
                .Returns(user.UserName);

            userRepoMock.Setup(urm => urm.GetUserByUserNameAsync(user.UserName))
                .ReturnsAsync(user);

            mapperMock.Setup(mm => mm.Map<UserFavoriteActivity>(activity))
                .Returns(favoriteActivity);

            favoriteRepoMock.Setup(frm => frm.GetFavoriteActivityAsync(favoriteActivity))
                .ReturnsAsync((UserFavoriteActivity)null);

            favoriteRepoMock.Setup(frm => frm.AddFavoriteActivityAsync(favoriteActivity))
                .Returns(Task.CompletedTask);

            //Act
            Func<Task> methodInTest = async () => await sut.CreateFavoriteAsync(activity);

            //Assert
            methodInTest.Should().Throw<Exception>();
            userRepoMock.Verify(urm => urm.GetUserByUserNameAsync(user.UserName), Times.Once);
            mapperMock.Verify(mm => mm.Map<UserFavoriteActivity>(activity), Times.Never);
            favoriteRepoMock.Verify(frm => frm.GetFavoriteActivityAsync(favoriteActivity), Times.Never);
            favoriteRepoMock.Verify(frm => frm.AddFavoriteActivityAsync(favoriteActivity), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RemoveFavoriteAsync_Successfull([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
          [Frozen] Mock<IUserRepository> userRepoMock, User user,
          FavoriteActivityBase favoriteActivity,
          FavoritesService sut)
        {

            //Arrange
            favoriteActivity.UserId = user.Id;

            userRepoMock.Setup(urm => urm.GetCurrentUsername())
                .Returns(user.UserName);

            userRepoMock.Setup(urm => urm.GetUserByUserNameAsync(user.UserName))
                .ReturnsAsync(user);

            favoriteRepoMock.Setup(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(favoriteActivity.UserId, favoriteActivity.ActivityId))
                .ReturnsAsync(true);

            //Act
            Func<Task> methodInTest = async () => await sut.RemoveFavoriteAsync(favoriteActivity);

            //Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(urm => urm.GetUserByUserNameAsync(user.UserName), Times.Once);
            favoriteRepoMock.Verify(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(favoriteActivity.UserId, favoriteActivity.ActivityId), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RemoveFavoriteAsync_ActivityNotFound([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
          [Frozen] Mock<IUserRepository> userRepoMock, User user,
          FavoriteActivityBase favoriteActivity,
          FavoritesService sut)
        {

            //Arrange
            favoriteActivity.UserId = user.Id;

            userRepoMock.Setup(urm => urm.GetCurrentUsername())
                .Returns(user.UserName);

            userRepoMock.Setup(urm => urm.GetUserByUserNameAsync(user.UserName))
                .ReturnsAsync(user);

            favoriteRepoMock.Setup(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(favoriteActivity.UserId, favoriteActivity.ActivityId))
                .ReturnsAsync(false);

            //Act
            Func<Task> methodInTest = async () => await sut.RemoveFavoriteAsync(favoriteActivity);

            //Assert
            methodInTest.Should().Throw<Exception>();
            userRepoMock.Verify(urm => urm.GetUserByUserNameAsync(user.UserName), Times.Once);
            favoriteRepoMock.Verify(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(favoriteActivity.UserId, favoriteActivity.ActivityId), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RemoveFavoriteAsync_IncorrectUserId([Frozen] Mock<IFavoritesRepository> favoriteRepoMock,
          [Frozen] Mock<IUserRepository> userRepoMock, User user,
          FavoriteActivityBase favoriteActivity,
          FavoritesService sut)
        {

            //Arrange
            favoriteActivity.UserId = user.Id + 1;

            userRepoMock.Setup(urm => urm.GetCurrentUsername())
                .Returns(user.UserName);

            userRepoMock.Setup(urm => urm.GetUserByUserNameAsync(user.UserName))
                .ReturnsAsync(user);

            favoriteRepoMock.Setup(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(favoriteActivity.UserId, favoriteActivity.ActivityId))
                .ReturnsAsync(true);

            //Act
            Func<Task> methodInTest = async () => await sut.RemoveFavoriteAsync(favoriteActivity);

            //Assert
            methodInTest.Should().Throw<Exception>();
            userRepoMock.Verify(urm => urm.GetUserByUserNameAsync(user.UserName), Times.Once);
            favoriteRepoMock.Verify(frm => frm.RemoveFavoriteActivityByActivityAndUserIdAsync(favoriteActivity.UserId, favoriteActivity.ActivityId), Times.Never);
        }

    }
}
