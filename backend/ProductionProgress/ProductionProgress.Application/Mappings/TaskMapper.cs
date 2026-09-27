using ProductionProgress.Application.Dtos;
using ProductionProgress.Domain.Entities;

namespace ProductionProgress.Application.Mappings;

/// <summary>
/// WorkTask(Entity)とTaskDtoの変換処理をまとめるクラス。
///
/// Serviceごとに同じ変換コードを書くと重複が増えるため、
/// Mapping処理を1か所にまとめる。
/// </summary>
public static class TaskMapper
{
    /// <summary>
    /// WorkTask EntityをAPI返却用のTaskDtoへ変換する。
    /// </summary>
    /// <param name="task">変換元のEntity</param>
    /// <returns>API返却用DTO</returns>
    public static TaskDto ToDto(WorkTask task)
    {
        return new TaskDto
        {
            Id = task.Id,
            TaskName = task.TaskName,
            ProcessId = task.ProcessId,
            ProcessName = task.Process?.ProcessName,
            Status = task.Status,
            PlannedDate = task.PlannedDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}