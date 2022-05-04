using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using AutoFixture;
using AutoMapper;
using DAL;
using Domain;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class FavoriteServiceTests
    {
        private IFixture _fixture;
        private Mock<IMapper> _mapperMock;
        private Mock<IUserAccessor> _userAccessorMock;
        private Mock<IUnitOfWork> _uowMock;
        private FavoritesService _sut;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithAutoMoqAndOmitRecursion();
            _userAccessorMock = new Mock<IUserAccessor>();
            _mapperMock = new Mock<IMapper>();
            _uowMock = new Mock<IUnitOfWork>();
            _sut = new FavoritesService(_mapperMock.Object, _userAccessorMock.Object, _uowMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetFavoriteActivity_SuccessfullAsync(UserFavoriteActivity userFavoriteActivity,
            UserFavoriteActivityReturn userFavoriteActivityReturn)
        {
            // Arrange
            _uowMock.Setup(x => x.UserFavorites.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(userFavoriteActivity);

            _mapperMock
                .Setup(x => x.Map<UserFavoriteActivityReturn>(userFavoriteActivity))
                .Returns(userFavoriteActivityReturn);

            // Act
            var res = await _sut.GetFavoriteActivityAsync(It.IsAny<int>());

            // Assert
            res.Should().BeEquivalentTo(userFavoriteActivityReturn);
            _uowMock.Verify(x => x.UserFavorites.GetAsync(It.IsAny<int>()), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetAllOwnerFavoriteIds_SuccessfullAsync(int userId,
            IEnumerable<UserFavoriteActivity> userFavoriteActivities,
            List<FavoriteActivityIdReturn> favoriteIds)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserFavorites.GetFavoriteActivitiesAsync(userId))
                .ReturnsAsync(userFavoriteActivities);

            _mapperMock
                .Setup(x => x.Map<List<FavoriteActivityIdReturn>>(userFavoriteActivities))
                .Returns(favoriteIds);

            // Act
            var res = await _sut.GetAllOwnerFavoriteIdsAsync();

            // Assert
            res.Should().BeEquivalentTo(favoriteIds);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserFavorites.GetFavoriteActivitiesAsync(userId), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task RemoveFavoriteActivity_SuccessfullAsync(int activityId,
            int userId,
            UserFavoriteActivity userFavoriteActivity)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserFavorites.GetFavoriteActivityAsync(userId, activityId))
                .ReturnsAsync(userFavoriteActivity);

            _uowMock.Setup(x => x.UserFavorites.Remove(userFavoriteActivity))
                .Verifiable();

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.RemoveFavoriteActivityAsync(activityId);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserFavorites.GetFavoriteActivityAsync(userId, activityId), Times.Once);
            _uowMock.Verify(x => x.UserFavorites.Remove(userFavoriteActivity), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task RemoveFavoriteActivity_FavoriteActivityNotFoundAsync(int activityId,
            int userId,
            UserFavoriteActivity userFavoriteActivity)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserFavorites.GetFavoriteActivityAsync(userId, activityId))
                .ReturnsAsync((UserFavoriteActivity)null);

            // Act
            var res = await _sut.RemoveFavoriteActivityAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserFavorites.GetFavoriteActivityAsync(userId, activityId), Times.Never);
            _uowMock.Verify(x => x.UserFavorites.Remove(userFavoriteActivity), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AddFavoriteActivityAsync_SuccessfullAsync(int activityId,
            int userId,
            UserFavoriteActivityReturn userFavoriteActivityReturn)
        {
            // Arrange
            userFavoriteActivityReturn.ActivityId = activityId;
            userFavoriteActivityReturn.UserId = userId;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserFavorites.GetFavoriteActivityAsync(userId, activityId))
                .ReturnsAsync((UserFavoriteActivity)null);

            _uowMock.Setup(x => x.UserFavorites.Add(It.IsAny<UserFavoriteActivity>()))
                .Verifiable();

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock.Setup(x => x.Map<UserFavoriteActivityReturn>(It.IsAny<UserFavoriteActivity>()))
                .Returns(userFavoriteActivityReturn);

            // Act
            var res = await _sut.AddFavoriteActivityAsync(activityId);

            // Assert
            res.Match(
                favoriteActivityreturn => favoriteActivityreturn.Should().BeEquivalentTo(userFavoriteActivityReturn),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserFavorites.GetFavoriteActivityAsync(userId, activityId), Times.Once);
            _uowMock.Verify(x => x.UserFavorites.Add(It.IsAny<UserFavoriteActivity>()), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AddFavoriteActivityAsync_ActivityAlreadyFavoriteAsync(int activityId,
            int userId,
            UserFavoriteActivity userFavoriteActivity)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserFavorites.GetFavoriteActivityAsync(userId, activityId))
                .ReturnsAsync(userFavoriteActivity);

            // Act
            var res = await _sut.AddFavoriteActivityAsync(activityId);

            // Assert
            res.Match(
                favoriteActivityreturn => favoriteActivityreturn.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserFavorites.GetFavoriteActivityAsync(userId, activityId), Times.Once);
            _uowMock.Verify(x => x.UserFavorites.Add(userFavoriteActivity), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }
    }
}
