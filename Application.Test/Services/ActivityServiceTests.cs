using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Errors;
using Application.Media;
using Application.Repositories;
using Application.ServiceInterfaces;
using Application.Services;
using AutoFixture;
using AutoFixture.NUnit3;
using AutoMapper;
using Domain.Entities;
using FixtureShared;
using FluentAssertions;
using Models.Activity;
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
        [Fixture(FixtureType.WithAutoMoq)]
        public void CreateActivityWithoutImageAsync_Successful(ActivityService sut)
        {

            // Arrange
            var activityCreate = _fixture
                .Build<ActivityCreate>()
                .Without(p => p.Images)
                .Create();

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
           [Frozen] Mock<IActivityRepository> activityRepoMock,
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
             .Setup(x => x.Map<PendingActivity>(It.IsAny<ActivityCreate>()))
             .Returns(pendingActivity);


            // Act
            Func<Task> methodInTest = async () => await sut.CreatePendingActivityAsync(It.IsAny<ActivityCreate>());

            // Assert
            methodInTest.Should().Throw<RestException>();
            photoAccessorMock.Verify(x => x.AddPhotoAsync(It.IsAny<ActivityCreate>().Images[0]), Times.Never);
            activityRepoMock.Verify(x => x.CreatePendingActivityAsync(pendingActivity), Times.Never);
            activityRepoMock.Verify(x => x.CreateActivityCreationCounter(It.IsAny<ActivityCreationCounter>()), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetPendingActivitiesAsync_Successful(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IActivityRepository> activityRepoMock,
            List<PendingActivity> pendingActivities,
            List<PendingActivityGet> pendingActivitiesGet,
            int limit, int offset,
            ActivityService sut)
        {

            // Arrange
            var pendingActivityEnvelope = _fixture.Create<PendingActivityEnvelope>();
            pendingActivityEnvelope.Activities = pendingActivitiesGet;
            pendingActivityEnvelope.ActivityCount = pendingActivitiesGet.Count;

            mapperMock
                .Setup(x => x.Map<List<PendingActivityGet>>(It.IsAny<PendingActivity>()))
                .Returns(pendingActivitiesGet);

            activityRepoMock
                .Setup(x => x.GetPendingActivitiesAsync(limit, offset))
                .ReturnsAsync(pendingActivities);

            activityRepoMock
                .Setup(x => x.GetPendingActivitiesCountAsync())
                .ReturnsAsync(pendingActivities.Count);

            // Act
            Func<Task<PendingActivityEnvelope>> methodInTest = async () => await sut.GetPendingActivitiesAsync(limit, offset);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            activityRepoMock.Verify(x => x.GetPendingActivitiesAsync(limit, offset), Times.Once);
            activityRepoMock.Verify(x => x.GetPendingActivitiesCountAsync(), Times.Once);

        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ApprovePendingActivityAsync_Successful(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IActivityRepository> activityRepoMock,
            [Frozen] Mock<IEmailService> emailServiceMock,
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

            activityRepoMock
                .Setup(x => x.GetPendingActivityByIDAsync(pendingActivityId))
                .ReturnsAsync(pendingActivity);

            // Act
            Func<Task> methodInTest = async () => await sut.ReslovePendingActivityAsync(pendingActivityId, approval);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            activityRepoMock.Verify(x => x.GetPendingActivityByIDAsync(pendingActivityId), Times.Once);
            activityRepoMock.Verify(x => x.CreateActivityAsync(activity), Times.Once);
            emailServiceMock.Verify(x => x.SendActivityApprovalEmailAsync(pendingActivity, approval.Approve), Times.Once);
            activityRepoMock.Verify(x => x.DeletePendingActivity(pendingActivity), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void DisapprovePendingActivityAsync_Successful(
          [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IActivityRepository> activityRepoMock,
            [Frozen] Mock<IEmailService> emailServiceMock,
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

            activityRepoMock
                .Setup(x => x.GetPendingActivityByIDAsync(pendingActivityId))
                .ReturnsAsync(pendingActivity);

            // Act
            Func<Task> methodInTest = async () => await sut.ReslovePendingActivityAsync(pendingActivityId, approval);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            activityRepoMock.Verify(x => x.GetPendingActivityByIDAsync(pendingActivityId), Times.Once);
            activityRepoMock.Verify(x => x.CreateActivityAsync(activity), Times.Never);
            emailServiceMock.Verify(x => x.SendActivityApprovalEmailAsync(pendingActivity, approval.Approve), Times.Once);
            activityRepoMock.Verify(x => x.DeletePendingActivity(pendingActivity), Times.Once);
        }
    }
}
