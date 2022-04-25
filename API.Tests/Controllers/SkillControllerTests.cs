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
        private SkillController _sut;

        [SetUp]
        public void SetUp()
        {
            _skillServiceMock = new Mock<ISkillService>();
            _sut = new SkillController(_skillServiceMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetSkillsDataAsync_Successfull(SkillData skillData)
        {
            // Arrange
            _skillServiceMock.Setup(x => x.GetSkillsDataAsync(It.IsAny<int>()))
               .ReturnsAsync(skillData);

            // Act
            var res = await _sut.GetSkillsData(It.IsAny<int>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(skillData);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task UpdateSkillsDataAsync_Successfull(SkillData skillData, UserBaseResponse userBaseResponse)
        {
            // Arrange
            _skillServiceMock.Setup(x => x.UpdateSkillsDataAsync(skillData))
               .ReturnsAsync(userBaseResponse);

            // Act
            var res = await _sut.UpdateSkillsData(skillData) as OkObjectResult;

            // Assert
            res.Value.Should().Be(userBaseResponse);
        }
    }
}
