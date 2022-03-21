using System;
using Application.InfrastructureInterfaces.Security;
using Application.Services;
using AutoFixture;
using AutoFixture.NUnit3;
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

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithAutoMoqAndOmitRecursion();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetDiceRollResultAsync_Successful(
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            int userId,
            DiceService sut)
        {

            // Arrange
            var eligableUser = _fixture
              .Build<User>()
              .With(u => u.LastRollDate, DateTimeOffset.Now.AddDays(-2))
              .Create();

            userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(eligableUser);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var diceResult = sut.GetDiceRollResult().Result;

            // Assert
            diceResult.Result.Should().BePositive();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetDiceRollResultAsync_Unsuccessful(
        [Frozen] Mock<IUserAccessor> userAccessorMock,
        [Frozen] Mock<IUnitOfWork> uowMock,
        int userId,
        DiceService sut)
        {

            // Arrange
            var nonEligableUser = _fixture
              .Build<User>()
              .With(u => u.LastRollDate, DateTimeOffset.Now.AddMinutes(-20))
              .Create();

            userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(nonEligableUser);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var diceResult = sut.GetDiceRollResult().Result;

            // Assert
            diceResult.Result.Should().Be(0);
        }
    }
}
