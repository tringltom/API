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
    public class ReviewServiceTests
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IUserAccessor> _userAccessorMock;
        private Mock<IUnitOfWork> _uowMock;
        private ReviewService _sut;

        [SetUp]
        public void SetUp()
        {
            _userAccessorMock = new Mock<IUserAccessor>();
            _mapperMock = new Mock<IMapper>();
            _uowMock = new Mock<IUnitOfWork>();
            _sut = new ReviewService(_uowMock.Object, _userAccessorMock.Object, _mapperMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetOwnerReviews_SuccessfullAsync(int userId, IEnumerable<UserReview> userReviews, List<UserReviewedActivity> userReviewedActivities)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserReviews.GetUserReviewsAsync(userId))
                .ReturnsAsync(userReviews);

            _mapperMock
                .Setup(x => x.Map<List<UserReviewedActivity>>(userReviews))
                .Returns(userReviewedActivities);

            // Act
            var res = await _sut.GetOwnerReviewsAsync();

            // Assert
            res.Should().BeEquivalentTo(userReviewedActivities);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserReviews.GetUserReviewsAsync(userId), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ReviewActivity_SuccessfulAsync(
            ActivityReview activityReview,
            ActivityReviewXp activityReviewXp,
            UserReview review,
            Activity activity,
            Skill creatorSkill,
            int reviewerId)
        {
            // Arrange
            reviewerId += 1;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(reviewerId);

            _uowMock.Setup(x => x.Activities.GetAsync(activityReview.ActivityId))
                .ReturnsAsync(activity);

            _uowMock.Setup(x => x.ActivityReviewXps.GetXpRewardAsync(activityReview.ActivityTypeId, activityReview.ReviewTypeId))
                .ReturnsAsync(activityReviewXp);

            _uowMock.Setup(x => x.Skills.GetSkillAsync(activity.User.Id, activityReview.ActivityTypeId))
                .ReturnsAsync(creatorSkill);

            _uowMock.Setup(x => x.UserReviews.GetUserReviewAsync(activityReview.ActivityId, reviewerId))
                .ReturnsAsync((UserReview)null);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock
               .Setup(x => x.Map<UserReview>(activityReview))
               .Returns(review);

            // Act
            var res = await _sut.ReviewActivityAsync(activityReview);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.Activities.GetAsync(activityReview.ActivityId), Times.Once);
            _uowMock.Verify(x => x.ActivityReviewXps.GetXpRewardAsync(It.IsAny<ActivityTypeId>(), It.IsAny<ReviewTypeId>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetSkillAsync(activity.User.Id, activityReview.ActivityTypeId), Times.Once);
            _uowMock.Verify(x => x.Activities.GetAsync(activityReview.ActivityId), Times.Once);
            _uowMock.Verify(x => x.UserReviews.GetUserReviewAsync(activityReview.ActivityId, reviewerId), Times.Once);
            _uowMock.Verify(x => x.UserReviews.Add(review), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ReviewActivity_ReviewerIsActivityCreatorAsync(
            ActivityReview activityReview,
            Activity activity,
            UserReview review,
            int reviewerId)
        {
            // Arrange
            activity.User.Id = reviewerId;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(reviewerId);

            _uowMock.Setup(x => x.Activities.GetAsync(activityReview.ActivityId))
                .ReturnsAsync(activity);

            // Act
            var res = await _sut.ReviewActivityAsync(activityReview);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.Activities.GetAsync(activityReview.ActivityId), Times.Once);
            _uowMock.Verify(x => x.ActivityReviewXps.GetXpRewardAsync(It.IsAny<ActivityTypeId>(), It.IsAny<ReviewTypeId>()), Times.Never);
            _uowMock.Verify(x => x.UserReviews.GetUserReviewAsync(activityReview.ActivityId, reviewerId), Times.Never);
            _uowMock.Verify(x => x.UserReviews.Add(review), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ReviewActivity_ExistingReviewAsync(
            ActivityReview activityReview,
            ActivityReviewXp activityReviewXpToYield,
            ActivityReviewXp activityReviewXpYielded,
            UserReview review,
            UserReview userReview,
            Activity activity,
            Skill creatorSkill,
            int xpToYield,
            int xpYielded,
            int reviewerId)
        {
            // Arrange
            reviewerId += 1;
            xpToYield = xpYielded + 1;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(reviewerId);

            _uowMock.Setup(x => x.ActivityReviewXps.GetXpRewardAsync(activityReview.ActivityTypeId, activityReview.ReviewTypeId))
                .ReturnsAsync(activityReviewXpToYield);
            activityReviewXpToYield.Xp = xpToYield;

            _uowMock.Setup(x => x.Skills.GetSkillAsync(activity.User.Id, activityReview.ActivityTypeId))
                .ReturnsAsync(creatorSkill);

            _uowMock.Setup(x => x.Activities.GetAsync(activityReview.ActivityId))
                    .ReturnsAsync(activity);

            _uowMock.Setup(x => x.UserReviews.GetUserReviewAsync(activityReview.ActivityId, reviewerId))
                 .ReturnsAsync(userReview);

            _uowMock.Setup(x => x.ActivityReviewXps.GetXpRewardAsync(userReview.Activity.ActivityTypeId, userReview.ReviewTypeId))
                .ReturnsAsync(activityReviewXpYielded);
            activityReviewXpYielded.Xp = xpYielded;

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.ReviewActivityAsync(activityReview);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.ActivityReviewXps.GetXpRewardAsync(It.IsAny<ActivityTypeId>(), It.IsAny<ReviewTypeId>()), Times.Exactly(2));
            _uowMock.Verify(x => x.Activities.GetAsync(activityReview.ActivityId), Times.Once);
            _uowMock.Verify(x => x.Skills.GetSkillAsync(activity.User.Id, activityReview.ActivityTypeId), Times.Once);
            _uowMock.Verify(x => x.UserReviews.GetUserReviewAsync(activityReview.ActivityId, reviewerId), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            _uowMock.Verify(x => x.UserReviews.Add(review), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ReviewActivity_ExistingReviewRewardTheSameAsync(
            ActivityReview activityReview,
            ActivityReviewXp activityReviewXp,
            UserReview review,
            UserReview userReview,
            Activity activity,
            Skill creatorSkill,
            int xp,
            int reviewerId)
        {
            // Arrange
            reviewerId += 1;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(reviewerId);

            _uowMock.Setup(x => x.ActivityReviewXps.GetXpRewardAsync(activityReview.ActivityTypeId, activityReview.ReviewTypeId))
                .ReturnsAsync(activityReviewXp);

            _uowMock.Setup(x => x.Skills.GetSkillAsync(activity.User.Id, activityReview.ActivityTypeId))
                .ReturnsAsync(creatorSkill);

            _uowMock.Setup(x => x.ActivityReviewXps.GetXpRewardAsync(userReview.Activity.ActivityTypeId, userReview.ReviewTypeId))
                .ReturnsAsync(activityReviewXp);

            _uowMock.Setup(x => x.Activities.GetAsync(activityReview.ActivityId))
                .ReturnsAsync(activity);

            _uowMock.Setup(x => x.UserReviews.GetUserReviewAsync(activityReview.ActivityId, reviewerId))
                .ReturnsAsync(userReview);

            // Act
            var res = await _sut.ReviewActivityAsync(activityReview);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.ActivityReviewXps.GetXpRewardAsync(It.IsAny<ActivityTypeId>(), It.IsAny<ReviewTypeId>()), Times.Exactly(2));
            _uowMock.Verify(x => x.Activities.GetAsync(activityReview.ActivityId), Times.Once);
            _uowMock.Verify(x => x.Skills.GetSkillAsync(activity.User.Id, activityReview.ActivityTypeId), Times.Once);
            _uowMock.Verify(x => x.UserReviews.GetUserReviewAsync(activityReview.ActivityId, reviewerId), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }
    }
}
