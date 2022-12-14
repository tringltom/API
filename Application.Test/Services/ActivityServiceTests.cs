using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.InfrastructureModels;
using Application.Models.Activity;
using Application.Services;
using AutoMapper;
using DAL;
using DAL.Query;
using Domain;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class ActivityServiceTests
    {
        private Mock<IUserAccessor> _userAccessorMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IEmailManager> _emailManagerMock;
        private Mock<IUnitOfWork> _uowMock;
        private Mock<IPhotoAccessor> _photoAccessorMock;
        private ActivityService _sut;

        [SetUp]
        public void SetUp()
        {
            _userAccessorMock = new Mock<IUserAccessor>();
            _mapperMock = new Mock<IMapper>();
            _emailManagerMock = new Mock<IEmailManager>();
            _uowMock = new Mock<IUnitOfWork>();
            _photoAccessorMock = new Mock<IPhotoAccessor>();
            _sut = new ActivityService(_userAccessorMock.Object, _mapperMock.Object, _emailManagerMock.Object, _uowMock.Object, _photoAccessorMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetActivity_SuccessfullAsync(Activity activity, ApprovedActivityReturn approvedActivityReturn)
        {
            // Arrange
            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);

            _mapperMock.Setup(x => x.Map<ApprovedActivityReturn>(activity))
                .Returns(approvedActivityReturn);

            // Act
            var res = await _sut.GetActivityAsync(It.IsAny<int>());

            // Assert
            res.Should().BeEquivalentTo(approvedActivityReturn);
            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetActivitiesFromOtherUsers_SuccessfullAsync(int userId, IEnumerable<Activity> activities,
            IEnumerable<OtherUserActivityReturn> activitiesGet, ActivitiesFromOtherUserEnvelope activityEnvelope)
        {
            // Arrange

            activityEnvelope.Activities = activitiesGet.ToList();
            activityEnvelope.ActivityCount = activitiesGet.ToList().Count;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.Activities.GetOrderedActivitiesFromOtherUsersAsync(It.IsAny<ActivityQuery>(), userId))
                .ReturnsAsync(activities);

            _mapperMock.Setup(x => x.Map<IEnumerable<Activity>, IEnumerable<OtherUserActivityReturn>>(activities))
                .Returns(activitiesGet);

            _uowMock.Setup(x => x.Activities.CountOtherUsersActivitiesAsync(userId, It.IsAny<ActivityQuery>()))
                .ReturnsAsync(activitiesGet.Count);

            // Act
            var res = await _sut.GetActivitiesFromOtherUsersAsync(It.IsAny<ActivityQuery>());

            // Assert
            res.Should().BeEquivalentTo(activityEnvelope);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.Activities.GetOrderedActivitiesFromOtherUsersAsync(It.IsAny<ActivityQuery>(), userId), Times.Once);
            _uowMock.Verify(x => x.Activities.CountOtherUsersActivitiesAsync(userId, It.IsAny<ActivityQuery>()), Times.Once);
        }


        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetApprovedActivitiesForUser_SuccessfullAsync(int userId, ActivityQuery activityQuery, IEnumerable<Activity> activities,
            IEnumerable<ApprovedActivityReturn> activitiesForEnvelope, ApprovedActivityEnvelope activityEnvelope)
        {
            // Arrange
            _uowMock.Setup(x => x.Activities.GetActivitiesCreatedByUser(userId, activityQuery))
                .ReturnsAsync(activities);

            _uowMock.Setup(x => x.Activities.CountActivitiesCreatedByUser(userId))
              .ReturnsAsync(activityEnvelope.ActivityCount);

            _mapperMock
                .Setup(x => x.Map<IEnumerable<Activity>, IEnumerable<ApprovedActivityReturn>>(activities))
                .Returns(activitiesForEnvelope);

            activityEnvelope.Activities = activitiesForEnvelope.ToList();

            // Act
            var res = await _sut.GetApprovedActivitiesCreatedByUserAsync(userId, activityQuery);

            // Assert
            res.Should().BeEquivalentTo(activityEnvelope);
            _uowMock.Verify(x => x.Activities.GetActivitiesCreatedByUser(userId, activityQuery), Times.Once);
            _uowMock.Verify(x => x.Activities.CountActivitiesCreatedByUser(userId), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AnswerToPuzzle_FirstTimeSuccessfullAsync(Activity activity, PuzzleAnswer puzzleAnswer, int userId, User user)
        {
            // Arrange
            userId = 1;
            activity.ActivityTypeId = ActivityTypeId.Puzzle;
            activity.Answer = puzzleAnswer.Answer;
            activity.User.Id = 2;
            activity.XpReward = null;
            activity.DateApproved = System.DateTimeOffset.Now;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()))
                .ReturnsAsync((UserPuzzleAnswer)null);
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(userId))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);
            _uowMock.Setup(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.AnswerToPuzzleAsync(It.IsAny<int>(), puzzleAnswer);

            // Assert
            res.Match(
                reward => reward.Should().Be(activity.XpReward),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetPuzzleSkillAsync(userId), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AnswerToPuzzle_AnsweredSuccessfullAsync(Activity activity, PuzzleAnswer puzzleAnswer, int userId, User user)
        {
            // Arrange
            userId = 1;
            activity.ActivityTypeId = ActivityTypeId.Puzzle;
            activity.Answer = puzzleAnswer.Answer;
            activity.User.Id = 2;
            activity.XpReward = 100;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()))
                .ReturnsAsync((UserPuzzleAnswer)null);
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(userId))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);
            _uowMock.Setup(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName))
                .Verifiable();

            // Act
            var res = await _sut.AnswerToPuzzleAsync(It.IsAny<int>(), puzzleAnswer);


            // Assert
            res.Match(
                reward => reward.Should().Be(activity.XpReward),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetPuzzleSkillAsync(userId), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            _emailManagerMock.Verify(x => x.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AnswerToPuzzle_ActivityNotFoundAsync(Activity activity, PuzzleAnswer puzzleAnswer, int userId, User user)
        {
            // Arrange
            userId = 1;
            activity.ActivityTypeId = ActivityTypeId.Puzzle;
            activity.Answer = puzzleAnswer.Answer;
            activity.User.Id = 2;
            activity.XpReward = 100;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync((Activity)null);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()))
                .ReturnsAsync((UserPuzzleAnswer)null);
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(userId))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);
            _uowMock.Setup(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName))
                .Verifiable();

            // Act
            var res = await _sut.AnswerToPuzzleAsync(It.IsAny<int>(), puzzleAnswer);

            // Assert
            res.Match(
                reward => reward.Should().Be(0),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()), Times.Never);
            _uowMock.Verify(x => x.Skills.GetPuzzleSkillAsync(userId), Times.Never);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Never);
            _uowMock.Verify(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AnswerToPuzzle_ActivityNotPuzzleAsync(Activity activity, PuzzleAnswer puzzleAnswer, int userId, User user)
        {
            // Arrange
            userId = 1;
            activity.ActivityTypeId = ActivityTypeId.GoodDeed;
            activity.Answer = puzzleAnswer.Answer;
            activity.User.Id = 2;
            activity.XpReward = 100;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()))
                .ReturnsAsync((UserPuzzleAnswer)null);
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(userId))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);
            _uowMock.Setup(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName))
                .Verifiable();

            // Act
            var res = await _sut.AnswerToPuzzleAsync(It.IsAny<int>(), puzzleAnswer);

            // Assert
            res.Match(
                reward => reward.Should().Be(0),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()), Times.Never);
            _uowMock.Verify(x => x.Skills.GetPuzzleSkillAsync(userId), Times.Never);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Never);
            _uowMock.Verify(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AnswerToPuzzle_IncorrectAnswerAsync(Activity activity, PuzzleAnswer puzzleAnswer, int userId, User user)
        {
            // Arrange
            userId = 1;
            puzzleAnswer.Answer = "correct";
            activity.ActivityTypeId = ActivityTypeId.Puzzle;
            activity.Answer = "wrong";
            activity.User.Id = 2;
            activity.XpReward = 100;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()))
                .ReturnsAsync((UserPuzzleAnswer)null);
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(userId))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);
            _uowMock.Setup(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName))
                .Verifiable();

            // Act
            var res = await _sut.AnswerToPuzzleAsync(It.IsAny<int>(), puzzleAnswer);

            // Assert
            res.Match(
                reward => reward.Should().Be(0),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()), Times.Never);
            _uowMock.Verify(x => x.Skills.GetPuzzleSkillAsync(userId), Times.Never);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Never);
            _uowMock.Verify(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AnswerToPuzzle_CreatorAnsweringAsync(Activity activity, PuzzleAnswer puzzleAnswer, int userId, User user)
        {
            // Arrange
            userId = 2;
            activity.ActivityTypeId = ActivityTypeId.Puzzle;
            activity.Answer = puzzleAnswer.Answer;
            activity.User.Id = 2;
            activity.XpReward = 100;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()))
                .ReturnsAsync((UserPuzzleAnswer)null);
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(userId))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);
            _uowMock.Setup(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName))
                .Verifiable();

            // Act
            var res = await _sut.AnswerToPuzzleAsync(It.IsAny<int>(), puzzleAnswer);

            // Assert
            res.Match(
                reward => reward.Should().Be(0),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()), Times.Never);
            _uowMock.Verify(x => x.Skills.GetPuzzleSkillAsync(userId), Times.Never);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Never);
            _uowMock.Verify(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AnswerToPuzzle_AlreadyAnsweredAsync(Activity activity, PuzzleAnswer puzzleAnswer, int userId, UserPuzzleAnswer userAnswer, User user)
        {
            // Arrange
            userId = 1;
            activity.ActivityTypeId = ActivityTypeId.Puzzle;
            activity.Answer = puzzleAnswer.Answer;
            activity.User.Id = 2;
            activity.XpReward = 100;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()))
                .ReturnsAsync(userAnswer);
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(userId))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);
            _uowMock.Setup(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName))
                .Verifiable();

            // Act
            var res = await _sut.AnswerToPuzzleAsync(It.IsAny<int>(), puzzleAnswer);

            // Assert
            res.Match(
                reward => reward.Should().Be(0),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetPuzzleSkillAsync(userId), Times.Never);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Never);
            _uowMock.Verify(x => x.UserPuzzleAnswers.Add(It.IsAny<UserPuzzleAnswer>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApprovePendingActivity_SuccessfullAsync(PendingActivity pendingActivity, Activity activity,
            ApprovedActivityReturn approvedActivityReturn)
        {
            // Arrange
            _uowMock.Setup(x => x.PendingActivities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(pendingActivity);

            _mapperMock
                .Setup(x => x.Map<Activity>(pendingActivity))
                .Returns(activity);

            _uowMock.Setup(x => x.Activities.Add(activity))
               .Verifiable();

            _uowMock.Setup(x => x.PendingActivities.Remove(pendingActivity))
              .Verifiable();

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, true))
                .Verifiable();

            _mapperMock
               .Setup(x => x.Map<ApprovedActivityReturn>(activity))
               .Returns(approvedActivityReturn);

            // Act
            var res = await _sut.ApprovePendingActivity(It.IsAny<int>());

            // Assert
            res.Match(
                activityreturn => activityreturn.Should().BeEquivalentTo(approvedActivityReturn),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.PendingActivities.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Activities.Add(activity), Times.Once);
            _uowMock.Verify(x => x.PendingActivities.Remove(pendingActivity), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, true), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApprovePendingActivity_ActivityNotFoundAsync(PendingActivity pendingActivity, Activity activity, BadRequest badRequest)
        {
            // Arrange
            _uowMock.Setup(x => x.PendingActivities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync((PendingActivity)null);

            // Act
            var res = await _sut.ApprovePendingActivity(It.IsAny<int>());

            // Assert
            res.Match(
                activityreturn => activityreturn.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.PendingActivities.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Activities.Add(activity), Times.Never);
            _uowMock.Verify(x => x.PendingActivities.Remove(pendingActivity), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, true), Times.Never);
        }


    }
}
