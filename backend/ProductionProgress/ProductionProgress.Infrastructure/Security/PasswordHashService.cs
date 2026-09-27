using Microsoft.AspNetCore.Identity;
using ProductionProgress.Application.Interfaces;
using ProductionProgress.Domain.Entities;

namespace ProductionProgress.Infrastructure.Security;

/// <summary>
/// ASP.NET Core IdentityのPasswordHasherを使用して、
/// パスワードのハッシュ化・検証を行います。
/// </summary>
public class PasswordHashService : IPasswordHashService
{
    /// <summary>
    /// ASP.NET Core標準のパスワードハッシャーです。
    ///
    /// 自分でSHA256などを直接使用するのではなく、
    /// パスワード保存用途として用意されたPasswordHasherを使用します。
    /// </summary>
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    /// <summary>
    /// 平文パスワードをハッシュ化します。
    /// </summary>
    public string HashPassword(AppUser user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    /// <summary>
    /// 入力されたパスワードと、
    /// DBに保存されているハッシュ値を比較します。
    /// </summary>
    public bool VerifyPassword(AppUser user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password);

        // Success
        //     → パスワード一致
        //
        // SuccessRehashNeeded
        //     → パスワードは一致しているが、
        //       より新しいハッシュ方式への更新が推奨されている
        //
        // Failed
        //     → 不一致
        return result == PasswordVerificationResult.Success
            || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}