using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Services;
using AutoFixture;
using AutoFixture.NUnit3;
using AutoMapper;
using Domain.Entities;
using FixtureShared;
using FluentAssertions;
using Models.User;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class UsersServiceTests
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithAutoMoqAndOmitRecursion();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetTopXpUsers_Successfull([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IMapper> mapperMock, int? limit,
            int? offset,
            List<UserArenaGet> userArenaGet,
            List<User> users,int usersCount,
            UsersService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.GetTopXpUsersAsync(limit, offset))
                .ReturnsAsync(users);

            userRepoMock.Setup(x => x.GetUserCountAsync())
                .ReturnsAsync(usersCount);

            mapperMock.Setup(x => x.Map<List<UserArenaGet>>(It.IsAny<User>())).Returns(userArenaGet);

            var userArenaEnvelope = _fixture.Create<UserArenaEnvelope>();
            userArenaEnvelope.Users = userArenaGet;
            userArenaEnvelope.UserCount = usersCount;

            // Act
            Func<Task<UserArenaEnvelope>> methodInTest = async () => await sut.GetTopXpUsers(limit, offset);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            methodInTest.Should().NotBeNull();
        }
    }
}
