using ProductionProgress.Domain.Entities;

namespace ProductionProgress.Application.Interfaces;

/// <summary>
/// 工程マスタを操作するRepositoryのインターフェース。
///
/// Application層では、
/// 「どのDBを使うか」「Entity Framework Coreをどう使うか」
/// といった具体的なDBアクセス方法を意識しない。
///
/// 実際のDBアクセス処理はInfrastructure層で実装する。
/// </summary>
public interface IProcessRepository
{
    /// <summary>
    /// 工程をすべて取得する。
    /// </summary>
    Task<IReadOnlyList<WorkProcess>> GetAllAsync();

    /// <summary>
    /// 指定したIDの工程を取得する。
    /// 存在しない場合はnullを返す。
    /// </summary>
    Task<WorkProcess?> GetByIdAsync(int id);

    /// <summary>
    /// 指定した工程名の工程を取得する。
    ///
    /// 工程名の重複チェックなどで使用する。
    /// </summary>
    Task<WorkProcess?> GetByNameAsync(string processName);

    /// <summary>
    /// 新しい工程を追加する。
    ///
    /// この時点ではまだDBへの保存は確定しない。
    /// SaveChangesAsync()で確定する。
    /// </summary>
    Task AddAsync(WorkProcess process);

    /// <summary>
    /// DbContextで追跡している変更内容をDBへ保存する。
    /// </summary>
    Task SaveChangesAsync();
}