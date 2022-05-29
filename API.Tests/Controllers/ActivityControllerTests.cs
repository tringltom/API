using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using AutoFixture;
using DAL.Query;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    public class ActivityControllerTests
    {
        private Mock<IActivityService> _activityServiceMock;
        private ActivityController _sut;

        [SetUp]
        public void SetUp()
        {
            _activityServiceMock = new Mock<IActivityService>();
            _sut = new ActivityController(_activityServiceMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetActivity_SuccessfullAsync(ApprovedActivityReturn activityReturn)
        {
            // Arrange
            _activityServiceMock.Setup(x => x.GetActivityAsync(It.IsAny<int>()))
               .ReturnsAsync(activityReturn);

            // Act
            var res = await _sut.GetActivity(It.IsAny<int>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(activityReturn);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetActivitiesFromOtherUsers_SuccessfullAsync(ApprovedActivityEnvelope activityReturnEnvelope)
        {
            // Arrange
            _activityServiceMock.Setup(x => x.GetActivitiesFromOtherUsersAsync(It.IsAny<ActivityQuery>()))
               .ReturnsAsync(activityReturnEnvelope);

            // Act
            var res = await _sut.GetActivitiesFromOtherUsers(It.IsAny<ActivityQuery>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(activityReturnEnvelope);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetHappeningsForApproval_SuccessfullAsync(HappeningEnvelope happeningEnvelope)
        {
            // Arrange
            _activityServiceMock.Setup(x => x.GetHappeningsForApprovalAsync(It.IsAny<QueryObject>()))
               .ReturnsAsync(happeningEnvelope);

            // Act
            var res = await _sut.GetHappeningsForApproval(It.IsAny<QueryObject>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(happeningEnvelope);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task AnswerToPuzzle_SuccessfullAsync(PuzzleAnswer puzzleAnswer, int xpReward)
        {
            // Arrange
            _activityServiceMock.Setup(x => x.AnswerToPuzzleAsync(It.IsAny<int>(), puzzleAnswer))
               .ReturnsAsync(xpReward);

            // Act
            var res = await _sut.AnswerToPuzzle(It.IsAny<int>(), puzzleAnswer) as OkObjectResult;

            // Assert
            res.Value.Should().Be(xpReward);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task ConfirmAttendenceToHappening_SuccessfullAsync(ActivityReview activityReview)
        {
            // Arrange
            _activityServiceMock.Setup(x => x.ConfirmAttendenceToHappeningAsync(It.IsAny<int>()))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.ConfirmAttendenceToHappening(It.IsAny<int>()) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task ApproveHappeningCompletition_SuccessfullAsync(HappeningApprove happeningApprove)
        {
            // Arrange
            _activityServiceMock.Setup(x => x.ApproveHappeningCompletitionAsync(It.IsAny<int>(), happeningApprove.Approve))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.ApproveHappeningCompletition(It.IsAny<int>(), happeningApprove) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task CancelAttendenceToHappening_SuccessfullAsync()
        {
            // Arrange
            _activityServiceMock.Setup(x => x.AttendToHappeningAsync(It.IsAny<int>(), false))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.CancelAttendenceToHappening(It.IsAny<int>()) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task ApprovePendingActivity_SuccessfullAsync(ApprovedActivityReturn activity)
        {
            // Arrange
            _activityServiceMock.Setup(x => x.ApprovePendingActivity(It.IsAny<int>()))
               .ReturnsAsync(activity);

            // Act
            var res = await _sut.ApprovePendingActivity(It.IsAny<int>()) as CreatedAtRouteResult;

            // Assert
            res.Value.Should().Be(activity);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task AttendToHappening_SuccessfullAsync()
        {
            // Arrange
            _activityServiceMock.Setup(x => x.AttendToHappeningAsync(It.IsAny<int>(), true))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.AttendToHappening(It.IsAny<int>()) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task CompleteHappening_SuccessfullAsync(HappeningUpdate happeningUpdate)
        {
            // Arrange
            _activityServiceMock.Setup(x => x.CompleteHappeningAsync(It.IsAny<int>(), happeningUpdate))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.CompleteHappening(It.IsAny<int>(), happeningUpdate) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }
    }
}
