using ProductionProgress.Domain.Enums;

namespace ProductionProgress.Application.Dtos;

/// <summary>
/// APIからタスク情報を返すためのDTO。
///
/// DBのEntityをそのまま外部に公開せず、
/// APIで必要なデータだけを返すために使用する。
/// </summary>
public class TaskDto
{
    public int Id { get; set; }

    public string TaskName { get; set; } = string.Empty;

    public WorkTaskStatus Status { get; set; }

    public DateOnly? PlannedDate { get; set; }

    /// <summary>
    /// 作業が属する工程ID。
    /// </summary>
    public int ProcessId { get; set; }

    /// <summary>
    /// 作業が属する工程名。
    ///
    /// ProcessIdが未設定の場合はNULLになる。
    /// </summary>
    public string? ProcessName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}