using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.InfrastructureModels;
using Application.Models.Activity;
using Application.Services;
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
    public class ActivityServiceTests
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithAutoMoqAndOmitRecursion();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task CreateActivityWithoutImageAsync_Successful(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            int userId,
            User user,
            PendingActivity activity,
            List<SkillActivity> skillActivities,
            ActivityService sut)
        {

            // Arrange
            var activityCreate = _fixture
                .Build<ActivityCreate>()
                .Without(p => p.Images)
                .Create();

            var skill = _fixture
                .Build<Skill>()
                .With(s => s.ActivityTypeId, activityCreate.Type)
                .Create();

            mapperMock
                .Setup(x => x.Map<PendingActivity>(activityCreate))
                .Returns(activity);

            userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
               .Returns(userId);

            uowMock.Setup(x => x.Users.GetAsync(userId))
               .ReturnsAsync(user);

            uowMock.Setup(x => x.Skills.GetSkillAsync(It.IsAny<int>(), activityCreate.Type))
                .ReturnsAsync(skill);

            uowMock.Setup(x => x.SkillActivities.GetAllAsync())
                .ReturnsAsync(skillActivities);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.CreatePendingActivityAsync(activityCreate);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void CreateActivityWithImageAsync_Successful(
            [Frozen] Mock<IPhotoAccessor> photoAccessorMock,
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            int userId,
            User user,
            ActivityCreate activityCreate,
            PhotoUploadResult photoUploadResult,
            PendingActivity activity,
            List<SkillActivity> skillActivities,
            ActivityService sut)
        {

            // Arrange
            var skill = _fixture
              .Build<Skill>()
              .With(s => s.ActivityTypeId, activityCreate.Type)
              .Create();

            mapperMock
                .Setup(x => x.Map<PendingActivity>(It.IsAny<ActivityCreate>()))
                .Returns(activity);

            photoAccessorMock
                .Setup(x => x.AddPhotoAsync(activityCreate.Images[0]))
                .ReturnsAsync(photoUploadResult);

            userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
               .Returns(userId);

            uowMock.Setup(x => x.Users.GetAsync(userId))
               .ReturnsAsync(user);

            uowMock.Setup(x => x.Skills.GetSkillAsync(It.IsAny<int>(), activityCreate.Type))
                .ReturnsAsync(skill);

            uowMock.Setup(x => x.SkillActivities.GetAllAsync())
                .ReturnsAsync(skillActivities);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.CreatePendingActivityAsync(activityCreate);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            photoAccessorMock.Verify(x => x.AddPhotoAsync(activityCreate.Images[0]), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void CreatePendingActivity_ExceededCount(
           [Frozen] Mock<IPhotoAccessor> photoAccessorMock,
           [Frozen] Mock<IMapper> mapperMock,
           [Frozen] Mock<IUnitOfWork> uowMock,
           [Frozen] Mock<IUserAccessor> userAccessorMock,
           int userId,
           ActivityCreate activityCreate,
           List<SkillActivity> skillActivities,
           ActivityService sut,
           User user)
        {

            // Arrange
            var userWithNoMoreGoodDeedCount = _fixture
               .Build<User>()
               .With(ac => ac.ActivityCreationCounters,
                    new List<ActivityCreationCounter>()
                    {
                        new ActivityCreationCounter
                        {
                            ActivityTypeId = ActivityTypeId.GoodDeed,
                            DateCreated = DateTimeOffset.Now,
                            User = user
                        },
                        new ActivityCreationCounter
                        {
                            ActivityTypeId = ActivityTypeId.GoodDeed,
                            DateCreated = DateTimeOffset.Now,
                            User = user
                        },
                    })
               .Create();

            var pendingActivity = _fixture
            .Build<PendingActivity>()
            .With(u => u.User, userWithNoMoreGoodDeedCount)
            .Create();

            var skill = _fixture
                .Build<Skill>()
                .With(s => s.ActivityTypeId, activityCreate.Type)
                .With(s => s.Level, 0)
                .Create();

            var skillActivity = _fixture
                .Build<SkillActivity>()
                .With(s => s.Level, 0)
                .With(s => s.Counter, 2)
                .Create();

            skillActivities.Add(skillActivity);

            mapperMock
             .Setup(x => x.Map<PendingActivity>(activityCreate))
             .Returns(pendingActivity);

            userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
               .Returns(userId);

            uowMock.Setup(x => x.Users.GetAsync(userId))
               .ReturnsAsync(userWithNoMoreGoodDeedCount);

            uowMock.Setup(x => x.Skills.GetSkillAsync(It.IsAny<int>(), activityCreate.Type))
                .ReturnsAsync(skill);

            uowMock.Setup(x => x.SkillActivities.GetAllAsync())
                .ReturnsAsync(skillActivities);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);


            // Act
            Func<Task> methodInTest = async () => await sut.CreatePendingActivityAsync(activityCreate);

            // Assert
            methodInTest.Should().Throw<RestError>();
            photoAccessorMock.Verify(x => x.AddPhotoAsync(activityCreate.Images[0]), Times.Never);
            uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetPendingActivitiesAsync_Successful(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            List<PendingActivity> pendingActivities,
            List<PendingActivityReturn> pendingActivitiesGet,
            int limit, int offset,
            ActivityService sut)
        {

            // Arrange
            var pendingActivityEnvelope = _fixture.Create<PendingActivityEnvelope>();
            pendingActivityEnvelope.Activities = pendingActivitiesGet;
            pendingActivityEnvelope.ActivityCount = pendingActivitiesGet.Count;

            mapperMock
                .Setup(x => x.Map<List<PendingActivityReturn>>(It.IsAny<PendingActivity>()))
                .Returns(pendingActivitiesGet);

            uowMock
                .Setup(x => x.PendingActivities.GetLatestPendingActivitiesAsync(limit, offset))
                .ReturnsAsync(pendingActivities);

            uowMock
                .Setup(x => x.PendingActivities.CountAsync())
                .ReturnsAsync(pendingActivities.Count);

            // Act
            Func<Task<PendingActivityEnvelope>> methodInTest = async () => await sut.GetPendingActivitiesAsync(limit, offset);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            uowMock.Verify(x => x.PendingActivities.GetLatestPendingActivitiesAsync(limit, offset), Times.Once);
            uowMock.Verify(x => x.PendingActivities.CountAsync(), Times.Once);

        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetPendingActivitiesForLoggedInUserAsync_Successful(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            List<PendingActivity> pendingActivities,
            List<PendingActivityForUserReturn> pendingActivitiesForUserGet,
            int userId, int limit, int offset,
            ActivityService sut)
        {

            // Arrange
            var pendingActivityEnvelope = _fixture.Create<PendingActivityForUserEnvelope>();
            pendingActivityEnvelope.Activities = pendingActivitiesForUserGet;
            pendingActivityEnvelope.ActivityCount = pendingActivitiesForUserGet.Count;

            mapperMock
                .Setup(x => x.Map<List<PendingActivityForUserReturn>>(It.IsAny<PendingActivity>()))
                .Returns(pendingActivitiesForUserGet);

            userAccessorMock.Setup(urm => urm.GetUserIdFromAccessToken())
             .Returns(userId);

            uowMock
                .Setup(x => x.PendingActivities.GetLatestPendingActivitiesAsync(userId, limit, offset))
                .ReturnsAsync(pendingActivities);

            uowMock
                .Setup(x => x.PendingActivities.CountPendingActivitiesAsync(userId))
                .ReturnsAsync(pendingActivities.Count);

            // Act
            Func<Task<PendingActivityForUserEnvelope>> methodInTest = async () => await sut.GetOwnerPendingActivitiesAsync(limit, offset);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            uowMock.Verify(x => x.PendingActivities.GetLatestPendingActivitiesAsync(userId, limit, offset), Times.Once);
            uowMock.Verify(x => x.PendingActivities.CountPendingActivitiesAsync(userId), Times.Once);

        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ApprovePendingActivityAsync_Successful(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IEmailManager> emailManagerMock,
            int pendingActivityId,
            PendingActivity pendingActivity,
            Activity activity,
            ActivityService sut)
        {

            // Arrange
            var approval = _fixture.Create<PendingActivityApproval>();
            approval.Approve = true;

            mapperMock
                .Setup(x => x.Map<Activity>(pendingActivity))
                .Returns(activity);

            uowMock
                .Setup(x => x.PendingActivities.GetAsync(pendingActivityId))
                .ReturnsAsync(pendingActivity);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, approval.Approve));

            // Act
            Func<Task> methodInTest = async () => await sut.ReslovePendingActivityAsync(pendingActivityId, approval);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            uowMock.Verify(x => x.PendingActivities.GetAsync(pendingActivityId), Times.Once);
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, approval.Approve), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void DisapprovePendingActivityAsync_Successful(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IEmailManager> emailManagerMock,
            int pendingActivityId,
            PendingActivity pendingActivity,
            Activity activity,
            ActivityService sut)
        {

            // Arrange
            var approval = _fixture.Create<PendingActivityApproval>();
            approval.Approve = false;

            mapperMock
                .Setup(x => x.Map<Activity>(pendingActivity))
                .Returns(activity);

            uowMock
                .Setup(x => x.PendingActivities.GetAsync(pendingActivityId))
                .ReturnsAsync(pendingActivity);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, approval.Approve));


            // Act
            Func<Task> methodInTest = async () => await sut.ReslovePendingActivityAsync(pendingActivityId, approval);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            uowMock.Verify(x => x.PendingActivities.GetAsync(pendingActivityId), Times.Once);
            emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, approval.Approve), Times.Once);
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }
    }
}
