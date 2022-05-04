using AutoFixture;
using Domain;
using FixtureShared;
using FluentAssertions;
using Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Security
{
    public class JwtGeneratorTests
    {
        private IConfiguration _config;
        private IFixture _fixture;
        private readonly string _tokenKey = "EkvitiDevSuperSecretKey";

        [SetUp]
        public void SetUp()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey").Value).Returns(_tokenKey);
            _config = mockConfig.Object;
            _fixture = new FixtureDirector().WithOmitRecursion();
        }

        [Test]
        public void CreateToken()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var sut = new TokenManager(_config);

            // Act
            var result = sut.CreateJWTToken(user.Id, user.UserName);

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void GetRefreshToken()
        {
            // Arrange
            var sut = new TokenManager(_config);

            // Act
            var tokenResult = sut.CreateRefreshToken();

            // Assert
            tokenResult.Should().NotBeNull();
        }
    }
}
