using System.ComponentModel.DataAnnotations;
using ProductionProgress.Domain.Enums;

namespace ProductionProgress.Application.Dtos;

/// <summary>
/// 新しいタスクを登録するときに
/// クライアントから受け取るデータ。
///
/// IdやCreatedAtなどはサーバー側で決定するため、
/// リクエストには含めない。
/// </summary>
public class CreateTaskRequest
{
    /// <summary>
    /// この作業を登録する工程ID。
    ///
    /// 1以上の有効な工程IDを指定する必要がある。
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "工程を指定してください。")]
    public int ProcessId { get; set; }

    /// <summary>
    /// 作業名。
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 作業状態。
    ///
    /// 新規登録時は通常NotStartedを指定する。
    /// </summary>
    public WorkTaskStatus Status { get; set; }
        = WorkTaskStatus.NotStarted;

    /// <summary>
    /// 作業予定日。
    /// 未定の場合はnullを許可する。
    /// </summary>
    public DateOnly? PlannedDate { get; set; }
}