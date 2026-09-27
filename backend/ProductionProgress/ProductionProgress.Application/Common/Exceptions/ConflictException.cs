namespace ProductionProgress.Application.Common.Exceptions;

/// <summary>
/// 現在のデータ状態と処理内容が競合した場合に使用する例外です。
///
/// 例:
/// ・同じユーザー名がすでに存在する
/// ・同じデータを重複登録しようとした
///
/// API層では、この例外をHTTP 409 Conflictへ変換します。
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}