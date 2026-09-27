using ProductionProgress.Application.Dtos;
using ProductionProgress.Application.Dtos.Common;
using ProductionProgress.Application.Dtos.Tasks;

namespace ProductionProgress.Application.Services;

/// <summary>
/// タスクに対して行える業務処理を定義する。
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// 指定された検索条件でタスク一覧を取得する。
    /// </summary>
    Task<PagedResult<TaskDto>> GetTasksAsync(TaskSearchCondition condition);

    /// <summary>
    /// IDを指定してタスクを1件取得する。
    ///
    /// 存在しない場合はnullを返す。
    /// </summary>
    Task<TaskDto?> GetTaskByIdAsync(int id);

    /// <summary>
    /// 新しいタスクを登録する。
    /// </summary>
    Task<TaskDto> CreateTaskAsync(CreateTaskRequest request);

    /// <summary>
    /// IDを指定してタスクを更新する。
    ///
    /// 対象が存在しない場合はnullを返す。
    /// </summary>
    Task<TaskDto?> UpdateTaskAsync(
        int id,
        UpdateTaskRequest request);

    /// <summary>
    /// IDを指定してタスクを削除する。
    ///
    /// 削除できた場合はtrue、
    /// 対象が存在しない場合はfalseを返す。
    /// </summary>
    Task<bool> DeleteTaskAsync(int id);
}