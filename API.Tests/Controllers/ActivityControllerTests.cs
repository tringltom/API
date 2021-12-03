using Models.Activity;

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
