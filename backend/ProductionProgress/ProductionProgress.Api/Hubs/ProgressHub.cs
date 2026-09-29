using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ProductionProgress.Api.Hubs;

/// <summary>
/// 工程進捗の変更をクライアントへ通知するためのSignalR Hub。
/// 
/// Hubは、サーバーとクライアントがリアルタイム通信を行うための
/// 「通信窓口」の役割を持つ。
/// </summary>
[Authorize]
public class ProgressHub : Hub
{
    // 現時点ではHub内に処理は実装しない。
    //
    // 後のPhaseで、
    // Taskが登録・更新・削除されたことを
    // Next.jsへ通知する仕組みを追加する。
}