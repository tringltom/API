using System.Threading.Tasks;
using API.Controllers;
using Application.ServiceInterfaces;
using AutoFixture.NUnit3;
using FixtureShared;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Models;
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
        public void RollTheDice_Successfull(
            [Frozen] Mock<IDiceService> diceServiceMock,
            DiceResult diceResult,
            [Greedy] DiceController sut)
        {
            // Arrange
            diceServiceMock.Setup(x => x.GetDiceRollResult())
               .ReturnsAsync(diceResult);

            // Act
            var res = sut.RollTheDice();

            // Assert
            res.Should().BeOfType<Task<ActionResult<DiceResult>>>();
        }
    }
}
