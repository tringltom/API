using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.Managers;
using AutoFixture.NUnit3;
using FixtureShared;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Models.Activity;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    internal class ReviewControllerTests
    {
        [SetUp]
        public void SetUp() { }


        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void ReviewActivity_Successfull([Frozen] Mock<IReviewManager> reviewManagerMock, ActivityReview activityReview, [Greedy] ReviewController sut)
        {
            // Arrange
            reviewManagerMock.Setup(x => x.ReviewActivityAsync(activityReview))
                .Returns(Task.CompletedTask);

            // Act
            var res = sut.ReviewActivity(activityReview);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            reviewManagerMock.Verify(x => x.ReviewActivityAsync(activityReview), Times.Once);
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }
    }
}
