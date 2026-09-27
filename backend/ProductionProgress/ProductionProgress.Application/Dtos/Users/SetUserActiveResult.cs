namespace ProductionProgress.Application.Dtos.Users;
/// <summary>
/// ユーザー有効状態の変更結果です。
/// </summary>
public class SetUserActiveResult
{
    /// <summary>
    /// 処理結果
    /// </summary>
    public SetUserActiveStatus Status { get; set; }

    /// <summary>
    /// 変更後のユーザー情報
    ///
    /// 成功時のみ値が入ります。
    /// </summary>
    public UserResponse? User { get; set; }
}