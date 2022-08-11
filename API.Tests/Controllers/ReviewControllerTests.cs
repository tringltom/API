using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    public class ReviewControllerTests
    {
        private Mock<IReviewService> _reviewServiceMock;
        private ReviewController _sut;

        [SetUp]
        public void SetUp()
        {
            _reviewServiceMock = new Mock<IReviewService>();
            _sut = new ReviewController(_reviewServiceMock.Object);
        }


        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetPendingActivities_SuccessfullAsync(List<UserReviewedActivity> userReviewedActivities)
        {
            // Arrange
            _reviewServiceMock.Setup(x => x.GetOwnerReviewsAsync())
               .ReturnsAsync(userReviewedActivities);

            // Act
            var res = await _sut.GetOwnerReviews() as OkObjectResult;

            // Assert
            res.Value.Should().Be(userReviewedActivities);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task ReviewActivity_SuccessfullAsync(ActivityReview activityReview)
        {
            // Arrange
            _reviewServiceMock.Setup(x => x.ReviewActivityAsync(activityReview))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.ReviewActivity(activityReview) as OkResult;

            // Assert
            res.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}
