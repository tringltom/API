using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using AutoFixture.NUnit3;
using FixtureShared;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    public class ActivityControllerTests
    {

        [SetUp]
        public void SetUp() { }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void CreateActivity_Successfull(
            [Frozen] Mock<IActivityService> activityServiceMock,
            ActivityCreate activityCreate,
            [Greedy] ActivityController sut)
        {
            // Arrange
            activityServiceMock.Setup(x => x.CreatePendingActivityAsync(activityCreate))
               .Returns(Task.CompletedTask);

            // Act
            var res = sut.CreatePendingActivity(activityCreate);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void GetPendingActivities_Successfull(
            [Frozen] Mock<IActivityService> activityServiceMock,
            PendingActivityEnvelope pendingActivityEnvelope,
            [Greedy] ActivityController sut)
        {
            // Arrange
            activityServiceMock.Setup(x => x.GetPendingActivitiesAsync(5, 2))
               .ReturnsAsync(pendingActivityEnvelope);

            // Act
            var res = sut.GetPendingActivities(5, 2);

            // Assert

            res.Should().BeOfType<Task<ActionResult<PendingActivityEnvelope>>>();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void ResolvePendingActivitiy_Successfull(
            [Frozen] Mock<IActivityService> activityServiceMock,
            PendingActivityApproval activityApproval,
            [Greedy] ActivityController sut)
        {
            // Arrange
            activityServiceMock.Setup(x => x.ReslovePendingActivityAsync(1, activityApproval))
               .ReturnsAsync(true);

            // Act
            var res = sut.ResolvePendingActivity(1, activityApproval);

            // Assert
            res.Should().BeOfType<Task<ActionResult<bool>>>();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void GetApprovedActivitiesExcludeUser_Successfull(
            [Frozen] Mock<IActivityService> activityServiceMock,
            ApprovedActivityEnvelope activityEnvelope,
            int id,
            int offset,
            int limit,
            [Greedy] ActivityController sut)
        {
            // Arrange
            activityServiceMock.Setup(x => x.GetApprovedActivitiesFromOtherUsersAsync(id, limit, offset))
               .ReturnsAsync(activityEnvelope);

            // Act
            var res = sut.GetApprovedActivitiesExcludeUser(id, limit, offset);

            // Assert
            res.Should().BeOfType<Task<ActionResult<ApprovedActivityEnvelope>>>();
        }


    }
}
