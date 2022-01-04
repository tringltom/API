using System;
using Application.RepositoryInterfaces;
using Application.Services;
using AutoFixture;
using AutoFixture.NUnit3;
using Domain.Entities;
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
            [Frozen] Mock<IUserRepository> userRepoMock,
            DiceService sut)
        {

            // Arrange
            var eligableUser = _fixture
              .Build<User>()
              .With(u => u.LastRollDate, DateTimeOffset.Now.AddDays(-2))
              .Create();

            userRepoMock.Setup(x => x.GetUserByTokenAsync())
                .ReturnsAsync(eligableUser);

            userRepoMock.Setup(x => x.UpdateUserAsync(eligableUser))
                .ReturnsAsync(true);

            // Act
            var diceResult = sut.GetDiceRollResult().Result;

            // Assert
            diceResult.Result.Should().BePositive();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetDiceRollResultAsync_Unsuccessful(
        [Frozen] Mock<IUserRepository> userRepoMock,
        DiceService sut)
        {

            // Arrange
            var nonEligableUser = _fixture
              .Build<User>()
              .With(u => u.LastRollDate, DateTimeOffset.Now.AddMinutes(-20))
              .Create();

            userRepoMock.Setup(x => x.GetUserByTokenAsync())
                .ReturnsAsync(nonEligableUser);

            // Act
            var diceResult = sut.GetDiceRollResult().Result;

            // Assert
            diceResult.Result.Should().Be(0);
        }
    }
}
