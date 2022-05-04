using System;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.Services;
using AutoFixture;
using DAL;
using Domain;
using FixtureShared;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class DiceServiceTests
    {
        private IFixture _fixture;
        private Mock<IUserAccessor> _userAccessorMock;
        private Mock<IUnitOfWork> _uowMock;
        private DiceService _sut;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithAutoMoqAndOmitRecursion();
            _userAccessorMock = new Mock<IUserAccessor>();
            _uowMock = new Mock<IUnitOfWork>();
            _sut = new DiceService(_userAccessorMock.Object, _uowMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task Roll_SuccessfulAsync()
        {

            // Arrange
            var eligableUser = _fixture
              .Build<User>()
              .With(u => u.LastRollDate, DateTimeOffset.Now.AddDays(-2))
              .Create();

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(eligableUser.Id);

            _uowMock.Setup(x => x.Users.GetAsync(eligableUser.Id))
                .ReturnsAsync(eligableUser);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.RollAsync();

            // Assert
            res.Match(
                diceResult => diceResult.Result.Should().BePositive(),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(eligableUser.Id), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task Roll_RollCountExceedAsync()
        {

            // Arrange
            var nonEligableUser = _fixture
              .Build<User>()
              .With(u => u.LastRollDate, DateTimeOffset.Now.AddMinutes(-20))
              .Create();

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(nonEligableUser.Id);

            _uowMock.Setup(x => x.Users.GetAsync(nonEligableUser.Id))
                .ReturnsAsync(nonEligableUser);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.RollAsync();

            // Assert
            res.Match(
                diceResult => diceResult.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(nonEligableUser.Id), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }
    }
}
