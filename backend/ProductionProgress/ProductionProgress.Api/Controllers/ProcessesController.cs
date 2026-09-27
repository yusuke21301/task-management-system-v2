using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionProgress.Application.Dtos.Processes;
using ProductionProgress.Application.Interfaces;

namespace ProductionProgress.Api.Controllers;

/// <summary>
/// 工程マスタを操作するAPI。
///
/// 工程の参照はログイン済みユーザーが利用でき、
/// 登録・更新はAdminのみ利用できる。
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProcessesController : ControllerBase
{
    private readonly IProcessService _processService;

    /// <summary>
    /// DIコンテナから工程Serviceを受け取る。
    /// </summary>
    public ProcessesController(IProcessService processService)
    {
        _processService = processService;
    }

    /// <summary>
    /// 工程一覧を取得する。
    ///
    /// GET /api/processes
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProcessDto>>> GetAll()
    {
        var processes = await _processService.GetAllAsync();

        return Ok(processes);
    }

    /// <summary>
    /// 指定したIDの工程を取得する。
    ///
    /// GET /api/processes/1
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProcessDto>> GetById(int id)
    {
        var process = await _processService.GetByIdAsync(id);

        // 指定された工程が存在しなければ404を返す。
        if (process is null)
        {
            return NotFound(new
            {
                message = $"工程ID {id} は存在しません。"
            });
        }

        return Ok(process);
    }

    /// <summary>
    /// 新しい工程を登録する。
    ///
    /// POST /api/processes
    ///
    /// 工程マスタの変更になるためAdminのみ実行可能。
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProcessDto>> Create(
        CreateProcessRequest request)
    {
        try
        {
            var createdProcess =
                await _processService.CreateAsync(request);

            // 201 Createdを返す。
            //
            // Locationヘッダーには、
            // 作成した工程を取得するURLが設定される。
            //
            // 例：
            // /api/processes/1
            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProcess.Id },
                createdProcess);
        }
        catch (InvalidOperationException ex)
        {
            // 工程名がすでに存在する場合など、
            // 業務上の競合なので409 Conflictを返す。
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// 指定した工程を更新する。
    ///
    /// PUT /api/processes/1
    ///
    /// 工程マスタの変更になるためAdminのみ実行可能。
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProcessDto>> Update(
        int id,
        UpdateProcessRequest request)
    {
        try
        {
            var updatedProcess =
                await _processService.UpdateAsync(id, request);

            // 更新対象が存在しなければ404を返す。
            if (updatedProcess is null)
            {
                return NotFound(new
                {
                    message = $"工程ID {id} は存在しません。"
                });
            }

            return Ok(updatedProcess);
        }
        catch (InvalidOperationException ex)
        {
            // 別の工程と同じ名前に変更しようとした場合。
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
}