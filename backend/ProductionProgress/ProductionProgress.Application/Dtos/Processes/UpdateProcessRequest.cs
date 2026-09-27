using System.ComponentModel.DataAnnotations;

namespace ProductionProgress.Application.Dtos.Processes;

/// <summary>
/// 工程を更新するときに受け取るデータ。
/// </summary>
public class UpdateProcessRequest
{
    /// <summary>
    /// 更新後の工程名。
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ProcessName { get; set; } = string.Empty;

    /// <summary>
    /// 更新後の表示順。
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// 工程を現在使用するかどうか。
    ///
    /// falseにすれば、過去データを残したまま
    /// 工程を使用停止にできる。
    /// </summary>
    public bool IsActive { get; set; }
}