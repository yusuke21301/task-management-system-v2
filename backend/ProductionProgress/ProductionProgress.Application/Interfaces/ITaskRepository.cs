using ProductionProgress.Application.Dtos.Common;
using ProductionProgress.Application.Dtos.Tasks;
using ProductionProgress.Domain.Entities;

namespace ProductionProgress.Application.Interfaces;

/// <summary>
/// タスクのデータアクセス処理を定義する。
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// 指定された検索条件に一致するタスク一覧を取得する。
    /// </summary>
    Task<PagedResult<WorkTask>> GetAllAsync(TaskSearchCondition condition);

    /// <summary>
    /// IDを指定してタスクを1件取得する。
    ///
    /// 対象が存在しない場合はnullを返す。
    /// </summary>
    Task<WorkTask?> GetByIdAsync(int id);

    /// <summary>
    /// 新しいタスクをDBへ登録する。
    /// </summary>
    Task<WorkTask> AddAsync(WorkTask task);

    /// <summary>
    /// タスクを更新する。
    /// </summary>
    Task<WorkTask> UpdateAsync(WorkTask task);

    /// <summary>
    /// タスクを削除する。
    /// </summary>
    Task DeleteAsync(WorkTask task);
}