namespace ProductionProgress.Domain.Enums;

/// <summary>
/// システム利用者の権限を表します。
/// </summary>
public enum UserRole
{
    /// <summary>
    /// 一般作業者
    /// </summary>
    Worker = 0,

    /// <summary>
    /// 管理者
    /// </summary>
    Admin = 1
}