using System.Threading.Tasks;
using API.Controllers;
using Application.Models;
using Application.Models.User;
using Application.ServiceInterfaces;
using FixtureShared;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    public class SkillControllerTests
    {
        private Mock<ISkillService> _skillServiceMock;
        private SkillController _skillController;

        [SetUp]
        public void SetUp()
        {
            _skillServiceMock = new Mock<ISkillService>();
            _skillController = new SkillController(_skillServiceMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void GetSkillsDataAsync_Successfull(int userId, SkillData skillData)
        {
            // Arrange
            _skillServiceMock.Setup(x => x.GetSkillsDataAsync(userId))
               .ReturnsAsync(skillData);

            // Act
            var res = _skillController.GetSkillsDataAsync(userId);

            // Assert
            res.Should().BeOfType<Task<SkillData>>();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void ResetSkillsDataAsync_Successfull(UserBaseResponse userBaseResponse)
        {
            // Arrange
            _skillServiceMock.Setup(x => x.ResetSkillsDataAsync())
               .ReturnsAsync(userBaseResponse);

            // Act
            var res = _skillController.ResetSkillsDataAsync();

            // Assert
            res.Should().BeOfType<Task<ActionResult<UserBaseResponse>>>();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void UpdateSkillsDataAsync_Successfull(SkillData skillData, UserBaseResponse userBaseResponse)
        {
            // Arrange
            _skillServiceMock.Setup(x => x.UpdateSkillsDataAsync(skillData))
               .ReturnsAsync(userBaseResponse);

            // Act
            var res = _skillController.UpdateSkillsDataAsync(skillData);

            // Assert
            res.Should().BeOfType<Task<ActionResult<UserBaseResponse>>>();
        }
    }
}
