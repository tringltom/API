using System;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.RepositoryInterfaces;
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
    public class ActivityReviewServiceTests
    {
        [SetUp]
        public void SetUp()
        { }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void UpdateReviewActivityAsync_Successful(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IUserReviewRepository> userReviewRepoMock,
            ActivityReview activityReview,
            UserReview review,
            ActivityReviewService sut)
        {

            // Arrange
            mapperMock.Setup(x => x.Map<UserReview>(activityReview))
                .Returns(review);

            userReviewRepoMock.Setup(x => x.UpdateUserActivityReviewAsync(review))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.UpdateReviewActivityAsync(activityReview);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            mapperMock.Verify(x => x.Map<UserReview>(activityReview), Times.Once);
            userReviewRepoMock.Verify(x => x.UpdateUserActivityReviewAsync(review), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void UpdateReviewActivityAsync_ReviewFailed(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IUserReviewRepository> userReviewRepoMock,
            ActivityReview activityReview,
            UserReview review,
            ActivityReviewService sut)
        {

            // Arrange
            mapperMock.Setup(x => x.Map<UserReview>(activityReview))
                .Returns(review);

            userReviewRepoMock.Setup(x => x.UpdateUserActivityReviewAsync(review))
                .Throws(new RestException(HttpStatusCode.InternalServerError, new { Activity = "Neuspešna izmena ocene aktivnosti." }));

            // Act
            Func<Task> methodInTest = async () => await sut.UpdateReviewActivityAsync(activityReview);

            // Assert
            methodInTest.Should().Throw<RestException>();
            mapperMock.Verify(x => x.Map<UserReview>(activityReview), Times.Once);
            userReviewRepoMock.Verify(x => x.UpdateUserActivityReviewAsync(review), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void AddReviewActivityAsync_Successful(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IUserReviewRepository> userReviewRepoMock,
            ActivityReview activityReview,
            UserReview review,
            ActivityReviewService sut)
        {

            // Arrange
            mapperMock.Setup(x => x.Map<UserReview>(activityReview))
                .Returns(review);

            userReviewRepoMock.Setup(x => x.ReviewUserActivityAsync(review))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.AddReviewActivityAsync(activityReview);

            // Assert
            methodInTest.Should().NotThrow<RestException>();
            mapperMock.Verify(x => x.Map<UserReview>(activityReview), Times.Once);
            userReviewRepoMock.Verify(x => x.ReviewUserActivityAsync(review), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void AddReviewActivityAsync_ReviewFailed(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IUserReviewRepository> userReviewRepoMock,
            ActivityReview activityReview,
            UserReview review,
            ActivityReviewService sut)
        {

            // Arrange
            mapperMock.Setup(x => x.Map<UserReview>(activityReview))
                .Returns(review);

            userReviewRepoMock.Setup(x => x.ReviewUserActivityAsync(review))
                .Throws(new RestException(HttpStatusCode.InternalServerError, new { Activity = "Neuspešna izmena ocene aktivnosti." }));

            // Act
            Func<Task> methodInTest = async () => await sut.AddReviewActivityAsync(activityReview);

            // Assert
            methodInTest.Should().Throw<RestException>();
            mapperMock.Verify(x => x.Map<UserReview>(activityReview), Times.Once);
            userReviewRepoMock.Verify(x => x.ReviewUserActivityAsync(review), Times.Once);
        }
    }
}
