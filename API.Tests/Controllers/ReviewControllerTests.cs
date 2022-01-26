using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.Managers;
using Application.ServiceInterfaces;
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
        public void ReviewActivity_Successfull([Frozen] Mock<IReviewManager> reviewManagerMock,
            ActivityReview activityReview,
            [Greedy] ReviewController sut)
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

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void GetReviewsForUser_Successfull([Frozen] Mock<IActivityReviewService> activityReviewServiceMock,
            int userId, List<ActivityReviewedByUser> acitvitiesReviewed,
            [Greedy] ReviewController sut)
        {
            // Arrange
            activityReviewServiceMock.Setup(x => x.GetAllReviewsByUserId(userId))
                .ReturnsAsync(acitvitiesReviewed);

            // Act
            var res = sut.GetReviewsForUser(userId);

            // Assert
            res.Result.Should().Equal(acitvitiesReviewed);
            activityReviewServiceMock.Verify(x => x.GetAllReviewsByUserId(userId), Times.Once);
        }
    }
}
