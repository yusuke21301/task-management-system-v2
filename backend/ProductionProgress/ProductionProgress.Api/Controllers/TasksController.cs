using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionProgress.Application.Dtos;
using ProductionProgress.Application.Dtos.Common;
using ProductionProgress.Application.Dtos.Tasks;
using ProductionProgress.Application.Services;
using ProductionProgress.Domain.Enums;

namespace ProductionProgress.Api.Controllers;

/// <summary>
/// タスク操作用のWeb API。
///
/// ControllerはHTTPリクエストを受け付ける入口だけを担当し、
/// DBアクセスや業務処理はServiceへ任せる。
/// </summary>
[ApiController]
[Route("api/tasks")]
[Authorize] // このController内のAPIはすべてログイン必須
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    /// <summary>
    /// DIによってTaskServiceを受け取る。
    /// </summary>
    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// タスク一覧を検索してページ単位で取得する。
    ///
    /// 例：
    /// GET /api/tasks?page=1&pageSize=20
    ///
    /// 検索条件との組み合わせも可能。
    ///
    /// GET /api/tasks?processId=2&page=1&pageSize=20
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetTasks(
        [FromQuery] int? processId,
        [FromQuery] WorkTaskStatus? status,
        [FromQuery] DateOnly? plannedDateFrom,
        [FromQuery] DateOnly? plannedDateTo,
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // ページ番号は1以上とする。
        if (page < 1)
        {
            return BadRequest(new
            {
                message = "pageは1以上を指定してください。"
            });
        }

        // pageSizeが大きすぎると、
        // 一度に大量データを取得できてしまうため
        // 最大100件に制限する。
        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new
            {
                message = "pageSizeは1～100の範囲で指定してください。"
            });
        }

        // From > Toは不正。
        if (plannedDateFrom.HasValue &&
            plannedDateTo.HasValue &&
            plannedDateFrom.Value > plannedDateTo.Value)
        {
            return BadRequest(new
            {
                message = "予定日の開始日は終了日以前を指定してください。"
            });
        }

        var condition = new TaskSearchCondition
        {
            ProcessId = processId,
            Status = status,
            PlannedDateFrom = plannedDateFrom,
            PlannedDateTo = plannedDateTo,
            Keyword = keyword,

            // ページ情報
            Page = page,
            PageSize = pageSize
        };

        var result =
            await _taskService.GetTasksAsync(condition);

        return Ok(result);
    }

    /// <summary>
    /// IDを指定してタスクを1件取得する。
    ///
    /// GET /api/tasks/1
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDto>> GetTaskById(int id)
    {
        // Serviceへ検索を依頼する。
        var task = await _taskService.GetTaskByIdAsync(id);

        // 対象が存在しなければHTTP 404 Not Foundを返す。
        if (task is null)
        {
            return NotFound();
        }

        // 対象が存在すればHTTP 200 OKで返す。
        return Ok(task);
    }

    /// <summary>
    /// 新しいタスクを登録する。
    ///
    /// POST /api/tasks
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask(
        CreateTaskRequest request)
    {
        try
        {
            // Serviceへ登録処理を依頼する。
            //
            // Service側では、
            // ・指定した工程が存在するか
            // ・使用停止中の工程ではないか
            // をチェックする。
            var createdTask =
                await _taskService.CreateTaskAsync(request);

            // HTTP 201 Createdを返す。
            //
            // 200 OKではなく201 Createdにすることで、
            // 「新しいリソースが作成された」ことを表す。
            return Created(
                $"/api/tasks/{createdTask.Id}",
                createdTask);
        }
        catch (InvalidOperationException ex)
        {
            // Service側の業務チェックでエラーになった場合。
            //
            // 例：
            // ・存在しない工程IDを指定した
            // ・使用停止中の工程を指定した
            //
            // サーバー内部エラー(500)ではなく、
            // リクエスト内容に問題があるため400を返す。
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// IDを指定して既存タスクを更新する。
    ///
    /// PUT /api/tasks/1
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskDto>> UpdateTask(
        int id,
        UpdateTaskRequest request)
    {
        // Serviceへ更新処理を依頼する。
        var updatedTask =
            await _taskService.UpdateTaskAsync(id, request);

        // 指定されたIDのタスクが存在しない場合は
        // HTTP 404 Not Foundを返す。
        if (updatedTask is null)
        {
            return NotFound();
        }

        // 更新後のデータをHTTP 200 OKで返す。
        return Ok(updatedTask);
    }

    /// <summary>
    /// IDを指定してタスクを削除する。
    ///
    /// DELETE /api/tasks/1
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        // Serviceへ削除処理を依頼する。
        var deleted = await _taskService.DeleteTaskAsync(id);

        // 指定されたIDのタスクが存在しない場合は
        // HTTP 404 Not Foundを返す。
        if (!deleted)
        {
            return NotFound();
        }

        // 削除に成功した場合は
        // HTTP 204 No Contentを返す。
        //
        // 削除後に返すデータがないため、
        // Response Bodyは空になる。
        return NoContent();
    }
}