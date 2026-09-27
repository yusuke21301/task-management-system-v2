using Microsoft.EntityFrameworkCore;
using ProductionProgress.Application.Dtos.Common;
using ProductionProgress.Application.Dtos.Tasks;
using ProductionProgress.Application.Interfaces;
using ProductionProgress.Domain.Entities;
using ProductionProgress.Infrastructure.Data;

namespace ProductionProgress.Infrastructure.Repositories;

/// <summary>
/// タスクデータをPostgreSQLから取得するRepository。
///
/// ITaskRepositoryの具体的な実装をInfrastructure層に置くことで、
/// Application層からDBアクセス処理を分離する。
/// </summary>
public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// DIによってAppDbContextを受け取る。
    /// </summary>
    public TaskRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// 検索条件に一致するタスク一覧をDBから取得する。
    /// </summary>
    public async Task<PagedResult<WorkTask>> GetAllAsync(
        TaskSearchCondition condition)
    {
        // この時点ではSQLを実行しない。
        //
        // IQueryableを使ってWHERE条件を組み立て、
        // 最後のToListAsync()でDBへ問い合わせる。
        IQueryable<WorkTask> query = _dbContext.WorkTasks
            // TaskDtoで工程名を使用するため、
            // 関連する工程も取得する。
            .Include(x => x.Process)
            .AsNoTracking();

        // --------------------------------
        // 工程
        // --------------------------------
        if (condition.ProcessId.HasValue)
        {
            query = query.Where(
                x => x.ProcessId == condition.ProcessId.Value);
        }

        // --------------------------------
        // ステータス
        // --------------------------------
        if (condition.Status.HasValue)
        {
            query = query.Where(
                x => x.Status == condition.Status.Value);
        }

        // --------------------------------
        // 予定日 From
        // --------------------------------
        if (condition.PlannedDateFrom.HasValue)
        {
            // 指定日以降のタスクを取得する。
            query = query.Where(
                x => x.PlannedDate >= condition.PlannedDateFrom.Value);
        }

        // --------------------------------
        // 予定日 To
        // --------------------------------
        if (condition.PlannedDateTo.HasValue)
        {
            // 指定日以前のタスクを取得する。
            query = query.Where(
                x => x.PlannedDate <= condition.PlannedDateTo.Value);
        }

        // --------------------------------
        // タスク名
        // --------------------------------
        if (!string.IsNullOrWhiteSpace(condition.Keyword))
        {
            // 前後の空白を取り除いてから検索する。
            var keyword = condition.Keyword.Trim();

            // タスク名に検索文字列が含まれているものを取得する。
            query = query.Where(
                x => x.TaskName.Contains(keyword));
        }

        // ページングする前に、
        // 検索条件に一致する全件数を取得する。
        var totalCount =
            await query.CountAsync();

        // 何件読み飛ばすか計算する。
        //
        // page=1, pageSize=20
        // → 0件読み飛ばす
        //
        // page=2, pageSize=20
        // → 20件読み飛ばす
        //
        // page=3, pageSize=20
        // → 40件読み飛ばす
        var skip =
            (condition.Page - 1) * condition.PageSize;

        // 指定ページに必要なデータだけ取得する。
        var items = await query
            .OrderBy(x => x.Id)
            .Skip(skip)
            .Take(condition.PageSize)
            .ToListAsync();

        return new PagedResult<WorkTask>
        {
            Items = items,
            Page = condition.Page,
            PageSize = condition.PageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// IDを指定してタスクを1件取得する。
    /// </summary>
    public async Task<WorkTask?> GetByIdAsync(int id)
    {
        // 読み取り専用なのでAsNoTrackingを使用する。
        //
        // SingleOrDefaultAsyncは、
        // 条件に一致するデータが1件あればそのデータを返し、
        // 0件の場合はnullを返す。
        return await _dbContext.WorkTasks
            // TaskDtoで工程名を使用できるように、
            // 関連する工程も取得する。
            .Include(x => x.Process)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// 新しいタスクをDBへ登録する。
    /// </summary>
    public async Task<WorkTask> AddAsync(WorkTask task)
    {
        await _dbContext.WorkTasks.AddAsync(task);

        await _dbContext.SaveChangesAsync();

        return task;
    }

    /// <summary>
    /// 既存のタスクをPostgreSQL上で更新する。
    /// </summary>
    public async Task<WorkTask> UpdateAsync(WorkTask task)
    {
        // GetByIdAsyncではAsNoTrackingを使用しているため、
        // 取得したEntityはEF Coreの変更追跡対象になっていない。
        //
        // Updateを呼び出すことで、
        // このEntityを更新対象としてDbContextへ登録する。
        _dbContext.WorkTasks.Update(task);

        // SaveChangesAsyncを実行すると、
        // PostgreSQLへUPDATE文が送信される。
        await _dbContext.SaveChangesAsync();

        return task;
    }

    /// <summary>
    /// 指定されたタスクをPostgreSQLから削除する。
    /// </summary>
    public async Task DeleteAsync(WorkTask task)
    {
        // 削除対象としてDbContextへ登録する。
        //
        // GetByIdAsyncではAsNoTrackingを使用しているため、
        // 取得したEntityは変更追跡されていない。
        // Removeを呼び出すことで削除対象として扱われる。
        _dbContext.WorkTasks.Remove(task);

        // SaveChangesAsyncを実行すると、
        // PostgreSQLへDELETE文が送信される。
        await _dbContext.SaveChangesAsync();
    }
}