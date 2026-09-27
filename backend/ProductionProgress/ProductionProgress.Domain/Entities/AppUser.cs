using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionProgress.Domain.Enums;

namespace ProductionProgress.Domain.Entities;

/// <summary>
/// システムへログインするユーザーを表します。
/// </summary>
[Table("users")]
public class AppUser
{
    /// <summary>
    /// ユーザーID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// ログイン時に使用するユーザー名
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// ハッシュ化されたパスワードです。
    ///
    /// パスワードそのものは保存せず、
    /// ハッシュ化した値のみをDBへ保存します。
    /// </summary>
    [Required]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// ユーザーの権限です。
    /// </summary>
    [Required]
    [Column("role")]
    public UserRole Role { get; set; } = UserRole.Worker;

    /// <summary>
    /// ユーザーが有効かどうかを表します。
    /// false の場合はログイン不可とします。
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// ユーザー作成日時です。
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}