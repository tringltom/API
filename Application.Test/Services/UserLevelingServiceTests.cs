using System;
using System.Threading.Tasks;
using Application.Errors;
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
    public class UserLevelingServiceTests
    {
        [SetUp]
        public void SetUp()
        { }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ReviewerExistsAsync_Successful(
            int reviewerId,
            [Frozen] Mock<IUserRepository> userRepositoryMock,
            User user,
            UserLevelingService sut)
        {

            // Arrange
            userRepositoryMock.Setup(x => x.GetUserByIdAsync(reviewerId))
                .ReturnsAsync(user);

            var result = new bool();

            // Act
            Func<Task> methodInTest = async () => result = await sut.ReviewerExistsAsync(reviewerId);

            // Assert
            methodInTest.Should().NotThrow<RestException>();
            result.Should().BeTrue();
            userRepositoryMock.Verify(x => x.GetUserByIdAsync(reviewerId), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ReviewerExistsAsync_Fail(
            int reviewerId,
            [Frozen] Mock<IUserRepository> userRepositoryMock,
            UserLevelingService sut)
        {

            // Arrange
            userRepositoryMock.Setup(x => x.GetUserByIdAsync(reviewerId))
                .ReturnsAsync((User)null);

            var result = new bool();

            // Act
            Func<Task> methodInTest = async () => result = await sut.ReviewerExistsAsync(reviewerId);

            // Assert
            methodInTest.Should().NotThrow<RestException>();
            result.Should().BeFalse();
            userRepositoryMock.Verify(x => x.GetUserByIdAsync(reviewerId), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetXpRewardYieldByReviewAsync_Successful(
            UserReview userReview,
            [Frozen] Mock<IActivityReviewXpRepository> activityReviewXpRepositoryMock,
            int xpReward,
            UserLevelingService sut)
        {
            // Arrange
            var result = new int();

            activityReviewXpRepositoryMock.Setup(x => x.GetXpRewardByActivityAndReviewTypeIdsAsync(userReview.Activity.ActivityTypeId, userReview.ReviewTypeId))
                .ReturnsAsync(xpReward);

            // Act
            Func<Task> methodInTest = async () => result = await sut.GetXpRewardYieldByReviewAsync(userReview);

            // Assert
            methodInTest.Should().NotThrow<RestException>();
            result.Should().Equals(xpReward);
            activityReviewXpRepositoryMock.Verify(x => x.GetXpRewardByActivityAndReviewTypeIdsAsync(userReview.Activity.ActivityTypeId, userReview.ReviewTypeId), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void UpdateUserXpAsync_Successful(
           int amount,
           User user,
           [Frozen] Mock<IUserRepository> userRepositoryMock,
           UserLevelingService sut)
        {
            // Arrange
            userRepositoryMock.Setup(x => x.GetUserByIdAsync(user.Id))
                .ReturnsAsync(user);

            userRepositoryMock.Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.UpdateUserXpAsync(amount, user.Id);

            // Assert
            methodInTest.Should().NotThrow<RestException>();
            userRepositoryMock.Verify(x => x.GetUserByIdAsync(user.Id), Times.Once);
            userRepositoryMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void UpdateUserXpAsync_Failed(
           int amount,
           User user,
           [Frozen] Mock<IUserRepository> userRepositoryMock,
           UserLevelingService sut)
        {
            // Arrange
            userRepositoryMock.Setup(x => x.GetUserByIdAsync(user.Id))
                .ThrowsAsync(new Exception());

            userRepositoryMock.Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.UpdateUserXpAsync(amount, user.Id);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepositoryMock.Verify(x => x.GetUserByIdAsync(user.Id), Times.Once);
            userRepositoryMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
        }
    }
}
