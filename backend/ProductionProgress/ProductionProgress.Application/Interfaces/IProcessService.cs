using ProductionProgress.Application.Dtos.Processes;

namespace ProductionProgress.Application.Interfaces;

/// <summary>
/// 工程マスタに対する業務処理を定義するService。
/// </summary>
public interface IProcessService
{
    /// <summary>
    /// 工程一覧を取得する。
    /// </summary>
    Task<IReadOnlyList<ProcessDto>> GetAllAsync();

    /// <summary>
    /// IDを指定して工程を1件取得する。
    ///
    /// 存在しない場合はnullを返す。
    /// </summary>
    Task<ProcessDto?> GetByIdAsync(int id);

    /// <summary>
    /// 新しい工程を登録する。
    /// </summary>
    Task<ProcessDto> CreateAsync(CreateProcessRequest request);

    /// <summary>
    /// 指定した工程を更新する。
    ///
    /// 対象が存在しない場合はnullを返す。
    /// </summary>
    Task<ProcessDto?> UpdateAsync(
        int id,
        UpdateProcessRequest request);
}