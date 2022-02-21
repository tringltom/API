using System;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.Services;
using AutoFixture;
using AutoFixture.NUnit3;
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
            [Frozen] Mock<IUserManager> userManagerRepoMock,
            DiceService sut)
        {

            // Arrange
            var eligableUser = _fixture
              .Build<User>()
              .With(u => u.LastRollDate, DateTimeOffset.Now.AddDays(-2))
              .Create();

            userAccessorMock.Setup(x => x.FindUserFromAccessToken())
                .ReturnsAsync(eligableUser);

            userManagerRepoMock.Setup(x => x.UpdateUserAsync(eligableUser))
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
        DiceService sut)
        {

            // Arrange
            var nonEligableUser = _fixture
              .Build<User>()
              .With(u => u.LastRollDate, DateTimeOffset.Now.AddMinutes(-20))
              .Create();

            userAccessorMock.Setup(x => x.FindUserFromAccessToken())
                .ReturnsAsync(nonEligableUser);

            // Act
            var diceResult = sut.GetDiceRollResult().Result;

            // Assert
            diceResult.Result.Should().Be(0);
        }
    }
}
