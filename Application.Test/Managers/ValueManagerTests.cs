using Application.Managers;
using Application.Repositories;
using Domain.Entities;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Tests.Managers
{
    public class ValueManagerTests
    {

        private List<Value> _valuesInMemory;

        [SetUp]
        public void SetUp()
        {
            _valuesInMemory = new List<Value>
            {
                new Value() {Id = 1, Name = "Value1"},
                new Value() {Id = 2, Name = "Value2"},
                new Value() {Id = 3, Name = "Value3"},
                new Value() {Id = 4, Name = "Value4"},
                new Value() {Id = 5, Name = "Value5"}
            };
        }

        [Test]
        [TestCase(1, 4)]
        [TestCase(3, 2)]
        [TestCase(5, 0)]
        public async Task GetAllWithIdAbove_IdGreaterThanZero(int id, int expectedReturn)
        {
            //arrange
            var valueRepo = new Mock<IValueRepository>();

            valueRepo.Setup(x => x.GetAllValues())
                .ReturnsAsync(_valuesInMemory);

            var sut = new ValueManager(valueRepo.Object);

            //act
            var result = await sut.GetAllWithIdAbove(id);

            //assert
            Assert.AreEqual(result.Count, expectedReturn);
        }

        [Test]
        [TestCase(-1, 5)]
        [TestCase(-3, 5)]
        [TestCase(-5, 5)]
        public async Task GetAllWithIdAbove_IdLessThanZero(int id, int expectedReturn)
        {
            //arrange
            var valueRepo = new Mock<IValueRepository>();

            valueRepo.Setup(x => x.GetAllValues())
                .ReturnsAsync(_valuesInMemory);

            var sut = new ValueManager(valueRepo.Object);

            //act
            var result = await sut.GetAllWithIdAbove(id);

            //assert
            Assert.AreEqual(result.Count, expectedReturn);
        }

        [Test]
        [TestCase(0, 5)]
        public async Task GetAllWithIdAbove_IdEqualToZero(int id, int expectedReturn)
        {
            //arrange
            var valueRepo = new Mock<IValueRepository>();

            valueRepo.Setup(x => x.GetAllValues())
                .ReturnsAsync(_valuesInMemory);

            var sut = new ValueManager(valueRepo.Object);

            //act
            var result = await sut.GetAllWithIdAbove(id);

            //assert
            Assert.AreEqual(result.Count, expectedReturn);
        }
    }
}
