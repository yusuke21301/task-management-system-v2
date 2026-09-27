using ProductionProgress.Application.Dtos.Processes;
using ProductionProgress.Application.Interfaces;
using ProductionProgress.Domain.Entities;

namespace ProductionProgress.Application.Services;

/// <summary>
/// 工程マスタに関する業務処理を行うService。
///
/// RepositoryはDBアクセスを担当し、
/// Serviceは重複チェックなどの業務ルールを担当する。
/// </summary>
public class ProcessService : IProcessService
{
    private readonly IProcessRepository _processRepository;

    /// <summary>
    /// DIコンテナからRepositoryを受け取る。
    /// </summary>
    public ProcessService(IProcessRepository processRepository)
    {
        _processRepository = processRepository;
    }

    /// <summary>
    /// 工程一覧を取得する。
    /// </summary>
    public async Task<IReadOnlyList<ProcessDto>> GetAllAsync()
    {
        // RepositoryからEntity一覧を取得する。
        var processes = await _processRepository.GetAllAsync();

        // Entityをそのまま外部へ返さず、
        // DTOへ変換して返す。
        return processes
            .Select(ToDto)
            .ToList();
    }

    /// <summary>
    /// IDを指定して工程を取得する。
    /// </summary>
    public async Task<ProcessDto?> GetByIdAsync(int id)
    {
        var process = await _processRepository.GetByIdAsync(id);

        // 工程が存在しない場合はnullを返す。
        if (process is null)
        {
            return null;
        }

        return ToDto(process);
    }

    /// <summary>
    /// 新しい工程を登録する。
    /// </summary>
    public async Task<ProcessDto> CreateAsync(
        CreateProcessRequest request)
    {
        // 前後の余分な空白を除去する。
        // 例：
        // "  塗装  "
        // ↓
        // "塗装"
        var processName = request.ProcessName.Trim();

        // 同じ名前の工程がすでに存在しないか確認する。
        var existingProcess =
            await _processRepository.GetByNameAsync(processName);

        if (existingProcess is not null)
        {
            throw new InvalidOperationException(
                $"工程名「{processName}」は既に登録されています。");
        }

        var now = DateTime.Now;

        // DTOではなくEntityを作成し、
        // DBへ登録できる形にする。
        var process = new WorkProcess
        {
            ProcessName = processName,
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive,
            CreatedAt = now,
            UpdatedAt = now
        };

        // DbContextへ追加する。
        await _processRepository.AddAsync(process);

        // DBへ保存する。
        await _processRepository.SaveChangesAsync();

        // 保存後は自動採番されたIdもprocessへ設定されている。
        return ToDto(process);
    }

    /// <summary>
    /// 既存の工程を更新する。
    /// </summary>
    public async Task<ProcessDto?> UpdateAsync(
        int id,
        UpdateProcessRequest request)
    {
        // 更新対象を取得する。
        //
        // GetByIdAsyncではAsNoTrackingを使っていないため、
        // EF CoreがこのEntityを追跡している。
        var process =
            await _processRepository.GetByIdAsync(id);

        if (process is null)
        {
            return null;
        }

        var processName = request.ProcessName.Trim();

        // 同じ工程名が別の工程で使われていないか確認する。
        var sameNameProcess =
            await _processRepository.GetByNameAsync(processName);

        // 同じ名前が見つかっても、
        // それが自分自身なら問題ない。
        //
        // 例：
        // ID=1 塗装
        // を
        // ID=1 塗装
        // のまま更新する場合。
        if (sameNameProcess is not null &&
            sameNameProcess.Id != id)
        {
            throw new InvalidOperationException(
                $"工程名「{processName}」は既に登録されています。");
        }

        // EF Coreが追跡しているEntityを書き換える。
        process.ProcessName = processName;
        process.DisplayOrder = request.DisplayOrder;
        process.IsActive = request.IsActive;
        process.UpdatedAt = DateTime.Now;

        // SaveChangesAsync()を実行すると、
        // 変更された項目がUPDATEされる。
        await _processRepository.SaveChangesAsync();

        return ToDto(process);
    }

    /// <summary>
    /// WorkProcess EntityをProcessDtoへ変換する。
    ///
    /// 一覧取得・1件取得・登録・更新で
    /// 同じ変換処理を書く必要がないように
    /// 共通メソッドとして切り出している。
    /// </summary>
    private static ProcessDto ToDto(WorkProcess process)
    {
        return new ProcessDto
        {
            Id = process.Id,
            ProcessName = process.ProcessName,
            DisplayOrder = process.DisplayOrder,
            IsActive = process.IsActive,
            CreatedAt = process.CreatedAt,
            UpdatedAt = process.UpdatedAt
        };
    }
}