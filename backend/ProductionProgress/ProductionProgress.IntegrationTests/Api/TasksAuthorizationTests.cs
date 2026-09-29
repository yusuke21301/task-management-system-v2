using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ProductionProgress.IntegrationTests.Api;

/// <summary>
/// Tasks APIの認証に関する統合テスト。
///
/// Moqは使用せず、
/// WebApplicationFactoryで実際のASP.NET Coreアプリを起動して
/// HTTPリクエストを送信する。
/// </summary>
public class TasksAuthorizationTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    /// <summary>
    /// xUnitからWebApplicationFactoryを受け取り、
    /// テスト用HttpClientを作成する。
    /// </summary>
    public TasksAuthorizationTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// ログインしていない状態でTasks APIへアクセスすると、
    /// 401 Unauthorizedになることを確認する。
    /// </summary>
    [Fact]
    public async Task GetTasks_WithoutAuthentication_ReturnsUnauthorized()
    {
        // =========================================
        // Act
        // 認証情報を付けずにAPIへアクセスする
        // =========================================

        var response =
            await _client.GetAsync("/api/Tasks");

        // =========================================
        // Assert
        // =========================================

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}