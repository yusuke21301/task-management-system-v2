using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionProgress.Domain.Entities;

/// <summary>
/// 工程マスタを表すEntity。
/// 
/// 例：
/// ・成形
/// ・塗装
/// ・検査
/// ・梱包
/// 
/// WorkTaskと関連付けることで、
/// 「どの工程の作業なのか」を管理できるようにする。
/// </summary>
[Table("processes")]
public class WorkProcess
{
    /// <summary>
    /// 工程ID。
    /// DBの主キー。
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 工程名。
    /// 例：成形、塗装、検査など。
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("process_name")]
    public string ProcessName { get; set; } = string.Empty;

    /// <summary>
    /// この工程に属している作業一覧。
    ///
    /// 1つの工程に対して、
    /// 複数のWorkTaskを関連付けることができる。
    /// </summary>
    public ICollection<WorkTask> WorkTasks { get; set; }
        = new List<WorkTask>();

    /// <summary>
    /// 画面などで工程を表示するときの並び順。
    /// 
    /// 例：
    /// 10 = 成形
    /// 20 = 塗装
    /// 30 = 検査
    /// </summary>
    [Column("display_order")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// 使用中の工程かどうか。
    /// 
    /// true  : 使用中
    /// false : 使用停止
    /// 
    /// 工程を完全削除せず、
    /// 過去データを残したまま使用停止できるようにする。
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 登録日時。
    /// </summary>
    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新日時。
    /// </summary>
    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; }
}