using System.Threading.Tasks;
using API.Controllers;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using AutoFixture;
using DAL.Query;
using FixtureShared;
using FluentAssertions;
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
        public async Task GetActivitiesFromOtherUsers_SuccessfullAsync(ActivitiesFromOtherUserEnvelope activityReturnEnvelope)
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
        public async Task GetApprovedActivitiesForUser_SuccessfullAsync(ApprovedActivityEnvelope approvedActivitiesEnvelope)
        {
            // Arrange
            _activityServiceMock.Setup(x => x.GetApprovedActivitiesCreatedByUserAsync(It.IsAny<int>(), It.IsAny<ActivityQuery>()))
               .ReturnsAsync(approvedActivitiesEnvelope);

            // Act
            var res = await _sut.GetApprovedActivitiesCreatedByUser(It.IsAny<int>(), It.IsAny<ActivityQuery>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(approvedActivitiesEnvelope);
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

    }
}
