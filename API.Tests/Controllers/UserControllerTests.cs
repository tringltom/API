using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.Models;
using Application.Models.User;
using Application.ServiceInterfaces;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    public class UserControllerTests
    {
        private Mock<IUsersService> _userServiceMock;
        private UserController _sut;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUsersService>();
            _sut = new UserController(_userServiceMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetRankedUsers_Successfull(UserRankedEnvelope userArenaEnvelope)
        {
            // Arrange
            _userServiceMock.Setup(x => x.GetRankedUsersAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(userArenaEnvelope);

            // Act
            var res = await _sut.GetRankedUsers(It.IsAny<int>(), It.IsAny<int>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(userArenaEnvelope);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetImagesForApproval_Successfull(UserImageEnvelope userImageEnvelope)
        {
            // Arrange
            _userServiceMock.Setup(x => x.GetImagesForApprovalAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(userImageEnvelope);

            // Act
            var res = await _sut.GetImagesForApproval(It.IsAny<int>(), It.IsAny<int>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(userImageEnvelope);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task UpdateAbout_Successfull(UserAbout userAbout)
        {
            // Arrange
            _userServiceMock.Setup(x => x.UpdateAboutAsync(userAbout))
                .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.UpdateAbout(userAbout) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task UpdateImage_Successfull(UserImageUpdate userImageUpdate)
        {
            // Arrange
            _userServiceMock.Setup(x => x.UpdateImageAsync(userImageUpdate))
                .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.UpdateImage(userImageUpdate) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ResolveImage_Successfull(PhotoApprove photoApprove)
        {
            // Arrange
            _userServiceMock.Setup(x => x.ResolveImageAsync(It.IsAny<int>(), photoApprove.Approve))
                .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.ResolveImage(It.IsAny<int>(), photoApprove) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }
    }
}
