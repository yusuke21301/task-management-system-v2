using Microsoft.EntityFrameworkCore;
using ProductionProgress.Application.Interfaces;
using ProductionProgress.Domain.Entities;
using ProductionProgress.Infrastructure.Data;
using Npgsql;
using ProductionProgress.Application.Common.Exceptions;

namespace ProductionProgress.Infrastructure.Repositories;

/// <summary>
/// PostgreSQL上のユーザー情報を操作するRepositoryです。
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// ユーザー名からユーザーを取得します。
    ///
    /// ログイン処理ではデータを更新しないため、
    /// AsNoTrackingを使用します。
    /// </summary>
    public async Task<AppUser?> GetByUsernameAsync(string username)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    /// <summary>
    /// IDからユーザーを取得します。
    ///
    /// この後IsActiveなどを変更する可能性があるため、
    /// ここではAsNoTrackingを使用しません。
    /// </summary>
    public async Task<AppUser?> GetByIdAsync(int id)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// 全ユーザーを取得します。
    /// </summary>
    public async Task<List<AppUser>> GetAllAsync()
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    /// <summary>
    /// 新しいユーザーを追加します。
    /// </summary>
    public async Task AddAsync(AppUser user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    /// <summary>
    /// ユーザー情報の変更をDBへ保存します。
    /// </summary>
    public async Task SaveChangesAsync()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (
                ex.InnerException is PostgresException postgresException
                &&
                postgresException.SqlState
                    == PostgresErrorCodes.UniqueViolation
                &&
                postgresException.ConstraintName
                    == "IX_users_username"
            )
        {
            throw new ConflictException(
                "同じユーザー名がすでに登録されています。");
        }
    }
}