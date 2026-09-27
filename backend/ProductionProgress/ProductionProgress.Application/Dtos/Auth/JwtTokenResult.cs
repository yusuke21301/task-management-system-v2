namespace ProductionProgress.Application.Dtos.Auth;

/// <summary>
/// JWT生成結果を表します。
/// </summary>
public class JwtTokenResult
{
    /// <summary>
    /// 発行されたアクセストークンです。
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// アクセストークンの有効期限です。
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}