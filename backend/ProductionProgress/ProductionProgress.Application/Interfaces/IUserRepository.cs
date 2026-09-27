using ProductionProgress.Domain.Entities;

namespace ProductionProgress.Application.Interfaces;

/// <summary>
/// ユーザー情報を取得するためのリポジトリです。
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// ユーザー名からユーザーを取得します。
    /// </summary>
    /// <param name="username">検索するユーザー名</param>
    /// <returns>
    /// ユーザーが存在する場合はAppUser、
    /// 存在しない場合はnull
    /// </returns>
    Task<AppUser?> GetByUsernameAsync(string username);

    /// <summary>
    /// IDからユーザーを取得します。
    /// </summary>
    Task<AppUser?> GetByIdAsync(int id);

    /// <summary>
    /// 全ユーザーを取得します。
    /// </summary>
    Task<List<AppUser>> GetAllAsync();

    /// <summary>
    /// ユーザーを追加します。
    /// </summary>
    Task AddAsync(AppUser user);

    /// <summary>
    /// DBへの変更を保存します。
    /// </summary>
    Task SaveChangesAsync();
}