using Microsoft.EntityFrameworkCore;
using ProductionProgress.Application.Interfaces;
using ProductionProgress.Domain.Entities;
using ProductionProgress.Infrastructure.Data;

namespace ProductionProgress.Infrastructure.Repositories;

/// <summary>
/// PostgreSQLに対する工程マスタのDBアクセス処理。
///
/// Application層で定義したIProcessRepositoryを
/// Infrastructure層で実装する。
/// </summary>
public class ProcessRepository : IProcessRepository
{
    private readonly AppDbContext _db;

    /// <summary>
    /// DIコンテナからAppDbContextを受け取る。
    /// </summary>
    public ProcessRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// 工程をすべて取得する。
    ///
    /// 表示順 → IDの順番で並べて返す。
    /// 読み取り専用なのでAsNoTracking()を使用する。
    /// </summary>
    public async Task<IReadOnlyList<WorkProcess>> GetAllAsync()
    {
        return await _db.Processes
            .AsNoTracking()
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Id)
            .ToListAsync();
    }

    /// <summary>
    /// IDから工程を1件取得する。
    ///
    /// このEntityは後で更新する可能性があるため、
    /// AsNoTracking()は使用しない。
    /// EF CoreがEntityの変更を追跡できる状態にする。
    /// </summary>
    public async Task<WorkProcess?> GetByIdAsync(int id)
    {
        return await _db.Processes
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// 工程名から工程を取得する。
    ///
    /// 主に工程名の重複チェックで使用する。
    /// 内容を変更する目的ではないためAsNoTracking()を使用する。
    /// </summary>
    public async Task<WorkProcess?> GetByNameAsync(string processName)
    {
        return await _db.Processes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProcessName == processName);
    }

    /// <summary>
    /// 新しい工程をDbContextへ追加する。
    ///
    /// ここではまだDBへの保存は確定しない。
    /// </summary>
    public async Task AddAsync(WorkProcess process)
    {
        await _db.Processes.AddAsync(process);
    }

    /// <summary>
    /// DbContextで管理している変更をDBへ保存する。
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}