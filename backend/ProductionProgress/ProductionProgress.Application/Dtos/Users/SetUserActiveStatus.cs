namespace ProductionProgress.Application.Dtos.Users;

/// <summary>
/// ユーザーの有効・無効変更結果を表します。
/// </summary>
public enum SetUserActiveStatus
{
    /// <summary>
    /// 正常に変更できました。
    /// </summary>
    Success,

    /// <summary>
    /// 指定されたユーザーが存在しません。
    /// </summary>
    NotFound,

    /// <summary>
    /// Adminユーザーの変更は禁止されています。
    /// </summary>
    AdminNotAllowed
}