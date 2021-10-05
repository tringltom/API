using Application.Security;
using AutoFixture;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Security
{
    public class JwtGeneratorTests
    {
        private IConfiguration _config;
        private Fixture _fixture;
        private readonly string _tokenKey = "EkvitiDevSuperSecretKey";
        [SetUp]
        public void SetUp()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey").Value).Returns(_tokenKey);
            _config = mockConfig.Object;
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Test]
        public void CreateToken()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var sut = new JwtGenerator(_config);

            // Act
            var result = sut.CreateToken(user);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetRefreshToken()
        {
            // Arrange
            var sut = new JwtGenerator(_config);

            // Act
            var tokenResult = sut.GetRefreshToken();

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(tokenResult.Token));
        }
    }
}
