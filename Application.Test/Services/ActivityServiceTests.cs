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
        public void CreateActivityWithoutImageAsync_Successful(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            PendingActivity activity,
            ActivityService sut)
        {

            // Arrange
            var activityCreate = _fixture
                .Build<ActivityCreate>()
                .Without(p => p.Images)
                .Create();

            mapperMock
                .Setup(x => x.Map<PendingActivity>(activityCreate))
                .Returns(activity);

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
            ActivityCreate activityCreate,
            PhotoUploadResult photoUploadResult,
            PendingActivity activity,
            ActivityService sut)
        {

            // Arrange
            mapperMock
                .Setup(x => x.Map<PendingActivity>(It.IsAny<ActivityCreate>()))
                .Returns(activity);

            photoAccessorMock
                .Setup(x => x.AddPhotoAsync(activityCreate.Images[0]))
                .ReturnsAsync(photoUploadResult);

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
           ActivityCreate activityCreate,
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

            mapperMock
             .Setup(x => x.Map<PendingActivity>(activityCreate))
             .Returns(pendingActivity);


            // Act
            Func<Task> methodInTest = async () => await sut.CreatePendingActivityAsync(activityCreate);

            // Assert
            methodInTest.Should().Throw<RestException>();
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
                .Setup(x => x.PendingActivities.GetLatestPendingActivities(limit, offset))
                .ReturnsAsync(pendingActivities);

            uowMock
                .Setup(x => x.PendingActivities.CountAsync())
                .ReturnsAsync(pendingActivities.Count);

            // Act
            Func<Task<PendingActivityEnvelope>> methodInTest = async () => await sut.GetPendingActivitiesAsync(limit, offset);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            uowMock.Verify(x => x.PendingActivities.GetLatestPendingActivities(limit, offset), Times.Once);
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
                .Setup(x => x.PendingActivities.GetLatestPendingActivities(userId, limit, offset))
                .ReturnsAsync(pendingActivities);

            uowMock
                .Setup(x => x.PendingActivities.CountPendingActivities(userId))
                .ReturnsAsync(pendingActivities.Count);

            // Act
            Func<Task<PendingActivityForUserEnvelope>> methodInTest = async () => await sut.GetPendingActivitiesForLoggedInUserAsync(limit, offset);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            uowMock.Verify(x => x.PendingActivities.GetLatestPendingActivities(userId, limit, offset), Times.Once);
            uowMock.Verify(x => x.PendingActivities.CountPendingActivities(userId), Times.Once);

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

            // Act
            Func<Task> methodInTest = async () => await sut.ReslovePendingActivityAsync(pendingActivityId, approval);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            uowMock.Verify(x => x.PendingActivities.GetAsync(pendingActivityId), Times.Once);
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(pendingActivity, approval.Approve), Times.Once);
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

            // Act
            Func<Task> methodInTest = async () => await sut.ReslovePendingActivityAsync(pendingActivityId, approval);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            uowMock.Verify(x => x.PendingActivities.GetAsync(pendingActivityId), Times.Once);
            emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(pendingActivity, approval.Approve), Times.Once);
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }
    }
}
