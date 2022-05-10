using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using Application.Services;
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
    public class PendingActivityServiceTests
    {
        private Mock<IPhotoAccessor> _photoAccessorMock;
        private Mock<IUserAccessor> _userAccessorMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IEmailManager> _emailManagerMock;
        private Mock<IUnitOfWork> _uowMock;
        private PendingActivityService _sut;

        [SetUp]
        public void SetUp()
        {
            _photoAccessorMock = new Mock<IPhotoAccessor>();
            _userAccessorMock = new Mock<IUserAccessor>();
            _mapperMock = new Mock<IMapper>();
            _emailManagerMock = new Mock<IEmailManager>();
            _uowMock = new Mock<IUnitOfWork>();
            _sut = new PendingActivityService(_photoAccessorMock.Object, _userAccessorMock.Object, _mapperMock.Object, _emailManagerMock.Object, _uowMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetPendingActivities_SuccessfullAsync(IEnumerable<PendingActivity> activities,
            IEnumerable<PendingActivityReturn> activitiesReturn,
            PendingActivityEnvelope pendingActivityEnvelope)
        {
            // Arrange
            pendingActivityEnvelope.Activities = activitiesReturn.ToList();
            pendingActivityEnvelope.ActivityCount = activitiesReturn.ToList().Count;

            _uowMock.Setup(x => x.PendingActivities.GetLatestPendingActivitiesAsync(It.IsAny<QueryObject>()))
               .ReturnsAsync(activities);

            _mapperMock
                .Setup(x => x.Map<IEnumerable<PendingActivity>, IEnumerable<PendingActivityReturn>>(activities))
                .Returns(activitiesReturn);

            _uowMock
                .Setup(x => x.PendingActivities.CountAsync())
                .ReturnsAsync(activitiesReturn.Count);

            // Act
            var res = await _sut.GetPendingActivitiesAsync(It.IsAny<QueryObject>());

            // Assert
            res.Should().BeEquivalentTo(pendingActivityEnvelope);
            _uowMock.Verify(x => x.PendingActivities.GetLatestPendingActivitiesAsync(It.IsAny<QueryObject>()), Times.Once);
            _uowMock.Verify(x => x.PendingActivities.CountAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetOwnerPendingActivities_SuccessfullAsync(int userId,
            IEnumerable<PendingActivity> activities,
            IEnumerable<PendingActivityForUserReturn> pendingActivityForUserReturn,
            PendingActivityForUserEnvelope pendingActivityForUserEnvelope)
        {
            // Arrange
            pendingActivityForUserEnvelope.Activities = pendingActivityForUserReturn.ToList();
            pendingActivityForUserEnvelope.ActivityCount = pendingActivityForUserReturn.ToList().Count;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.PendingActivities.GetLatestPendingActivitiesAsync(userId, It.IsAny<ActivityQuery>()))
               .ReturnsAsync(activities);

            _mapperMock
                .Setup(x => x.Map<IEnumerable<PendingActivity>, IEnumerable<PendingActivityForUserReturn>>(activities))
                .Returns(pendingActivityForUserReturn);

            _uowMock
                .Setup(x => x.PendingActivities.CountPendingActivitiesAsync(userId))
                .ReturnsAsync(pendingActivityForUserReturn.Count);

            // Act
            var res = await _sut.GetOwnerPendingActivitiesAsync(It.IsAny<ActivityQuery>());

            // Assert
            res.Should().BeEquivalentTo(pendingActivityForUserEnvelope);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.PendingActivities.GetLatestPendingActivitiesAsync(userId, It.IsAny<ActivityQuery>()), Times.Once);
            _uowMock.Verify(x => x.PendingActivities.CountPendingActivitiesAsync(userId), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetOwnerPendingActivity_SuccessfullAsync(int userId,
            int activityId,
            PendingActivity pendingActivity,
            ActivityCreate activityCreate)
        {
            // Arrange
            pendingActivity.User.Id = userId;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.PendingActivities.GetAsync(activityId))
               .ReturnsAsync(pendingActivity);

            _mapperMock
                .Setup(x => x.Map<ActivityCreate>(pendingActivity))
                .Returns(activityCreate);

            // Act
            var res = await _sut.GetOwnerPendingActivityAsync(activityId);

            // Assert
            res.Match(
                pendingActivityReturn => pendingActivityReturn.Should().BeEquivalentTo(activityCreate),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.PendingActivities.GetAsync(activityId), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetOwnerPendingActivity_InvalidCreatorAsync(int userId,
            int activityId,
            PendingActivity pendingActivity,
            ActivityCreate activityCreate)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.PendingActivities.GetAsync(activityId))
               .ReturnsAsync(pendingActivity);

            _mapperMock
                .Setup(x => x.Map<ActivityCreate>(pendingActivity))
                .Returns(activityCreate);

            // Act
            var res = await _sut.GetOwnerPendingActivityAsync(activityId);

            // Assert
            res.Match(
                pendingActivityReturn => pendingActivityReturn.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.PendingActivities.GetAsync(activityId), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task UpdatePendingActivity_SuccessfullAsync(int userId,
            int activityId,
            PendingActivity pendingActivity,
            ActivityCreate updatedActivityCreate,
            PendingActivityReturn pendingActivityReturn)
        {
            // Arrange
            pendingActivity.User.Id = userId;
            updatedActivityCreate.Type = pendingActivity.ActivityTypeId;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.PendingActivities.GetAsync(activityId))
               .ReturnsAsync(pendingActivity);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map<PendingActivityReturn>(pendingActivity))
                .Returns(pendingActivityReturn);

            // Act
            var res = await _sut.UpdatePendingActivityAsync(activityId, updatedActivityCreate);

            // Assert
            res.Match(
                pendingActivityReturnRes => pendingActivityReturnRes.Should().BeEquivalentTo(pendingActivityReturn),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.PendingActivities.GetAsync(activityId), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task UpdatePendingActivity_ActivityNotFoundAsync(int userId,
            int activityId,
            PendingActivity pendingActivity,
            ActivityCreate updatedActivityCreate,
            PendingActivityReturn pendingActivityReturn)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.PendingActivities.GetAsync(activityId))
               .ReturnsAsync((PendingActivity)null);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map<PendingActivityReturn>(pendingActivity))
                .Returns(pendingActivityReturn);

            // Act
            var res = await _sut.UpdatePendingActivityAsync(activityId, updatedActivityCreate);

            // Assert
            res.Match(
                pendingActivityReturnRes => pendingActivityReturnRes.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.PendingActivities.GetAsync(activityId), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task UpdatePendingActivity_InvalidCreatorAsync(int userId,
            int activityId,
            PendingActivity pendingActivity,
            ActivityCreate updatedActivityCreate,
            PendingActivityReturn pendingActivityReturn)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.PendingActivities.GetAsync(activityId))
               .ReturnsAsync(pendingActivity);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map<PendingActivityReturn>(pendingActivity))
                .Returns(pendingActivityReturn);

            // Act
            var res = await _sut.UpdatePendingActivityAsync(activityId, updatedActivityCreate);

            // Assert
            res.Match(
                pendingActivityReturnRes => pendingActivityReturnRes.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.PendingActivities.GetAsync(activityId), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task UpdatePendingActivity_TypeUpdateAsync(int userId,
            int activityId,
            PendingActivity pendingActivity,
            ActivityCreate updatedActivityCreate,
            PendingActivityReturn pendingActivityReturn)
        {
            // Arrange
            updatedActivityCreate.Type = ActivityTypeId.Puzzle;

            pendingActivity.User.Id = userId;
            pendingActivity.ActivityTypeId = ActivityTypeId.GoodDeed;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.PendingActivities.GetAsync(activityId))
               .ReturnsAsync(pendingActivity);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map<PendingActivityReturn>(pendingActivity))
                .Returns(pendingActivityReturn);

            // Act
            var res = await _sut.UpdatePendingActivityAsync(activityId, updatedActivityCreate);

            // Assert
            res.Match(
                pendingActivityReturnRes => pendingActivityReturnRes.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.PendingActivities.GetAsync(activityId), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task DisapprovePendingActivity_SuccessfullAsync(int activityId,
            PendingActivity pendingActivity)
        {
            // Arrange
            _uowMock.Setup(x => x.PendingActivities.GetAsync(activityId))
               .ReturnsAsync(pendingActivity);

            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, false))
                .Verifiable();

            _uowMock.Setup(x => x.PendingActivities.Remove(pendingActivity))
                .Verifiable();

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.DisapprovePendingActivityAsync(activityId);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.PendingActivities.GetAsync(activityId), Times.Once);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, false), Times.Once);
            _uowMock.Verify(x => x.PendingActivities.Remove(pendingActivity), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task DisapprovePendingActivity_ActivityNotFoundAsync(int activityId,
            PendingActivity pendingActivity)
        {
            // Arrange
            _uowMock.Setup(x => x.PendingActivities.GetAsync(activityId))
               .ReturnsAsync((PendingActivity)null);

            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, false))
                .Verifiable();

            _uowMock.Setup(x => x.PendingActivities.Remove(pendingActivity))
                .Verifiable();

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.DisapprovePendingActivityAsync(activityId);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.PendingActivities.GetAsync(activityId), Times.Once);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, false), Times.Never);
            _uowMock.Verify(x => x.PendingActivities.Remove(pendingActivity), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task CreatePendingActivity_SuccessfullAsync(int userId,
            PendingActivity pendingActivity,
            Skill skill,
            IEnumerable<SkillActivity> skillactivities,
            User user,
            ActivityCreate activityCreate,
            ActivityCreationCounter activityCreationCounter,
            PendingActivityReturn pendingActivityReturn)
        {
            // Arrange
            activityCreationCounter.User = pendingActivity.User;
            activityCreationCounter.ActivityTypeId = pendingActivity.ActivityTypeId;
            activityCreationCounter.DateCreated = DateTimeOffset.Now;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _mapperMock.Setup(x => x.Map<PendingActivity>(activityCreate))
                .Returns(pendingActivity);

            _uowMock.Setup(x => x.Skills.GetSkillAsync(userId, activityCreate.Type))
                .ReturnsAsync(skill);

            _uowMock.Setup(x => x.SkillActivities.GetAllAsync())
                .ReturnsAsync(skillactivities);

            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);

            _uowMock.Setup(x => x.PendingActivities.Add(pendingActivity))
                .Verifiable();

            _uowMock.Setup(x => x.ActivityCreationCounters.Add(activityCreationCounter))
                .Verifiable();

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock.Setup(x => x.Map<PendingActivityReturn>(pendingActivity))
                .Returns(pendingActivityReturn);
            // Act
            var res = await _sut.CreatePendingActivityAsync(activityCreate);

            // Assert
            res.Match(
                pendingActivityReturnRes => pendingActivityReturnRes.Should().BeEquivalentTo(pendingActivityReturn),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.Skills.GetSkillAsync(userId, activityCreate.Type), Times.Once);
            _uowMock.Verify(x => x.SkillActivities.GetAllAsync(), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.PendingActivities.Add(pendingActivity), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task CreatePendingActivity_UnAllowedToCreateAsync(int userId,
            PendingActivity pendingActivity,
            Skill skill,
            List<SkillActivity> skillactivities,
            User user,
            ActivityCreate activityCreate,
            ActivityCreationCounter activityCreationCounter,
            PendingActivityReturn pendingActivityReturn)
        {
            // Arrange
            activityCreate.Type = ActivityTypeId.Puzzle;

            skillactivities.Add(new SkillActivity { Counter = 2, Level = 1 });

            user.ActivityCreationCounters.Add(new ActivityCreationCounter() { ActivityTypeId = ActivityTypeId.Puzzle, DateCreated = DateTimeOffset.Now });
            user.ActivityCreationCounters.Add(new ActivityCreationCounter() { ActivityTypeId = ActivityTypeId.Puzzle, DateCreated = DateTimeOffset.Now });

            skill.ActivityTypeId = ActivityTypeId.Puzzle;
            skill.Level = 1;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _mapperMock.Setup(x => x.Map<PendingActivity>(activityCreate))
                .Returns(pendingActivity);

            _uowMock.Setup(x => x.Skills.GetSkillAsync(userId, activityCreate.Type))
                .ReturnsAsync(skill);

            _uowMock.Setup(x => x.SkillActivities.GetAllAsync())
                .ReturnsAsync(skillactivities);

            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);

            _uowMock.Setup(x => x.PendingActivities.Add(pendingActivity))
                .Verifiable();

            _uowMock.Setup(x => x.ActivityCreationCounters.Add(activityCreationCounter))
                .Verifiable();

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock.Setup(x => x.Map<PendingActivityReturn>(pendingActivity))
                .Returns(pendingActivityReturn);

            // Act
            var res = await _sut.CreatePendingActivityAsync(activityCreate);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.Skills.GetSkillAsync(userId, activityCreate.Type), Times.Once);
            _uowMock.Verify(x => x.SkillActivities.GetAllAsync(), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.PendingActivities.Add(pendingActivity), Times.Never);
            _uowMock.Verify(x => x.ActivityCreationCounters.Add(activityCreationCounter), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }
    }
}
