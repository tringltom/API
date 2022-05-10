using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using DAL.Query;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    public class PendingActivityControllerTests
    {
        private Mock<IPendingActivityService> _pendingActivityServiceMock;
        private PendingActivityController _sut;

        [SetUp]
        public void SetUp()
        {
            _pendingActivityServiceMock = new Mock<IPendingActivityService>();
            _sut = new PendingActivityController(_pendingActivityServiceMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetPendingActivities_SuccessfullAsync(PendingActivityEnvelope pendingActivityEnvelope)
        {
            // Arrange
            _pendingActivityServiceMock.Setup(x => x.GetPendingActivitiesAsync(It.IsAny<QueryObject>()))
               .ReturnsAsync(pendingActivityEnvelope);

            // Act
            var res = await _sut.GetPendingActivities(It.IsAny<QueryObject>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(pendingActivityEnvelope);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetOwnerPendingActivities_SuccessfullAsync(PendingActivityForUserEnvelope pendingActivityForUserEnvelope)
        {
            // Arrange
            _pendingActivityServiceMock.Setup(x => x.GetOwnerPendingActivitiesAsync(It.IsAny<ActivityQuery>()))
               .ReturnsAsync(pendingActivityForUserEnvelope);

            // Act
            var res = await _sut.GetOwnerPendingActivities(It.IsAny<ActivityQuery>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(pendingActivityForUserEnvelope);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetOwnerPendingActivity_SuccessfullAsync(ActivityCreate activityCreate)
        {
            // Arrange
            _pendingActivityServiceMock.Setup(x => x.GetOwnerPendingActivityAsync(It.IsAny<int>()))
               .ReturnsAsync(activityCreate);

            // Act
            var res = await _sut.GetOwnerPendingActivity(It.IsAny<int>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(activityCreate);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task UpdatePendingActivity_SuccessfullAsync(ActivityCreate updatedactivityCreate, PendingActivityReturn pendingActivityReturn)
        {
            // Arrange
            _pendingActivityServiceMock.Setup(x => x.UpdatePendingActivityAsync(It.IsAny<int>(), updatedactivityCreate))
               .ReturnsAsync(pendingActivityReturn);

            // Act
            var res = await _sut.UpdatePendingActivitiy(It.IsAny<int>(), updatedactivityCreate) as CreatedAtRouteResult;

            // Assert
            res.Value.Should().Be(pendingActivityReturn);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task DisapprovePendingActivity_SuccessfullAsync()
        {
            // Arrange
            _pendingActivityServiceMock.Setup(x => x.DisapprovePendingActivityAsync(It.IsAny<int>()))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.DisapprovePendingActivity(It.IsAny<int>()) as NoContentResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.NoContent);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task CreatePendingActivity_SuccessfullAsync(ActivityCreate activityCreate, PendingActivityReturn pendingActivityReturn)
        {
            // Arrange
            _pendingActivityServiceMock.Setup(x => x.CreatePendingActivityAsync(activityCreate))
               .ReturnsAsync(pendingActivityReturn);

            // Act
            var res = await _sut.CreatePendingActivity(activityCreate) as CreatedAtRouteResult;

            // Assert
            res.Value.Should().Be(pendingActivityReturn);
        }
    }
}
