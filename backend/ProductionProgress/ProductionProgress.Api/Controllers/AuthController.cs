using Microsoft.AspNetCore.Mvc;
using ProductionProgress.Application.Dtos.Auth;
using ProductionProgress.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ProductionProgress.Api.Controllers;

/// <summary>
/// 認証に関するAPIを提供します。
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    /// <summary>
    /// DIからAuthServiceを受け取ります。
    /// </summary>
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// ユーザー名とパスワードを使用してログインします。
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        // ユーザーが存在しない場合も、
        // パスワードが違う場合も同じ401を返します。
        //
        // 「ユーザー名は存在する」などの情報を
        // 外部へ漏らさないためです。
        if (result is null)
        {
            return Unauthorized(new
            {
                message = "ユーザー名またはパスワードが正しくありません。"
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// 現在ログインしているユーザーの情報を取得します。
    ///
    /// [Authorize] が付いているため、
    /// 正常なJWTを持っているユーザーだけ実行できます。
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        // JWTから復元されたユーザー名を取得します。
        var username = User.Identity?.Name;

        // JWTから権限を取得します。
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            username,
            role
        });
    }

    /// <summary>
    /// Admin権限を持つユーザーだけが実行できる確認用APIです。
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminOnly()
    {
        return Ok(new
        {
            message = "Admin権限でアクセスしました。"
        });
    }

    /// <summary>
    /// Worker権限を持つユーザーだけが実行できる確認用APIです。
    /// </summary>
    [Authorize(Roles = "Worker")]
    [HttpGet("worker-only")]
    public IActionResult WorkerOnly()
    {
        return Ok(new
        {
            message = "Worker権限でアクセスしました。"
        });
    }

    /// <summary>
    /// ログイン中ユーザーのパスワードを変更します。
    /// </summary>
    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordRequest request)
    {
        // --------------------------------------------------
        // JWTに格納したUserIdを取得します。
        // --------------------------------------------------
        var userIdValue =
            User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

        // 正常に認証されていれば通常ここには入りませんが、
        // Claimが存在しない場合に備えてチェックします。
        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var result =
            await _authService.ChangePasswordAsync(
                userId,
                request);

        switch (result)
        {
            // --------------------------------------------------
            // ユーザーが存在しない
            // --------------------------------------------------
            case ChangePasswordStatus.UserNotFound:
                return Unauthorized();

            // --------------------------------------------------
            // 現在のパスワードが間違っている
            // --------------------------------------------------
            case ChangePasswordStatus.CurrentPasswordIncorrect:
                return BadRequest(new
                {
                    message = "現在のパスワードが正しくありません。"
                });

            // --------------------------------------------------
            // 新旧パスワードが同じ
            // --------------------------------------------------
            case ChangePasswordStatus.SamePassword:
                return BadRequest(new
                {
                    message =
                        "現在とは異なるパスワードを指定してください。"
                });

            // --------------------------------------------------
            // パスワード変更成功
            // --------------------------------------------------
            case ChangePasswordStatus.Success:
                return NoContent();

            default:
                throw new InvalidOperationException(
                    "想定されていないパスワード変更結果です。");
        }
    }
}