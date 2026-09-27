using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProductionProgress.Application.Dtos.Auth;
using ProductionProgress.Application.Interfaces;
using ProductionProgress.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductionProgress.Infrastructure.Authentication;

/// <summary>
/// JWTアクセストークンを生成します。
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// appsettings.jsonやUser Secretsなどの
    /// 設定情報をDIから受け取ります。
    /// </summary>
    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// ログインに成功したユーザーのJWTを生成します。
    /// </summary>
    public JwtTokenResult GenerateToken(AppUser user)
    {
        // --------------------------------------------------
        // JWT設定を取得
        // --------------------------------------------------

        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "Jwt:Keyが設定されていません。");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "Jwt:Issuerが設定されていません。");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "Jwt:Audienceが設定されていません。");

        // 有効期限（分）を取得します。
        // 未設定の場合は60分とします。
        var expirationMinutes =
            int.TryParse(
                _configuration["Jwt:ExpirationMinutes"],
                out var minutes)
                ? minutes
                : 60;

        var expiresAt =
            DateTime.UtcNow.AddMinutes(expirationMinutes);

        // --------------------------------------------------
        // JWTに入れるユーザー情報（Claim）
        // --------------------------------------------------

        var claims = new[]
        {
            // JWT標準のSubject。
            // このJWTが誰を表しているかを示します。
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            // ASP.NET Core側からUserIdを取得しやすくするため、
            // NameIdentifierにもユーザーIDを格納します。
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            // ユーザー名
            new Claim(
                ClaimTypes.Name,
                user.Username),

            // ユーザー権限
            new Claim(
                ClaimTypes.Role,
                user.Role.ToString()),

            // JWT自体を識別するID
            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        // --------------------------------------------------
        // JWTへ署名するためのキーを作成
        // --------------------------------------------------

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        // --------------------------------------------------
        // JWTを生成
        // --------------------------------------------------

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        // JwtSecurityTokenを文字列へ変換します。
        var tokenString =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        return new JwtTokenResult
        {
            AccessToken = tokenString,
            ExpiresAt = expiresAt
        };
    }
}