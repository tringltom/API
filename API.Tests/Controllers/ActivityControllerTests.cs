using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.ServiceInterfaces;
using AutoFixture.NUnit3;
using FixtureShared;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Models.Activity;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers;

public class ActivityControllerTests
{

    [SetUp]
    public void SetUp() { }

    [Test]
    [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
    public void CreateActivity_Successfull(
        [Frozen] Mock<IActivityService> activityServiceMock,
        ActivityCreate activityCreate,
        [Greedy] ActivityController sut)
    {
        // Arrange
        activityServiceMock.Setup(x => x.CreateActivityAsync(activityCreate))
            .Returns(Task.CompletedTask);

        // Act
        var res = sut.CreateActivity(activityCreate);

        // Assert
        res.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
    }
}
