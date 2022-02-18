using System;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using AutoFixture;
using AutoFixture.NUnit3;
using AutoMapper;
using DAL;
using Domain;
using FixtureShared;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class ReviewServiceTests
    {
        [SetUp]
        public void SetUp()
        { }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ReviewActivityAsync_NoExistingReview_Successful(
            ActivityReview activityReview,
            ActivityReviewXp activityReviewXp,
            UserReview review,
            Activity activity,
            int reviewerId,
            [Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            [Frozen] Mock<IMapper> mapperMock,
            ReviewService sut)
        {
            // Arrange
            reviewerId += 1;

            userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(reviewerId);

            uowMock.Setup(x => x.ActivityReviewXps.GetXpRewardAsync(activityReview.ActivityTypeId, activityReview.ReviewTypeId))
                .ReturnsAsync(activityReviewXp);

            uowMock.Setup(x => x.UserReviews.GetUserReview(activityReview.ActivityId, reviewerId))
                .ReturnsAsync((UserReview)null);

            uowMock.Setup(x => x.Activities.GetAsync(activityReview.ActivityId))
                .ReturnsAsync(activity);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            mapperMock
               .Setup(x => x.Map<UserReview>(activityReview))
               .Returns(review);

            // Act
            Func<Task> methodInTest = async () => await sut.ReviewActivityAsync(activityReview);

            // Assert
            methodInTest.Should().NotThrow<RestException>();

            userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            uowMock.Verify(x => x.ActivityReviewXps.GetXpRewardAsync(It.IsAny<ActivityTypeId>(), It.IsAny<ReviewTypeId>()), Times.Once);
            uowMock.Verify(x => x.Activities.GetAsync(activityReview.ActivityId), Times.Once);
            uowMock.Verify(x => x.UserReviews.GetUserReview(activityReview.ActivityId, reviewerId), Times.Once);
            uowMock.Verify(x => x.UserReviews.Add(review), Times.Once);
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ReviewActivityAsync_ExistingReview_Successful(
            ActivityReview activityReview,
            ActivityReviewXp activityReviewXpToYield,
            ActivityReviewXp activityReviewXpYielded,
            UserReview review,
            UserReview userReview,
            Activity activity,
            int xpToYield,
            int xpYielded,
            int reviewerId,
            [Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            ReviewService sut)
        {
            // Arrange
            reviewerId += 1;
            xpToYield = xpYielded + 1;

            userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(reviewerId);

            uowMock.Setup(x => x.ActivityReviewXps.GetXpRewardAsync(activityReview.ActivityTypeId, activityReview.ReviewTypeId))
                .ReturnsAsync(activityReviewXpToYield);
            activityReviewXpToYield.Xp = xpToYield;

            uowMock.Setup(x => x.ActivityReviewXps.GetXpRewardAsync(activityReview.ActivityTypeId, activityReview.ReviewTypeId))
                .ReturnsAsync(activityReviewXpYielded);
            activityReviewXpYielded.Xp = xpYielded;

            uowMock.Setup(x => x.Activities.GetAsync(activityReview.ActivityId))
                    .ReturnsAsync(activity);

            uowMock.Setup(x => x.UserReviews.GetUserReview(activityReview.ActivityId, reviewerId))
                 .ReturnsAsync(userReview);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.ReviewActivityAsync(activityReview);

            // Assert
            methodInTest.Should().NotThrow<RestException>();

            userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            uowMock.Verify(x => x.ActivityReviewXps.GetXpRewardAsync(It.IsAny<ActivityTypeId>(), It.IsAny<ReviewTypeId>()), Times.Exactly(2));
            uowMock.Verify(x => x.Activities.GetAsync(activityReview.ActivityId), Times.Once);
            uowMock.Verify(x => x.UserReviews.GetUserReview(activityReview.ActivityId, reviewerId), Times.Once);
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);

            uowMock.Verify(x => x.UserReviews.Add(review), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ReviewActivityAsync_ExistingReviewRewardTheSame_Successful(
            ActivityReview activityReview,
            ActivityReviewXp ActivityReviewXp,
            UserReview review,
            UserReview userReview,
            Activity activity,
            int xp,
            int reviewerId,
            [Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            ReviewService sut)
        {
            // Arrange
            reviewerId += 1;

            userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(reviewerId);

            uowMock.Setup(x => x.ActivityReviewXps.GetXpRewardAsync(activityReview.ActivityTypeId, activityReview.ReviewTypeId))
                .ReturnsAsync(ActivityReviewXp);

            uowMock.Setup(x => x.ActivityReviewXps.GetXpRewardAsync(userReview.Activity.ActivityTypeId, userReview.ReviewTypeId))
                .ReturnsAsync(ActivityReviewXp);

            uowMock.Setup(x => x.Activities.GetAsync(activityReview.ActivityId))
                .ReturnsAsync(activity);

            uowMock.Setup(x => x.UserReviews.GetUserReview(activityReview.ActivityId, reviewerId))
                .ReturnsAsync(userReview);

            // Act
            Func<Task> methodInTest = async () => await sut.ReviewActivityAsync(activityReview);

            // Assert
            methodInTest.Should().NotThrow<RestException>();

            userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            uowMock.Verify(x => x.ActivityReviewXps.GetXpRewardAsync(It.IsAny<ActivityTypeId>(), It.IsAny<ReviewTypeId>()), Times.Exactly(2));
            uowMock.Verify(x => x.Activities.GetAsync(activityReview.ActivityId), Times.Once);
            uowMock.Verify(x => x.UserReviews.GetUserReview(activityReview.ActivityId, reviewerId), Times.Once);
            uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ReviewActivityAsync_ReviewerIsActivityCreator_Fail(
            ActivityReview activityReview,
            Activity activity,
            UserReview review,
            int reviewerId,
            [Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            ReviewService sut)
        {
            // Arrange
            activity.User.Id = reviewerId;

            userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(reviewerId);

            uowMock.Setup(x => x.Activities.GetAsync(activityReview.ActivityId))
                .ReturnsAsync(activity);

            // Act
            Func<Task> methodInTest = async () => await sut.ReviewActivityAsync(activityReview);

            // Assert
            methodInTest.Should().Throw<RestException>();

            userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            uowMock.Verify(x => x.Activities.GetAsync(activityReview.ActivityId), Times.Once);
            uowMock.Verify(x => x.ActivityReviewXps.GetXpRewardAsync(It.IsAny<ActivityTypeId>(), It.IsAny<ReviewTypeId>()), Times.Never);
            uowMock.Verify(x => x.UserReviews.GetUserReview(activityReview.ActivityId, reviewerId), Times.Never);
            uowMock.Verify(x => x.UserReviews.Add(review), Times.Never);
        }
    }
}
