using ProductionProgress.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionProgress.Domain.Entities;

/// <summary>
/// 工程で管理する作業を表すエンティティ。
///
/// Table属性・Column属性を使用して、
/// PostgreSQLのテーブル名・列名との対応を定義する。
/// </summary>
[Table("work_tasks")]
public class WorkTask
{
    /// <summary>
    /// 作業を一意に識別するID。
    /// PostgreSQLではid列に対応する。
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 作業名。
    /// 最大200文字、必須項目とする。
    /// </summary>
    [Required]
    [MaxLength(200)]
    [Column("task_name")]
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 作業の進捗状態。
    ///
    /// C#ではWorkTaskStatusとして扱うが、
    /// PostgreSQLには整数値として保存される。
    /// </summary>
    [Column("status")]
    public WorkTaskStatus Status { get; set; }

    /// <summary>
    /// 作業予定日。
    /// nullの場合は予定日未設定を表す。
    /// </summary>
    [Column("planned_date")]
    public DateOnly? PlannedDate { get; set; }

    /// <summary>
    /// この作業が属する工程のID。
    ///
    /// Taskは必ず1つの工程に所属するため、
    /// NULLは許可しない。
    /// </summary>
    [Column("process_id")]
    public int ProcessId { get; set; }

    /// <summary>
    /// この作業が属する工程。
    ///
    /// ProcessIdを外部キーとして、
    /// processesテーブルのWorkProcessと関連付ける。
    /// </summary>
    public WorkProcess? Process { get; set; }

    /// <summary>
    /// データ作成日時。
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最終更新日時。
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}