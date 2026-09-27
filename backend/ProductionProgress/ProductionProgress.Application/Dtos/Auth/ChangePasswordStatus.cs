namespace ProductionProgress.Application.Dtos.Auth;

/// <summary>
/// パスワード変更処理の結果を表します。
/// </summary>
public enum ChangePasswordStatus
{
    /// <summary>
    /// 正常に変更できました。
    /// </summary>
    Success,

    /// <summary>
    /// 対象ユーザーが存在しません。
    /// </summary>
    UserNotFound,

    /// <summary>
    /// 現在のパスワードが正しくありません。
    /// </summary>
    CurrentPasswordIncorrect,

    /// <summary>
    /// 現在と同じパスワードが指定されています。
    /// </summary>
    SamePassword
}