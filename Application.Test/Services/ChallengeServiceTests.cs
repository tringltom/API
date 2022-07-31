using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using Application.Services;
using AutoFixture;
using AutoMapper;
using DAL;
using DAL.Query;
using Domain;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class ChallengeServiceTests
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IUnitOfWork> _uowMock;
        private Mock<IUserAccessor> _userAccessorMock;
        private Mock<IPhotoAccessor> _photoAccessorMock;
        private Mock<IEmailManager> _emailManagerMock;
        private ChallengeService _sut;

        [SetUp]
        public void SetUp()
        {
            _userAccessorMock = new Mock<IUserAccessor>();
            _mapperMock = new Mock<IMapper>();
            _uowMock = new Mock<IUnitOfWork>();
            _photoAccessorMock = new Mock<IPhotoAccessor>();
            _emailManagerMock = new Mock<IEmailManager>();
            _sut = new ChallengeService(_mapperMock.Object, _uowMock.Object, _userAccessorMock.Object, _photoAccessorMock.Object, _emailManagerMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetOwnerChallengeAnswers_SuccessfullAsync(int userId,
           Activity challenge,
           IEnumerable<UserChallengeAnswer> userChallengeAnswers,
           IEnumerable<ChallengeAnswerReturn> challengeAnswerReturns,
           ChallengeAnswerEnvelope challengeAnswerEnvelope)
        {
            // Arrange
            challenge.ActivityTypeId = ActivityTypeId.Challenge;
            challenge.XpReward = null;
            challenge.User.Id = userId;

            challengeAnswerEnvelope.ChallengeAnswers = challengeAnswerReturns.ToList();
            challengeAnswerEnvelope.ChallengeAnswersCount = challengeAnswerReturns.Count();

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(challenge);

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserChallengeAnswers.GetUserChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>()))
                .ReturnsAsync(userChallengeAnswers);

            _mapperMock.Setup(x => x.Map<IEnumerable<UserChallengeAnswer>, IEnumerable<ChallengeAnswerReturn>>(userChallengeAnswers))
                .Returns(challengeAnswerReturns);

            _uowMock.Setup(x => x.UserChallengeAnswers.CountChallengeAnswersAsync(It.IsAny<int>()))
                .ReturnsAsync(challengeAnswerReturns.Count);

            // Act
            var res = await _sut.GetOwnerChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>());

            // Assert
            res.Match(
                reward => reward.Should().BeEquivalentTo(challengeAnswerEnvelope),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetUserChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>()), Times.Once);
            _uowMock.Verify(x => x.UserChallengeAnswers.CountChallengeAnswersAsync(It.IsAny<int>()), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetOwnerChallengeAnswers_NotFoundAsync(int userId,
            Activity challenge,
            IEnumerable<UserChallengeAnswer> userChallengeAnswers,
            IEnumerable<ChallengeAnswerReturn> challengeAnswerReturns
            )
        {
            // Arrange
            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync((Activity)null);

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserChallengeAnswers.GetUserChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>()))
                .ReturnsAsync(userChallengeAnswers);

            _mapperMock.Setup(x => x.Map<IEnumerable<UserChallengeAnswer>, IEnumerable<ChallengeAnswerReturn>>(userChallengeAnswers))
                .Returns(challengeAnswerReturns);

            _uowMock.Setup(x => x.UserChallengeAnswers.CountChallengeAnswersAsync(It.IsAny<int>()))
                .ReturnsAsync(challengeAnswerReturns.Count);

            // Act
            var res = await _sut.GetOwnerChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>());

            // Assert
            res.Match(
                reward => reward.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetUserChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>()), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.CountChallengeAnswersAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetOwnerChallengeAnswers_ActivityNotChallengeAsync(int userId,
            Activity challenge,
            IEnumerable<UserChallengeAnswer> userChallengeAnswers,
            IEnumerable<ChallengeAnswerReturn> challengeAnswerReturns
            )
        {
            // Arrange
            challenge.ActivityTypeId = ActivityTypeId.Puzzle;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(challenge);

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserChallengeAnswers.GetUserChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>()))
                .ReturnsAsync(userChallengeAnswers);

            _mapperMock.Setup(x => x.Map<IEnumerable<UserChallengeAnswer>, IEnumerable<ChallengeAnswerReturn>>(userChallengeAnswers))
                .Returns(challengeAnswerReturns);

            _uowMock.Setup(x => x.UserChallengeAnswers.CountChallengeAnswersAsync(It.IsAny<int>()))
                .ReturnsAsync(challengeAnswerReturns.Count);

            // Act
            var res = await _sut.GetOwnerChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>());

            // Assert
            res.Match(
                reward => reward.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetUserChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>()), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.CountChallengeAnswersAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetOwnerChallengeAnswers_ChallengeSolvedAsync(int userId,
            Activity challenge,
            IEnumerable<UserChallengeAnswer> userChallengeAnswers,
            IEnumerable<ChallengeAnswerReturn> challengeAnswerReturns
            )
        {
            // Arrange
            challenge.ActivityTypeId = ActivityTypeId.Challenge;
            challenge.XpReward = 10;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(challenge);

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserChallengeAnswers.GetUserChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>()))
                .ReturnsAsync(userChallengeAnswers);

            _mapperMock.Setup(x => x.Map<IEnumerable<UserChallengeAnswer>, IEnumerable<ChallengeAnswerReturn>>(userChallengeAnswers))
                .Returns(challengeAnswerReturns);

            _uowMock.Setup(x => x.UserChallengeAnswers.CountChallengeAnswersAsync(It.IsAny<int>()))
                .ReturnsAsync(challengeAnswerReturns.Count);

            // Act
            var res = await _sut.GetOwnerChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>());

            // Assert
            res.Match(
                reward => reward.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetUserChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>()), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.CountChallengeAnswersAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetOwnerChallengeAnswers_NotOwnedChallengeAsync(
            Activity challenge,
            IEnumerable<UserChallengeAnswer> userChallengeAnswers,
            IEnumerable<ChallengeAnswerReturn> challengeAnswerReturns
            )
        {
            // Arrange
            var userId = 1;
            challenge.ActivityTypeId = ActivityTypeId.Challenge;
            challenge.XpReward = null;
            challenge.User.Id = 2;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(challenge);

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserChallengeAnswers.GetUserChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>()))
                .ReturnsAsync(userChallengeAnswers);

            _mapperMock.Setup(x => x.Map<IEnumerable<UserChallengeAnswer>, IEnumerable<ChallengeAnswerReturn>>(userChallengeAnswers))
                .Returns(challengeAnswerReturns);

            _uowMock.Setup(x => x.UserChallengeAnswers.CountChallengeAnswersAsync(It.IsAny<int>()))
                .ReturnsAsync(challengeAnswerReturns.Count);

            // Act
            var res = await _sut.GetOwnerChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>());

            // Assert
            res.Match(
                reward => reward.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetUserChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>()), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.CountChallengeAnswersAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetChallengesForApproval_SuccessfullAsync(
            IEnumerable<Activity> challengesForApproval,
            IEnumerable<ChallengeReturn> challengeReturn,
            ChallengeEnvelope challengeEnvelope)
        {
            // Arrange
            challengeEnvelope.Challenges = challengeReturn.ToList();
            challengeEnvelope.ChallengeCount = challengeReturn.Count();

            _uowMock.Setup(x => x.Activities.GetChallengesForApprovalAsync(It.IsAny<QueryObject>()))
                .ReturnsAsync(challengesForApproval);

            _mapperMock.Setup(x => x.Map<IEnumerable<Activity>, IEnumerable<ChallengeReturn>>(challengesForApproval))
                .Returns(challengeReturn);

            _uowMock.Setup(x => x.Activities.CountChallengesForApprovalAsync())
                .ReturnsAsync(challengeReturn.Count);

            // Act
            var res = await _sut.GetChallengesForApprovalAsync(It.IsAny<QueryObject>());

            // Assert
            res.Should().BeEquivalentTo(challengeEnvelope);
            _uowMock.Verify(x => x.Activities.GetChallengesForApprovalAsync(It.IsAny<QueryObject>()), Times.Once);
            _uowMock.Verify(x => x.Activities.CountChallengesForApprovalAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AnswerToChallenge_SuccessfullAddAsync(int activityId,
    Activity challenge,
    ChallengeAnswer challengeAnswer,
    User user)
        {
            // Arrange
            var userId = 1;
            challenge.User.Id = 2;
            challengeAnswer.Images = null;

            var userChallengeAnswer = new UserChallengeAnswer
            {
                UserId = userId,
                ActivityId = activityId,
                Description = challengeAnswer.Description,
                ChallengeMedias = new List<ChallengeMedia>()
            };

            _uowMock.Setup(x => x.Activities.GetAsync(activityId))
                .ReturnsAsync(challenge);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserChallengeAnswers.GetUserChallengeAnswerAsync(userId, activityId))
                .ReturnsAsync((UserChallengeAnswer)null);
            _uowMock.Setup(x => x.ChallengeMedias.GetChallengeMedias(userChallengeAnswer.Id))
                .ReturnsAsync((List<ChallengeMedia>)null);
            _uowMock.Setup(x => x.UserChallengeAnswers.Add(userChallengeAnswer))
                .Verifiable();
            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);
            _emailManagerMock.Setup(x => x.SendChallengeAnsweredEmailAsync(challenge.Title, challenge.User.Email, user.UserName))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.AnswerToChallengeAsync(activityId, challengeAnswer);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(activityId), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetUserChallengeAnswerAsync(userId, activityId), Times.Once);
            _uowMock.Verify(x => x.ChallengeMedias.GetChallengeMedias(userChallengeAnswer.Id), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.Add(It.IsAny<UserChallengeAnswer>()), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _emailManagerMock.Verify(x => x.SendChallengeAnsweredEmailAsync(challenge.Title, challenge.User.Email, user.UserName), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AnswerToChallenge_SuccessfullUpdateAsync(int activityId,
            Activity challenge,
            ChallengeAnswer challengeAnswer,
            UserChallengeAnswer userChallengeAnswer,
            User user)
        {
            // Arrange
            var userId = 1;
            challenge.User.Id = 2;
            challengeAnswer.Images = null;

            var newUserChallengeAnswer = new UserChallengeAnswer
            {
                UserId = userId,
                ActivityId = activityId,
                Description = challengeAnswer.Description,
                ChallengeMedias = new List<ChallengeMedia>()
            };

            _uowMock.Setup(x => x.Activities.GetAsync(activityId))
                .ReturnsAsync(challenge);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserChallengeAnswers.GetUserChallengeAnswerAsync(userId, activityId))
                .ReturnsAsync(userChallengeAnswer);
            _uowMock.Setup(x => x.ChallengeMedias.GetChallengeMedias(userChallengeAnswer.Id))
                .ReturnsAsync((List<ChallengeMedia>)null);
            _uowMock.Setup(x => x.UserChallengeAnswers.Add(newUserChallengeAnswer))
                .Verifiable();
            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);
            _emailManagerMock.Setup(x => x.SendChallengeAnsweredEmailAsync(challenge.Title, challenge.User.Email, user.UserName))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.AnswerToChallengeAsync(activityId, challengeAnswer);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(activityId), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetUserChallengeAnswerAsync(userId, activityId), Times.Once);
            _uowMock.Verify(x => x.ChallengeMedias.GetChallengeMedias(userChallengeAnswer.Id), Times.Once);
            _uowMock.Verify(x => x.UserChallengeAnswers.Add(It.IsAny<UserChallengeAnswer>()), Times.Never);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Never);
            _emailManagerMock.Verify(x => x.SendChallengeAnsweredEmailAsync(challenge.Title, challenge.User.Email, user.UserName), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AnswerToChallenge_ActivityNotFoundAsync(int activityId,
            Activity challenge,
            ChallengeAnswer challengeAnswer,
            UserChallengeAnswer userChallengeAnswer,
            User user)
        {
            // Arrange
            var userId = 1;
            challenge.User.Id = 2;
            challengeAnswer.Images = null;

            var newUserChallengeAnswer = new UserChallengeAnswer
            {
                UserId = userId,
                ActivityId = activityId,
                Description = challengeAnswer.Description,
                ChallengeMedias = new List<ChallengeMedia>()
            };

            _uowMock.Setup(x => x.Activities.GetAsync(activityId))
                .ReturnsAsync((Activity)null);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserChallengeAnswers.GetUserChallengeAnswerAsync(userId, activityId))
                .ReturnsAsync(userChallengeAnswer);
            _uowMock.Setup(x => x.ChallengeMedias.GetChallengeMedias(userChallengeAnswer.Id))
                .ReturnsAsync((List<ChallengeMedia>)null);
            _uowMock.Setup(x => x.UserChallengeAnswers.Add(newUserChallengeAnswer))
                .Verifiable();
            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);
            _emailManagerMock.Setup(x => x.SendChallengeAnsweredEmailAsync(challenge.Title, challenge.User.Email, user.UserName))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.AnswerToChallengeAsync(activityId, challengeAnswer);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(activityId), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetUserChallengeAnswerAsync(userId, activityId), Times.Never);
            _uowMock.Verify(x => x.ChallengeMedias.GetChallengeMedias(userChallengeAnswer.Id), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.Add(It.IsAny<UserChallengeAnswer>()), Times.Never);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Never);
            _emailManagerMock.Verify(x => x.SendChallengeAnsweredEmailAsync(challenge.Title, challenge.User.Email, user.UserName), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AnswerToChallenge_OwnerChallengeAsync(int activityId,
            Activity challenge,
            ChallengeAnswer challengeAnswer,
            UserChallengeAnswer userChallengeAnswer,
            User user)
        {
            // Arrange
            var userId = 1;
            challenge.User.Id = userId;
            challengeAnswer.Images = null;

            var newUserChallengeAnswer = new UserChallengeAnswer
            {
                UserId = userId,
                ActivityId = activityId,
                Description = challengeAnswer.Description,
                ChallengeMedias = new List<ChallengeMedia>()
            };

            _uowMock.Setup(x => x.Activities.GetAsync(activityId))
                .ReturnsAsync(challenge);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserChallengeAnswers.GetUserChallengeAnswerAsync(userId, activityId))
                .ReturnsAsync(userChallengeAnswer);
            _uowMock.Setup(x => x.ChallengeMedias.GetChallengeMedias(userChallengeAnswer.Id))
                .ReturnsAsync((List<ChallengeMedia>)null);
            _uowMock.Setup(x => x.UserChallengeAnswers.Add(newUserChallengeAnswer))
                .Verifiable();
            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);
            _emailManagerMock.Setup(x => x.SendChallengeAnsweredEmailAsync(challenge.Title, challenge.User.Email, user.UserName))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.AnswerToChallengeAsync(activityId, challengeAnswer);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(activityId), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetUserChallengeAnswerAsync(userId, activityId), Times.Never);
            _uowMock.Verify(x => x.ChallengeMedias.GetChallengeMedias(userChallengeAnswer.Id), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.Add(It.IsAny<UserChallengeAnswer>()), Times.Never);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Never);
            _emailManagerMock.Verify(x => x.SendChallengeAnsweredEmailAsync(challenge.Title, challenge.User.Email, user.UserName), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmChallengeAnswer_SuccessfullAsync(UserChallengeAnswer existingUserChallengeAnswer, UserChallengeAnswer confirmedAnswer)
        {
            // Arrange
            var userId = 1;
            existingUserChallengeAnswer.Activity.User.Id = userId;
            existingUserChallengeAnswer.Confirmed = false;

            _uowMock.Setup(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(existingUserChallengeAnswer);

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserChallengeAnswers.GetConfirmedUserChallengeAnswersAsync(existingUserChallengeAnswer.ActivityId))
                .ReturnsAsync(confirmedAnswer);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.ConfirmChallengeAnswerAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetConfirmedUserChallengeAnswersAsync(existingUserChallengeAnswer.ActivityId), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmChallengeAnswer_NotFoundAsync(UserChallengeAnswer existingUserChallengeAnswer, UserChallengeAnswer confirmedAnswer)
        {
            // Arrange
            var userId = 1;

            _uowMock.Setup(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()))
                .ReturnsAsync((UserChallengeAnswer)null);

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserChallengeAnswers.GetConfirmedUserChallengeAnswersAsync(existingUserChallengeAnswer.ActivityId))
                .ReturnsAsync(confirmedAnswer);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.ConfirmChallengeAnswerAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetConfirmedUserChallengeAnswersAsync(existingUserChallengeAnswer.ActivityId), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmChallengeAnswerAlreadyChosenAsync(UserChallengeAnswer existingUserChallengeAnswer, UserChallengeAnswer confirmedAnswer)
        {
            // Arrange
            var userId = 1;
            existingUserChallengeAnswer.Confirmed = true;

            _uowMock.Setup(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(existingUserChallengeAnswer);

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserChallengeAnswers.GetConfirmedUserChallengeAnswersAsync(existingUserChallengeAnswer.ActivityId))
                .ReturnsAsync(confirmedAnswer);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.ConfirmChallengeAnswerAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetConfirmedUserChallengeAnswersAsync(existingUserChallengeAnswer.ActivityId), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmChallengeAnswer_NotOwnedAsync(UserChallengeAnswer existingUserChallengeAnswer, UserChallengeAnswer confirmedAnswer)
        {
            // Arrange
            var userId = 1;
            existingUserChallengeAnswer.Activity.User.Id = 2;
            existingUserChallengeAnswer.Confirmed = false;

            _uowMock.Setup(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(existingUserChallengeAnswer);

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.UserChallengeAnswers.GetConfirmedUserChallengeAnswersAsync(existingUserChallengeAnswer.ActivityId))
                .ReturnsAsync(confirmedAnswer);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.ConfirmChallengeAnswerAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetConfirmedUserChallengeAnswersAsync(existingUserChallengeAnswer.ActivityId), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task DisapproveChallengeAnswer_SuccessfullAsync(UserChallengeAnswer userChallengeAnswer, UserChallengeAnswer confirmedAnswer)
        {
            // Arrange
            _uowMock.Setup(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(userChallengeAnswer);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.Activity.User.Email, true))
                .Verifiable();

            // Act
            var res = await _sut.DisapproveChallengeAnswerAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.Activity.User.Email, true), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task DisapproveChallengeAnswer_NotFoundAsync(UserChallengeAnswer userChallengeAnswer, UserChallengeAnswer confirmedAnswer)
        {
            // Arrange
            _uowMock.Setup(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()))
                .ReturnsAsync((UserChallengeAnswer)null);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.Activity.User.Email, true))
                .Verifiable();

            // Act
            var res = await _sut.DisapproveChallengeAnswerAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.Activity.User.Email, true), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApproveChallengeAnswer_SuccessfullAsync(UserChallengeAnswer userChallengeAnswer,
            IEnumerable<ActivityReviewXp> challengeXps,
            IEnumerable<UserChallengeAnswer> userChallangeAnswersForDeletion)
        {
            // Arrange
            userChallengeAnswer.Confirmed = true;
            userChallengeAnswer.Activity.XpReward = null;
            userChallengeAnswer.Activity.UserReviews = new List<UserReview>();

            _uowMock.Setup(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(userChallengeAnswer);
            _uowMock.Setup(x => x.Skills.GetChallengeSkillAsync(userChallengeAnswer.UserId))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.ActivityReviewXps.GetChallengeXpRewardAsync())
                .ReturnsAsync(challengeXps);
            _uowMock.Setup(x => x.UserChallengeAnswers.GetNotConfirmedUserChallengeAnswersAsync(userChallengeAnswer.ActivityId))
                .ReturnsAsync(userChallangeAnswersForDeletion);
            _photoAccessorMock.Setup(x => x.DeletePhotoAsync(It.IsAny<string>()))
                .Verifiable();
            _uowMock.Setup(x => x.UserChallengeAnswers.RemoveRange(userChallangeAnswersForDeletion))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.Activity.User.Email, true))
                .Verifiable();
            _emailManagerMock.Setup(x => x.SendChallengeAnswerAcceptedEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.User.Email))
                .Verifiable();

            // Act
            var res = await _sut.ApproveChallengeAnswerAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetChallengeSkillAsync(userChallengeAnswer.UserId), Times.Once);
            _uowMock.Verify(x => x.ActivityReviewXps.GetChallengeXpRewardAsync(), Times.Once);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetNotConfirmedUserChallengeAnswersAsync(userChallengeAnswer.ActivityId), Times.Once);
            _photoAccessorMock.Verify(x => x.DeletePhotoAsync(It.IsAny<string>()), Times.AtLeastOnce);
            _uowMock.Verify(x => x.UserChallengeAnswers.RemoveRange(userChallangeAnswersForDeletion), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.Activity.User.Email, true), Times.Once);
            _emailManagerMock.Verify(x => x.SendChallengeAnswerAcceptedEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.User.Email), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApproveChallengeAnswer_NotFoundAsync(UserChallengeAnswer userChallengeAnswer,
            IEnumerable<ActivityReviewXp> challengeXps,
            IEnumerable<UserChallengeAnswer> userChallangeAnswersForDeletion)
        {
            // Arrange
            _uowMock.Setup(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()))
                .ReturnsAsync((UserChallengeAnswer)null);
            _uowMock.Setup(x => x.Skills.GetChallengeSkillAsync(userChallengeAnswer.UserId))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.ActivityReviewXps.GetChallengeXpRewardAsync())
                .ReturnsAsync(challengeXps);
            _uowMock.Setup(x => x.UserChallengeAnswers.GetNotConfirmedUserChallengeAnswersAsync(userChallengeAnswer.ActivityId))
                .ReturnsAsync(userChallangeAnswersForDeletion);
            _photoAccessorMock.Setup(x => x.DeletePhotoAsync(It.IsAny<string>()))
                .Verifiable();
            _uowMock.Setup(x => x.UserChallengeAnswers.RemoveRange(userChallangeAnswersForDeletion))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.Activity.User.Email, true))
                .Verifiable();
            _emailManagerMock.Setup(x => x.SendChallengeAnswerAcceptedEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.User.Email))
                .Verifiable();

            // Act
            var res = await _sut.ApproveChallengeAnswerAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetChallengeSkillAsync(userChallengeAnswer.UserId), Times.Never);
            _uowMock.Verify(x => x.ActivityReviewXps.GetChallengeXpRewardAsync(), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetNotConfirmedUserChallengeAnswersAsync(userChallengeAnswer.ActivityId), Times.Never);
            _photoAccessorMock.Verify(x => x.DeletePhotoAsync(It.IsAny<string>()), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.RemoveRange(userChallangeAnswersForDeletion), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.Activity.User.Email, true), Times.Never);
            _emailManagerMock.Verify(x => x.SendChallengeAnswerAcceptedEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.User.Email), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApproveChallengeAnswer_NotChosenAsync(UserChallengeAnswer userChallengeAnswer,
            IEnumerable<ActivityReviewXp> challengeXps,
            IEnumerable<UserChallengeAnswer> userChallangeAnswersForDeletion)
        {
            // Arrange
            userChallengeAnswer.Confirmed = false;

            _uowMock.Setup(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(userChallengeAnswer);
            _uowMock.Setup(x => x.Skills.GetChallengeSkillAsync(userChallengeAnswer.UserId))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.ActivityReviewXps.GetChallengeXpRewardAsync())
                .ReturnsAsync(challengeXps);
            _uowMock.Setup(x => x.UserChallengeAnswers.GetNotConfirmedUserChallengeAnswersAsync(userChallengeAnswer.ActivityId))
                .ReturnsAsync(userChallangeAnswersForDeletion);
            _photoAccessorMock.Setup(x => x.DeletePhotoAsync(It.IsAny<string>()))
                .Verifiable();
            _uowMock.Setup(x => x.UserChallengeAnswers.RemoveRange(userChallangeAnswersForDeletion))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.Activity.User.Email, true))
                .Verifiable();
            _emailManagerMock.Setup(x => x.SendChallengeAnswerAcceptedEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.User.Email))
                .Verifiable();

            // Act
            var res = await _sut.ApproveChallengeAnswerAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetChallengeSkillAsync(userChallengeAnswer.UserId), Times.Never);
            _uowMock.Verify(x => x.ActivityReviewXps.GetChallengeXpRewardAsync(), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetNotConfirmedUserChallengeAnswersAsync(userChallengeAnswer.ActivityId), Times.Never);
            _photoAccessorMock.Verify(x => x.DeletePhotoAsync(It.IsAny<string>()), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.RemoveRange(userChallangeAnswersForDeletion), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.Activity.User.Email, true), Times.Never);
            _emailManagerMock.Verify(x => x.SendChallengeAnswerAcceptedEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.User.Email), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApproveChallengeAnswer_AlreadySolvedAsync(UserChallengeAnswer userChallengeAnswer,
            IEnumerable<ActivityReviewXp> challengeXps,
            IEnumerable<UserChallengeAnswer> userChallangeAnswersForDeletion)
        {
            // Arrange
            userChallengeAnswer.Confirmed = true;
            userChallengeAnswer.Activity.XpReward = 10;

            _uowMock.Setup(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(userChallengeAnswer);
            _uowMock.Setup(x => x.Skills.GetChallengeSkillAsync(userChallengeAnswer.UserId))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.ActivityReviewXps.GetChallengeXpRewardAsync())
                .ReturnsAsync(challengeXps);
            _uowMock.Setup(x => x.UserChallengeAnswers.GetNotConfirmedUserChallengeAnswersAsync(userChallengeAnswer.ActivityId))
                .ReturnsAsync(userChallangeAnswersForDeletion);
            _photoAccessorMock.Setup(x => x.DeletePhotoAsync(It.IsAny<string>()))
                .Verifiable();
            _uowMock.Setup(x => x.UserChallengeAnswers.RemoveRange(userChallangeAnswersForDeletion))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.Activity.User.Email, true))
                .Verifiable();
            _emailManagerMock.Setup(x => x.SendChallengeAnswerAcceptedEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.User.Email))
                .Verifiable();

            // Act
            var res = await _sut.ApproveChallengeAnswerAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.UserChallengeAnswers.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetChallengeSkillAsync(userChallengeAnswer.UserId), Times.Never);
            _uowMock.Verify(x => x.ActivityReviewXps.GetChallengeXpRewardAsync(), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.GetNotConfirmedUserChallengeAnswersAsync(userChallengeAnswer.ActivityId), Times.Never);
            _photoAccessorMock.Verify(x => x.DeletePhotoAsync(It.IsAny<string>()), Times.Never);
            _uowMock.Verify(x => x.UserChallengeAnswers.RemoveRange(userChallangeAnswersForDeletion), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.Activity.User.Email, true), Times.Never);
            _emailManagerMock.Verify(x => x.SendChallengeAnswerAcceptedEmailAsync(userChallengeAnswer.Activity.Title, userChallengeAnswer.User.Email), Times.Never);
        }
    }
}
