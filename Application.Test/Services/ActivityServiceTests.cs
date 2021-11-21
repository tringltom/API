using System;
using System.Threading.Tasks;
using Application.Media;
using Application.Services;
using AutoFixture;
using AutoFixture.NUnit3;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Models.Activity;
using Moq;
using NUnit.Framework;
using SuperFixture.Fixtures;

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
                .Without(p => p.Image)
                .Create();

            // Act
            Func<Task> methodInTest = async () => await sut.CreateActivityAsync(activityCreate);

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
                .Setup(x => x.AddPhotoAsync(activityCreate.Image))
                .ReturnsAsync(photoUploadResult);

            // Act
            Func<Task> methodInTest = async () => await sut.CreateActivityAsync(activityCreate);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            photoAccessorMock.Verify(x => x.AddPhotoAsync(activityCreate.Image), Times.Once);
        }
    }
}
