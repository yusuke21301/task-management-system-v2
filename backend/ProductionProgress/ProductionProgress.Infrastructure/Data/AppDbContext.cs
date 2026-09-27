using Microsoft.EntityFrameworkCore;
using ProductionProgress.Domain.Entities;

namespace ProductionProgress.Infrastructure.Data;

/// <summary>
/// PostgreSQLとの接続を管理するDbContext。
///
/// Entity Framework Coreを使用して、
/// C#のEntityとPostgreSQLのテーブルを橋渡しする。
///
/// テーブル名や列名などのマッピングは、
/// WorkTask側の [Table] / [Column] 属性で定義しているため、
/// AppDbContextでは基本的にDbSetの定義だけを行う。
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Program.csで設定したDbContextOptionsを受け取る。
    ///
    /// DbContextOptionsには、
    /// PostgreSQLを使用する設定や接続文字列などが含まれる。
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// PostgreSQLのwork_tasksテーブルを操作するためのDbSet。
    ///
    /// WorkTaskクラスには、
    /// [Table("work_tasks")]
    /// が付いているため、
    /// このDbSetからwork_tasksテーブルへアクセスできる。
    /// </summary>
    public DbSet<WorkTask> WorkTasks => Set<WorkTask>();

    /// <summary>
    /// ログインユーザー
    /// </summary>
    public DbSet<AppUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // UserRole enum を数値ではなく文字列でDBへ保存する。
        // 例:
        // Worker = 0 → "Worker"
        // Admin  = 1 → "Admin"
        modelBuilder.Entity<AppUser>()
            .Property(x => x.Role)
            .HasConversion<string>();

        // 同じユーザー名を複数登録できないようにする。
        modelBuilder.Entity<AppUser>()
            .HasIndex(x => x.Username)
            .IsUnique();

        // 工程名が重複しないように一意インデックスを設定する。
        // 例：「塗装」という工程を2件登録できないようにする。
        modelBuilder.Entity<WorkProcess>()
            .HasIndex(x => x.ProcessName)
            .IsUnique();

        // WorkTaskとWorkProcessのリレーションを設定する。
        //
        // WorkTask側から見ると、1つのTaskは1つの工程に属する。
        // WorkProcess側から見ると、1つの工程は複数のTaskを持つ。
        modelBuilder.Entity<WorkTask>()
            .HasOne(x => x.Process)
            .WithMany(x => x.WorkTasks)
            .HasForeignKey(x => x.ProcessId)
            .OnDelete(DeleteBehavior.Restrict);

    }

    /// <summary>
    /// 工程マスタ
    /// </summary>
    public DbSet<WorkProcess> Processes => Set<WorkProcess>();
}