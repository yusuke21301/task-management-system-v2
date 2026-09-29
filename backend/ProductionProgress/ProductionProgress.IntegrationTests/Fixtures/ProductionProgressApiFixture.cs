using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductionProgress.Infrastructure.Data;
using Testcontainers.PostgreSql;

namespace ProductionProgress.IntegrationTests.Fixtures;

/// <summary>
/// Integration Test用のテスト環境。
///
/// ・PostgreSQLコンテナを起動
/// ・Migrationを適用
/// ・ProductionProgress.ApiのDB接続先を
///   TestcontainersのPostgreSQLへ差し替える
/// </summary>
public class ProductionProgressApiFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;

    private WebApplicationFactory<Program>? _factory;

    public ProductionProgressApiFixture()
    {
        // テスト専用PostgreSQLコンテナを定義する。
        _postgresContainer =
            new PostgreSqlBuilder("postgres:16")
                .WithDatabase("production_progress_test")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();
    }

    /// <summary>
    /// テスト開始前に一度だけ実行される。
    /// </summary>
    public async Task InitializeAsync()
    {
        // =========================================
        // 1. PostgreSQLコンテナを起動
        // =========================================

        await _postgresContainer.StartAsync();

        var connectionString =
            _postgresContainer.GetConnectionString();

        // =========================================
        // 2. テストDBへMigrationを適用
        // =========================================

        // APIを起動する前にDBを作っておく。
        // Program.csでは初期Admin登録も行うため、
        // usersテーブルなどが先に必要になる。
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

        await using (var dbContext =
            new AppDbContext(options))
        {
            await dbContext.Database.MigrateAsync();
        }

        // =========================================
        // 3. APIの設定をテスト用に差し替える
        // =========================================

        _factory =
            new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    // テスト用の設定値を追加する。
                    builder.ConfigureAppConfiguration(
                        (_, configuration) =>
                        {
                            var settings =
                                new Dictionary<string, string?>
                                {
                                    // テスト専用Admin
                                    ["InitialAdmin:Username"] =
                                        "testadmin",

                                    ["InitialAdmin:Password"] =
                                        "TestPassword123!",

                                    // JWTもテスト用に固定する
                                    ["Jwt:Key"] =
                                        "ProductionProgress-Test-Jwt-Key-2026-0123456789",

                                    ["Jwt:Issuer"] =
                                        "ProductionProgress.Api",

                                    ["Jwt:Audience"] =
                                        "ProductionProgress.Client",

                                    ["Jwt:ExpirationMinutes"] =
                                        "60"
                                };

                            configuration.AddInMemoryCollection(
                                settings);
                        });

                    builder.ConfigureServices(services =>
                    {
                        // Program.csで登録されている
                        // AppDbContextの設定を削除する。
                        services.RemoveAll<
                            DbContextOptions<AppDbContext>>();

                        services.RemoveAll<AppDbContext>();

                        // Testcontainersで起動した
                        // PostgreSQLへ接続するように差し替える。
                        services.AddDbContext<AppDbContext>(
                            options =>
                            {
                                options.UseNpgsql(
                                    connectionString);
                            });
                    });
                });
    }

    /// <summary>
    /// APIへHTTPリクエストするための
    /// HttpClientを作成する。
    /// </summary>
    public HttpClient CreateClient()
    {
        if (_factory is null)
        {
            throw new InvalidOperationException(
                "テスト環境が初期化されていません。");
        }

        return _factory.CreateClient();
    }

    /// <summary>
    /// テスト終了後に実行される。
    /// </summary>
    public async Task DisposeAsync()
    {
        // ASP.NET Coreのテストサーバーを破棄
        _factory?.Dispose();

        // PostgreSQLコンテナを破棄
        await _postgresContainer.DisposeAsync();
    }
}