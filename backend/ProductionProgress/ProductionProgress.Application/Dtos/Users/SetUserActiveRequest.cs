namespace ProductionProgress.Application.Dtos.Users;

/// <summary>
/// ユーザーの有効・無効を切り替えるための入力情報です。
/// </summary>
public class SetUserActiveRequest
{
    /// <summary>
    /// true  : 有効
    /// false : 無効
    /// </summary>
    public bool IsActive { get; set; }
}