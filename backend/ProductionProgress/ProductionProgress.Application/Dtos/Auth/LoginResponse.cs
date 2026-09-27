namespace ProductionProgress.Application.Dtos.Auth;

/// <summary>
/// ログイン成功時に返す情報です。
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// ユーザーID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// ユーザー名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// ユーザー権限
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// API認証に使用するJWTアクセストークン
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// アクセストークンの有効期限
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}