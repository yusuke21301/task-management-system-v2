using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionProgress.Application.Dtos.Users;
using ProductionProgress.Application.Services;

namespace ProductionProgress.Api.Controllers;

/// <summary>
/// ユーザー管理APIです。
///
/// ユーザー管理は管理者だけが実行できるため、
/// Controller全体にAdmin権限を要求します。
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 登録されているユーザー一覧を取得します。
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetAll()
    {
        var users =
            await _userService.GetAllAsync();

        return Ok(users);
    }

    /// <summary>
    /// Workerユーザーを新規登録します。
    /// </summary>
    /// <summary>
    /// Workerユーザーを新規登録します。
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(
        CreateUserRequest request)
    {
        var result =
            await _userService.CreateWorkerAsync(request);

        return StatusCode(
            StatusCodes.Status201Created,
            result);
    }

    /// <summary>
    /// Workerユーザーの有効・無効を変更します。
    /// </summary>
    [HttpPut("{id:int}/active")]
    public async Task<ActionResult<UserResponse>> SetActive(
        int id,
        SetUserActiveRequest request)
    {
        var result =
            await _userService.SetActiveAsync(
                id,
                request.IsActive);

        // ユーザーが存在しない
        if (result.Status == SetUserActiveStatus.NotFound)
        {
            return NotFound(new
            {
                message = "指定されたユーザーが見つかりません。"
            });
        }

        // Adminユーザーの変更は禁止
        if (result.Status == SetUserActiveStatus.AdminNotAllowed)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    message =
                        "Adminユーザーの有効状態は変更できません。"
                });
        }

        return Ok(result.User);
    }
}