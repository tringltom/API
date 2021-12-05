using Application.Security;
using Microsoft.Extensions.Configuration;

namespace Application.Tests.Security;

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
