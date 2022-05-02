using System.Threading.Tasks;
using API.Controllers;
using Application.Models;
using Application.ServiceInterfaces;
using AutoFixture.NUnit3;
using FixtureShared;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    public class DiceControllerTests
    {
        [SetUp]
        public void SetUp() { }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task RollTheDice_SuccessfullAsync(
            [Frozen] Mock<IDiceService> diceServiceMock,
            DiceResult diceResult,
            [Greedy] DiceController sut)
        {
            // Arrange
            diceServiceMock.Setup(x => x.RollAsync())
               .ReturnsAsync(diceResult);

            // Act
            var res = await sut.RollDice() as OkObjectResult; ;

            // Assert
            res.Value.Should().Be(diceResult);
        }
    }
}
