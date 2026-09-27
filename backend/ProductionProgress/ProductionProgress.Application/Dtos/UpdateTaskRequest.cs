using System.ComponentModel.DataAnnotations;
using ProductionProgress.Domain.Enums;

namespace ProductionProgress.Application.Dtos;

/// <summary>
/// 既存タスクを更新するときに
/// クライアントから受け取るデータ。
///
/// IdはURLから受け取るため、このDTOには含めない。
/// CreatedAt、UpdatedAtもサーバー側で管理する。
/// </summary>
public class UpdateTaskRequest
{
    /// <summary>
    /// 更新後の工程ID。
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
    /// 作業の進捗状態。
    /// </summary>
    public WorkTaskStatus Status { get; set; }

    /// <summary>
    /// 作業予定日。
    /// 未定の場合はnullを許可する。
    /// </summary>
    public DateOnly? PlannedDate { get; set; }
}