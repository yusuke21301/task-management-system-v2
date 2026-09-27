using Microsoft.EntityFrameworkCore;
using ProductionProgress.Application.Interfaces;
using ProductionProgress.Domain.Entities;
using ProductionProgress.Domain.Enums;

namespace ProductionProgress.Infrastructure.Data;

/// <summary>
/// システム初期起動時に必要なデータを登録するクラスです。
/// </summary>
public class InitialAdminSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHashService _passwordHashService;

    /// <summary>
    /// DIからDBコンテキストとパスワードハッシュサービスを受け取ります。
    /// </summary>
    public InitialAdminSeeder(
        AppDbContext dbContext,
        IPasswordHashService passwordHashService)
    {
        _dbContext = dbContext;
        _passwordHashService = passwordHashService;
    }

    /// <summary>
    /// 管理者ユーザーがまだ存在しない場合のみ、
    /// 初期管理者ユーザーを登録します。
    /// </summary>
    public async Task SeedAdminAsync(
        string username,
        string password)
    {
        // すでに管理者ユーザーが存在する場合は何もしません。
        // アプリ起動のたびにadminが増えることを防ぎます。
        var adminExists = await _dbContext.Users
            .AnyAsync(x => x.Role == UserRole.Admin);

        if (adminExists)
        {
            return;
        }

        // 同じユーザー名がすでに存在する場合は、
        // UNIQUE制約エラーになるため明示的にエラーとします。
        var sameUsernameExists = await _dbContext.Users
            .AnyAsync(x => x.Username == username);

        if (sameUsernameExists)
        {
            throw new InvalidOperationException(
                $"ユーザー名 '{username}' はすでに使用されています。");
        }

        // 管理者ユーザーを作成します。
        var user = new AppUser
        {
            Username = username,
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // 平文パスワードはDBに保存しません。
        // PasswordHasherでハッシュ化した値だけを保存します。
        user.PasswordHash =
            _passwordHashService.HashPassword(user, password);

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();
    }
}