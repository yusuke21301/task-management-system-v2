namespace ProductionProgress.Application.Dtos.Common;

/// <summary>
/// ページングされた一覧データを表す共通DTO。
///
/// Task以外の一覧APIでも再利用できるように
/// ジェネリック型として定義する。
/// </summary>
public class PagedResult<T>
{
    /// <summary>
    /// 現在のページに含まれるデータ。
    /// </summary>
    public IReadOnlyList<T> Items { get; set; }
        = new List<T>();

    /// <summary>
    /// 現在のページ番号。
    /// 1から開始する。
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 1ページあたりの件数。
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 検索条件に一致する全件数。
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 全ページ数。
    ///
    /// 例：
    /// 83件を20件ずつ表示する場合
    /// → 5ページ
    /// </summary>
    public int TotalPages =>
        PageSize == 0
            ? 0
            : (int)Math.Ceiling(
                TotalCount / (double)PageSize);
}