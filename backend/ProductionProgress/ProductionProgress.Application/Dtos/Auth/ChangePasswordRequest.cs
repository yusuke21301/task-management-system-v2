using System.ComponentModel.DataAnnotations;

namespace ProductionProgress.Application.Dtos.Auth;

/// <summary>
/// パスワード変更時に受け取る情報です。
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// 現在使用しているパスワード
    /// </summary>
    [Required(ErrorMessage = "現在のパスワードは必須です。")]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新しいパスワード
    /// </summary>
    [Required(ErrorMessage = "新しいパスワードは必須です。")]
    [StringLength(
        128,
        MinimumLength = 4,
        ErrorMessage = "新しいパスワードは4文字以上128文字以下で入力してください。")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新しいパスワードの確認入力
    /// </summary>
    [Required(ErrorMessage = "確認用パスワードは必須です。")]
    [Compare(
        nameof(NewPassword),
        ErrorMessage = "新しいパスワードと確認用パスワードが一致しません。")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}