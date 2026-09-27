using System.ComponentModel.DataAnnotations;

namespace ProductionProgress.Application.Dtos.Users;

/// <summary>
/// Workerユーザーを新規登録するときの入力情報です。
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// ログイン時に使用するユーザー名です。
    /// </summary>
    [Required(ErrorMessage = "ユーザー名は必須です。")]
    [StringLength(
        50,
        MinimumLength = 3,
        ErrorMessage = "ユーザー名は3文字以上50文字以下で入力してください。")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// ログイン時に使用するパスワードです。
    ///
    /// DBにはこの値をそのまま保存せず、
    /// ハッシュ化した値を保存します。
    /// </summary>
    [Required(ErrorMessage = "パスワードは必須です。")]
    [StringLength(
        128,
        MinimumLength = 4,
        ErrorMessage = "パスワードは4文字以上128文字以下で入力してください。")]
    public string Password { get; set; } = string.Empty;
}