using System.ComponentModel.DataAnnotations;

namespace ProductionProgress.Application.Dtos.Processes;

/// <summary>
/// 工程を新規登録するときに受け取るデータ。
/// </summary>
public class CreateProcessRequest
{
    /// <summary>
    /// 工程名。
    ///
    /// Required:
    /// 空文字や未入力を禁止する。
    ///
    /// MaxLength:
    /// DBのvarchar(100)と合わせて最大100文字にする。
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ProcessName { get; set; } = string.Empty;

    /// <summary>
    /// 表示順。
    /// 数字が小さい工程から先に表示する。
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// 使用中かどうか。
    /// 新規登録時はtrueを初期値とする。
    /// </summary>
    public bool IsActive { get; set; } = true;
}