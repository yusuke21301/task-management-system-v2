using System.ComponentModel.DataAnnotations;

namespace ProductionProgress.Application.Dtos.Auth;

/// <summary>
/// ログイン時にクライアントから受け取る情報です。
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// ユーザー名
    /// </summary>
    [Required(ErrorMessage = "ユーザー名は必須です。")]
    [MaxLength(
        50,
        ErrorMessage = "ユーザー名は50文字以下で入力してください。")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// パスワード
    /// </summary>
    [Required(ErrorMessage = "パスワードは必須です。")]
    [MaxLength(
        128,
        ErrorMessage = "パスワードは128文字以下で入力してください。")]
    public string Password { get; set; } = string.Empty;
}