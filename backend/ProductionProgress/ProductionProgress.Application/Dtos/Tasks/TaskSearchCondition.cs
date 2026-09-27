using ProductionProgress.Domain.Entities;
using ProductionProgress.Domain.Enums;

namespace ProductionProgress.Application.Dtos.Tasks;

/// <summary>
/// タスク一覧を検索するときの条件。
///
/// 各条件は任意指定とし、
/// 値が指定された条件だけ検索に使用する。
/// </summary>
public class TaskSearchCondition
{
    /// <summary>
    /// 工程ID。
    ///
    /// nullの場合は工程で絞り込まない。
    /// </summary>
    public int? ProcessId { get; set; }

    /// <summary>
    /// タスクの状態。
    ///
    /// nullの場合は状態で絞り込まない。
    /// </summary>
    public WorkTaskStatus? Status { get; set; }

    /// <summary>
    /// 予定日の検索開始日。
    ///
    /// nullの場合は開始日の条件を指定しない。
    /// </summary>
    public DateOnly? PlannedDateFrom { get; set; }

    /// <summary>
    /// 予定日の検索終了日。
    ///
    /// nullの場合は終了日の条件を指定しない。
    /// </summary>
    public DateOnly? PlannedDateTo { get; set; }

    /// <summary>
    /// タスク名の検索文字列。
    ///
    /// nullまたは空文字の場合は、
    /// タスク名による絞り込みを行わない。
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 取得するページ番号。
    /// 1ページ目から開始する。
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 1ページあたりの取得件数。
    /// 初期値は20件。
    /// </summary>
    public int PageSize { get; set; } = 20;
}