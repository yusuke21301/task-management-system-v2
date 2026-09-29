using System.Net;
using System.Net.Http.Json;
using ProductionProgress.IntegrationTests.Fixtures;

namespace ProductionProgress.IntegrationTests.Api;

/// <summary>
/// TestcontainersのPostgreSQLを
/// ProductionProgress.Apiから利用できることを確認する。
/// </summary>
public class TestcontainersApiTests
    : IClassFixture<ProductionProgressApiFixture>
{
    private readonly ProductionProgressApiFixture _fixture;

    public TestcontainersApiTests(
        ProductionProgressApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Login_WithTestDatabase_ReturnsOk()
    {
        // =========================================
        // Arrange
        // =========================================

        using var client =
            _fixture.CreateClient();

        // Fixtureで設定した
        // テスト専用Adminのログイン情報。
        var request = new
        {
            username = "testadmin",
            password = "TestPassword123!"
        };

        // =========================================
        // Act
        // =========================================

        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/login",
                request);

        // =========================================
        // Assert
        // =========================================

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }
}