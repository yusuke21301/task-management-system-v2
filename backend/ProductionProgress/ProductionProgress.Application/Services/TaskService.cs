using ProductionProgress.Application.Dtos;
using ProductionProgress.Application.Dtos.Common;
using ProductionProgress.Application.Dtos.Tasks;
using ProductionProgress.Application.Interfaces;
using ProductionProgress.Application.Mappings;
using ProductionProgress.Domain.Entities;
using System.Diagnostics;

namespace ProductionProgress.Application.Services;

/// <summary>
/// タスクに関する業務処理を担当するService。
///
/// Controllerから直接Repositoryを呼ばず、
/// Serviceを経由することで、
/// 業務ロジックをControllerから分離する。
/// </summary>
public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProcessRepository _processRepository;

    /// <summary>
    /// DIによってRepositoryを受け取る。
    /// </summary>
    public TaskService(
        ITaskRepository taskRepository,
        IProcessRepository processRepository)
    {
        _taskRepository = taskRepository;
        _processRepository = processRepository;
    }

    /// <summary>
    /// 指定された検索条件でタスク一覧を取得する。
    /// </summary>
    public async Task<PagedResult<TaskDto>> GetTasksAsync(
    TaskSearchCondition condition)
    {
        // Repositoryからページング済みのEntityを取得する。
        var result =
            await _taskRepository.GetAllAsync(condition);

        // WorkTaskをTaskDtoへ変換する。
        var items = result.Items
            .Select(TaskMapper.ToDto)
            .ToList();

        // ページ情報を維持したまま、
        // API返却用のTaskDtoへ詰め替える。
        return new PagedResult<TaskDto>
        {
            Items = items,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    /// <summary>
    /// IDを指定してタスクを1件取得する。
    /// </summary>
    public async Task<TaskDto?> GetTaskByIdAsync(int id)
    {
        // RepositoryへDB検索を依頼する。
        var task = await _taskRepository.GetByIdAsync(id);

        // 対象のタスクが存在しない場合はnullを返す。
        if (task is null)
        {
            return null;
        }

        // EntityをAPI公開用DTOへ変換する。
        return TaskMapper.ToDto(task);
    }

    /// <summary>
    /// 新しいタスクを登録する。
    ///
    /// Request DTOからDomain Entityを生成し、
    /// Repositoryへ登録を依頼する。
    /// </summary>
    public async Task<TaskDto> CreateTaskAsync(
        CreateTaskRequest request)
    {
        // 指定された工程が実際に存在するか確認する。
        var process =
            await _processRepository.GetByIdAsync(request.ProcessId);

        if (process is null)
        {
            throw new InvalidOperationException(
                $"工程ID {request.ProcessId} は存在しません。");
        }

        // 使用停止中の工程には、
        // 新しいTaskを登録できないようにする。
        if (!process.IsActive)
        {
            throw new InvalidOperationException(
                $"工程「{process.ProcessName}」は現在使用停止中です。");
        }

        // 登録日時と更新日時は
        // クライアントではなくサーバー側で決定する。
        var now = DateTime.UtcNow;

        // APIから受け取ったRequestを
        // DB保存用のEntityへ変換する。
        var task = new WorkTask
        {
            TaskName = request.TaskName,
            ProcessId = request.ProcessId,
            // すでに取得済みの工程Entityも設定する。
            Process = process,
            Status = request.Status,
            PlannedDate = request.PlannedDate,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Repositoryを使用してDBへ登録する。
        var createdTask =
            await _taskRepository.AddAsync(task);

        // DB EntityをそのままAPIへ返さず、
        // TaskDtoへ変換して返す。
        return TaskMapper.ToDto(createdTask);
    }

    /// <summary>
    /// IDを指定して既存タスクを更新する。
    /// </summary>
    public async Task<TaskDto?> UpdateTaskAsync(
        int id,
        UpdateTaskRequest request)
    {
        // まず更新対象のタスクが存在するか確認する。
        var task = await _taskRepository.GetByIdAsync(id);

        // 対象が存在しない場合、
        // Controller側で404を返せるようnullを返す。
        if (task is null)
        {
            return null;
        }

        // 指定された工程を取得する。
        var process =
            await _processRepository.GetByIdAsync(request.ProcessId);

        if (process is null)
        {
            throw new InvalidOperationException(
                $"工程ID {request.ProcessId} は存在しません。");
        }

        // 現在とは別の工程へ変更する場合のみ、
        // 変更先工程が使用中か確認する。
        if (task.ProcessId != request.ProcessId &&
            !process.IsActive)
        {
            throw new InvalidOperationException(
                $"工程「{process.ProcessName}」は現在使用停止中です。");
        }

        // クライアントから受け取った値で
        // Entityの内容を更新する。
        task.TaskName = request.TaskName;
        task.ProcessId = request.ProcessId;
        task.Process = process;
        task.Status = request.Status;
        task.PlannedDate = request.PlannedDate;

        // 更新日時はクライアントから受け取らず、
        // サーバー側で現在日時を設定する。
        task.UpdatedAt = DateTime.UtcNow;

        // RepositoryへDB更新を依頼する。
        var updatedTask =
            await _taskRepository.UpdateAsync(task);

        // DB EntityをAPIへ直接返さず、
        // TaskDtoへ変換して返す。
        return TaskMapper.ToDto(updatedTask);
    }

    /// <summary>
    /// IDを指定して既存タスクを削除する。
    /// </summary>
    public async Task<bool> DeleteTaskAsync(int id)
    {
        // まず削除対象のタスクが存在するか確認する。
        var task = await _taskRepository.GetByIdAsync(id);

        // 対象が存在しない場合は削除できないためfalseを返す。
        if (task is null)
        {
            return false;
        }

        // Repositoryへ削除処理を依頼する。
        await _taskRepository.DeleteAsync(task);

        // 正常に削除できたことをControllerへ伝える。
        return true;
    }
}