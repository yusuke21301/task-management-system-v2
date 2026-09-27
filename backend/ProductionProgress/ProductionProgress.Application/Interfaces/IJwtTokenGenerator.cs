using ProductionProgress.Application.Dtos.Auth;
using ProductionProgress.Domain.Entities;

namespace ProductionProgress.Application.Interfaces;

/// <summary>
/// JWTアクセストークンを生成するためのインターフェースです。
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// 指定したユーザーのJWTを生成します。
    /// </summary>
    /// <param name="user">ログインに成功したユーザー</param>
    /// <returns>JWTと有効期限</returns>
    JwtTokenResult GenerateToken(AppUser user);
}