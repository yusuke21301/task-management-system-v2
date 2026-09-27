using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductionProgress.Application.Common.Exceptions;

namespace ProductionProgress.Api.ExceptionHandlers;

/// <summary>
/// アプリケーション全体で発生した
/// 未処理例外を共通処理するクラス。
///
/// Controllerごとにtry-catchを書くのではなく、
/// ここで一括して例外を処理する。
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>
    /// ASP.NET CoreのDIからLoggerを受け取る。
    /// </summary>
    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// アプリケーション内で処理されなかった例外を処理する。
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
    {
        // --------------------------------------------------
        // 例外の種類からHTTPレスポンスを決定します。
        // --------------------------------------------------
        var (statusCode, title, detail) = exception switch
        {
            // データの競合
            ConflictException => (
                StatusCodes.Status409Conflict,
                "データが競合しました。",
                exception.Message),

            // その他の予期しない例外
            _ => (
                StatusCodes.Status500InternalServerError,
                "サーバー内部でエラーが発生しました。",
                "処理中に予期しないエラーが発生しました。")
        };

        // --------------------------------------------------
        // ログ出力
        // --------------------------------------------------

        if (statusCode >= 500)
        {
            // 500系はシステム障害の可能性があるのでError
            _logger.LogError(
                exception,
                "未処理の例外が発生しました。");
        }
        else
        {
            // 409など想定可能な業務上の例外はWarning
            _logger.LogWarning(
                exception,
                "処理できないリクエストが発生しました。");
        }

        // --------------------------------------------------
        // ProblemDetails形式でレスポンスを返します。
        // --------------------------------------------------
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}