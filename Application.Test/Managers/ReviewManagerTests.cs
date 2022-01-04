using System;
using System.Threading.Tasks;
using Application.Errors;
using Application.Managers;
using Application.ServiceInterfaces;
using AutoFixture;
using AutoFixture.NUnit3;
using AutoMapper;
using Domain.Entities;
using FixtureShared;
using FluentAssertions;
using Models.Activity;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Managers
{
    public class ReviewManagerTests
    {
        [SetUp]
        public void SetUp()
        { }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ReviewActivityAsync_NoExistingReview_Successful(
            ActivityReview activityReview,
            UserReview review,
            Activity activity,
            int xpToYield,
            [Frozen] Mock<IActivityReviewService> activityReviewServiceMock,
            [Frozen] Mock<IUserLevelingService> userLevelingServiceMock,
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IActivityService> activityServiceMock,
            ReviewManager sut)
        {
            // Arrange
            activity.User.Id = activityReview.UserId + 1;

            userLevelingServiceMock.Setup(x => x.ReviewerExistsAsync(activityReview.UserId))
                .ReturnsAsync(true);

            userLevelingServiceMock.Setup(x => x.GetXpRewardYieldByReviewAsync(review))
                .ReturnsAsync(xpToYield);

            userLevelingServiceMock.Setup(x => x.UpdateUserXpAsync(xpToYield, activity.User.Id))
                .Returns(Task.CompletedTask);

            mapperMock.Setup(x => x.Map<UserReview>(activityReview))
                .Returns(review);

            activityServiceMock.Setup(x => x.GetActivitityUserIdByActivityId(activityReview.ActivityId))
                .ReturnsAsync(activity);

            activityReviewServiceMock.Setup(x => x.GetUserReviewByActivityAndUserIds(activityReview.ActivityId, activityReview.UserId))
                .ReturnsAsync((UserReview)null);

            activityReviewServiceMock.Setup(x => x.AddReviewActivityAsync(activityReview))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.ReviewActivityAsync(activityReview);

            // Assert
            methodInTest.Should().NotThrow<RestException>();

            userLevelingServiceMock.Verify(x => x.ReviewerExistsAsync(activityReview.UserId), Times.Once);
            userLevelingServiceMock.Verify(x => x.GetXpRewardYieldByReviewAsync(It.IsAny<UserReview>()), Times.Once);
            userLevelingServiceMock.Verify(x => x.UpdateUserXpAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            mapperMock.Verify(x => x.Map<UserReview>(activityReview), Times.Once);
            activityServiceMock.Verify(x => x.GetActivitityUserIdByActivityId(activityReview.ActivityId), Times.Once);
            activityReviewServiceMock.Verify(x => x.GetUserReviewByActivityAndUserIds(activityReview.ActivityId, activityReview.UserId), Times.Once);
            activityReviewServiceMock.Verify(x => x.AddReviewActivityAsync(activityReview), Times.Once);

            activityReviewServiceMock.Verify(x => x.UpdateReviewActivityAsync(activityReview), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ReviewActivityAsync_ExistingReview_Successful(
            ActivityReview activityReview,
            UserReview review,
            UserReview userReview,
            Activity activity,
            int xpToYield,
            int xpYielded,
            [Frozen] Mock<IActivityReviewService> activityReviewServiceMock,
            [Frozen] Mock<IUserLevelingService> userLevelingServiceMock,
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IActivityService> activityServiceMock,
            ReviewManager sut)
        {
            // Arrange
            activity.User.Id = activityReview.UserId + 1;
            xpToYield = xpYielded + 1;

            userLevelingServiceMock.Setup(x => x.ReviewerExistsAsync(activityReview.UserId))
                .ReturnsAsync(true);

            userLevelingServiceMock.Setup(x => x.GetXpRewardYieldByReviewAsync(review))
                .ReturnsAsync(xpToYield);

            userLevelingServiceMock.Setup(x => x.GetXpRewardYieldByReviewAsync(userReview))
                .ReturnsAsync(xpYielded);

            userLevelingServiceMock.Setup(x => x.UpdateUserXpAsync(xpToYield - xpYielded, activity.User.Id))
                .Returns(Task.CompletedTask);

            mapperMock.Setup(x => x.Map<UserReview>(activityReview))
                .Returns(review);

            activityServiceMock.Setup(x => x.GetActivitityUserIdByActivityId(activityReview.ActivityId))
                .ReturnsAsync(activity);

            activityReviewServiceMock.Setup(x => x.GetUserReviewByActivityAndUserIds(activityReview.ActivityId, activityReview.UserId))
                .ReturnsAsync(userReview);

            activityReviewServiceMock.Setup(x => x.UpdateReviewActivityAsync(activityReview))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.ReviewActivityAsync(activityReview);

            // Assert
            methodInTest.Should().NotThrow<RestException>();

            userLevelingServiceMock.Verify(x => x.ReviewerExistsAsync(activityReview.UserId), Times.Once);
            userLevelingServiceMock.Verify(x => x.GetXpRewardYieldByReviewAsync(It.IsAny<UserReview>()), Times.Exactly(2));
            userLevelingServiceMock.Verify(x => x.UpdateUserXpAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            mapperMock.Verify(x => x.Map<UserReview>(activityReview), Times.Once);
            activityServiceMock.Verify(x => x.GetActivitityUserIdByActivityId(activityReview.ActivityId), Times.Once);
            activityReviewServiceMock.Verify(x => x.GetUserReviewByActivityAndUserIds(activityReview.ActivityId, activityReview.UserId), Times.Once);
            activityReviewServiceMock.Verify(x => x.UpdateReviewActivityAsync(activityReview), Times.Once);

            activityReviewServiceMock.Verify(x => x.AddReviewActivityAsync(activityReview), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ReviewActivityAsync_ExistingReviewRewardTheSame_Successful(
            ActivityReview activityReview,
            UserReview review,
            UserReview userReview,
            Activity activity,
            int xp,
            [Frozen] Mock<IActivityReviewService> activityReviewServiceMock,
            [Frozen] Mock<IUserLevelingService> userLevelingServiceMock,
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IActivityService> activityServiceMock,
            ReviewManager sut)
        {
            // Arrange
            activity.User.Id = activityReview.UserId + 1;

            userLevelingServiceMock.Setup(x => x.ReviewerExistsAsync(activityReview.UserId))
                .ReturnsAsync(true);

            userLevelingServiceMock.Setup(x => x.GetXpRewardYieldByReviewAsync(review))
                .ReturnsAsync(xp);

            userLevelingServiceMock.Setup(x => x.GetXpRewardYieldByReviewAsync(userReview))
                .ReturnsAsync(xp);

            mapperMock.Setup(x => x.Map<UserReview>(activityReview))
                .Returns(review);

            activityServiceMock.Setup(x => x.GetActivitityUserIdByActivityId(activityReview.ActivityId))
                .ReturnsAsync(activity);

            activityReviewServiceMock.Setup(x => x.GetUserReviewByActivityAndUserIds(activityReview.ActivityId, activityReview.UserId))
                .ReturnsAsync(userReview);

            // Act
            Func<Task> methodInTest = async () => await sut.ReviewActivityAsync(activityReview);

            // Assert
            methodInTest.Should().NotThrow<RestException>();

            userLevelingServiceMock.Verify(x => x.ReviewerExistsAsync(activityReview.UserId), Times.Once);
            userLevelingServiceMock.Verify(x => x.GetXpRewardYieldByReviewAsync(It.IsAny<UserReview>()), Times.Exactly(2));
            mapperMock.Verify(x => x.Map<UserReview>(activityReview), Times.Once);
            activityServiceMock.Verify(x => x.GetActivitityUserIdByActivityId(activityReview.ActivityId), Times.Once);
            activityReviewServiceMock.Verify(x => x.GetUserReviewByActivityAndUserIds(activityReview.ActivityId, activityReview.UserId), Times.Once);

            userLevelingServiceMock.Verify(x => x.UpdateUserXpAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            activityReviewServiceMock.Verify(x => x.UpdateReviewActivityAsync(activityReview), Times.Never);
            activityReviewServiceMock.Verify(x => x.AddReviewActivityAsync(activityReview), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ReviewActivityAsync_ReviewerDoesNotExist_Fail(
            ActivityReview activityReview,
            Activity activity,
            int xpToYield,
            int xpYielded,
            [Frozen] Mock<IActivityReviewService> activityReviewServiceMock,
            [Frozen] Mock<IUserLevelingService> userLevelingServiceMock,
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IActivityService> activityServiceMock,
            ReviewManager sut)
        {
            // Arrange
            activity.User.Id = activityReview.UserId + 1;
            xpToYield = xpYielded + 1;

            userLevelingServiceMock.Setup(x => x.ReviewerExistsAsync(activityReview.UserId))
                .ReturnsAsync(false);

            // Act
            Func<Task> methodInTest = async () => await sut.ReviewActivityAsync(activityReview);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userLevelingServiceMock.Verify(x => x.ReviewerExistsAsync(activityReview.UserId), Times.Once);

            userLevelingServiceMock.Verify(x => x.GetXpRewardYieldByReviewAsync(It.IsAny<UserReview>()), Times.Never);
            userLevelingServiceMock.Verify(x => x.UpdateUserXpAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            mapperMock.Verify(x => x.Map<UserReview>(activityReview), Times.Never);
            activityServiceMock.Verify(x => x.GetActivitityUserIdByActivityId(activityReview.ActivityId), Times.Never);
            activityReviewServiceMock.Verify(x => x.GetUserReviewByActivityAndUserIds(activityReview.ActivityId, activityReview.UserId), Times.Never);
            activityReviewServiceMock.Verify(x => x.UpdateReviewActivityAsync(activityReview), Times.Never);
            activityReviewServiceMock.Verify(x => x.AddReviewActivityAsync(activityReview), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ReviewActivityAsync_ReviewerIsActivityCreator_Fail(
            ActivityReview activityReview,
            Activity activity,
            UserReview review,
            [Frozen] Mock<IActivityReviewService> activityReviewServiceMock,
            [Frozen] Mock<IUserLevelingService> userLevelingServiceMock,
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IActivityService> activityServiceMock,
            ReviewManager sut)
        {
            // Arrange
            activity.User.Id = activityReview.UserId;

            userLevelingServiceMock.Setup(x => x.ReviewerExistsAsync(activityReview.UserId))
                .ReturnsAsync(true);

            mapperMock.Setup(x => x.Map<UserReview>(activityReview))
                .Returns(review);

            activityServiceMock.Setup(x => x.GetActivitityUserIdByActivityId(activityReview.ActivityId))
                .ReturnsAsync(activity);

            // Act
            Func<Task> methodInTest = async () => await sut.ReviewActivityAsync(activityReview);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userLevelingServiceMock.Verify(x => x.ReviewerExistsAsync(activityReview.UserId), Times.Once);
            mapperMock.Verify(x => x.Map<UserReview>(activityReview), Times.Once);
            activityServiceMock.Verify(x => x.GetActivitityUserIdByActivityId(activityReview.ActivityId), Times.Once);

            userLevelingServiceMock.Verify(x => x.GetXpRewardYieldByReviewAsync(It.IsAny<UserReview>()), Times.Never);
            userLevelingServiceMock.Verify(x => x.UpdateUserXpAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            activityReviewServiceMock.Verify(x => x.GetUserReviewByActivityAndUserIds(activityReview.ActivityId, activityReview.UserId), Times.Never);
            activityReviewServiceMock.Verify(x => x.UpdateReviewActivityAsync(activityReview), Times.Never);
            activityReviewServiceMock.Verify(x => x.AddReviewActivityAsync(activityReview), Times.Never);
        }
    }
}
