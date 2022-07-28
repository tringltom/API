using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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
    public class HappeningControllerTests
    {
        private Mock<IHappeningService> _happeningServiceMock;
        private HappeningController _sut;

        [SetUp]
        public void SetUp()
        {
            _happeningServiceMock = new Mock<IHappeningService>();
            _sut = new HappeningController(_happeningServiceMock.Object);
        }


        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetHappeningsForApproval_SuccessfullAsync(HappeningEnvelope happeningEnvelope)
        {
            // Arrange
            _happeningServiceMock.Setup(x => x.GetHappeningsForApprovalAsync(It.IsAny<QueryObject>()))
               .ReturnsAsync(happeningEnvelope);

            // Act
            var res = await _sut.GetHappeningsForApproval(It.IsAny<QueryObject>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(happeningEnvelope);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task ConfirmAttendenceToHappening_SuccessfullAsync(ActivityReview activityReview)
        {
            // Arrange
            _happeningServiceMock.Setup(x => x.ConfirmAttendenceToHappeningAsync(It.IsAny<int>()))
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
            _happeningServiceMock.Setup(x => x.ApproveHappeningCompletitionAsync(It.IsAny<int>(), happeningApprove.Approve))
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
            _happeningServiceMock.Setup(x => x.AttendToHappeningAsync(It.IsAny<int>(), false))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.CancelAttendenceToHappening(It.IsAny<int>()) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task AttendToHappening_SuccessfullAsync()
        {
            // Arrange
            _happeningServiceMock.Setup(x => x.AttendToHappeningAsync(It.IsAny<int>(), true))
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
            _happeningServiceMock.Setup(x => x.CompleteHappeningAsync(It.IsAny<int>(), happeningUpdate))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.CompleteHappening(It.IsAny<int>(), happeningUpdate) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }
    }
}
