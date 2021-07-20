using API.Controllers;
using Application.Managers;
using Domain.Entities;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Tests.Controllers
{
    public class ValuesControllerTests
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
        public async Task Get_All()
        {
            // arrange
            var valueManagerMock = new Mock<IValueManager>();
            valueManagerMock.Setup(v => v.GetAllWithIdAbove(It.IsAny<int>())).ReturnsAsync(_valuesInMemory.Where(v => v.Id > It.IsAny<int>()).ToList());

            var sut = new ValuesController(valueManagerMock.Object);

            //act
            var test = await sut.Get();

            //assert
            Assert.DoesNotThrowAsync(async () => await sut.Get());

        }
    }
}
