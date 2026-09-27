using ProductionProgress.Domain.Entities;

namespace ProductionProgress.Application.Interfaces;

/// <summary>
/// パスワードのハッシュ化と検証を行うサービスです。
///
/// Application層では具体的なハッシュ化方式を意識せず、
/// このインターフェースを通して処理します。
/// </summary>
public interface IPasswordHashService
{
    /// <summary>
    /// パスワードをハッシュ化します。
    /// </summary>
    /// <param name="user">対象ユーザー</param>
    /// <param name="password">平文パスワード</param>
    /// <returns>ハッシュ化されたパスワード</returns>
    string HashPassword(AppUser user, string password);

    /// <summary>
    /// 入力されたパスワードが、
    /// 保存済みのハッシュ値と一致するか確認します。
    /// </summary>
    /// <param name="user">対象ユーザー</param>
    /// <param name="password">ログイン時に入力されたパスワード</param>
    /// <returns>一致する場合 true</returns>
    bool VerifyPassword(AppUser user, string password);
}