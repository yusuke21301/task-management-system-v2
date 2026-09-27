namespace ProductionProgress.Application.Dtos.Processes;

/// <summary>
/// 工程情報を画面やAPIへ返すためのDTO。
///
/// Entityをそのまま外部へ公開せず、
/// 必要な項目だけをDTOとして返す。
/// </summary>
public class ProcessDto
{
    /// <summary>
    /// 工程ID。
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 工程名。
    /// </summary>
    public string ProcessName { get; set; } = string.Empty;

    /// <summary>
    /// 画面上での表示順。
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// 使用中の工程かどうか。
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 登録日時。
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新日時。
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}