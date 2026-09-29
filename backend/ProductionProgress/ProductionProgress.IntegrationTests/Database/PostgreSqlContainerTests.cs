using Testcontainers.PostgreSql;

namespace ProductionProgress.IntegrationTests.Database;

/// <summary>
/// Testcontainersを使用して、
/// テスト専用のPostgreSQLコンテナを起動できることを確認する。
/// </summary>
public class PostgreSqlContainerTests
{
    [Fact]
    public async Task PostgreSqlContainer_CanStartAndExecuteSql()
    {
        // =========================================
        // Arrange
        // PostgreSQLコンテナの設定を作成する
        // =========================================

        await using var postgreSqlContainer =
            new PostgreSqlBuilder("postgres:16")
                .WithDatabase("production_progress_test")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

        // =========================================
        // Act
        // Docker上にPostgreSQLコンテナを起動する
        // =========================================

        await postgreSqlContainer.StartAsync();

        // PostgreSQLコンテナ内でSQLを実行する。
        var result =
            await postgreSqlContainer.ExecScriptAsync(
                "SELECT 1;");

        // =========================================
        // Assert
        // SQLが正常終了したことを確認する
        // =========================================

        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.Stderr);
    }
}