namespace ProductionProgress.Domain.Enums;

/// <summary>
/// 作業の進捗状態を表す。
///
/// DBには整数値として保存する。
/// 0 = 未着手
/// 1 = 作業中
/// 2 = 完了
/// </summary>
public enum WorkTaskStatus
{
    /// <summary>
    /// まだ作業を開始していない状態。
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// 現在作業中の状態。
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// 作業が完了した状態。
    /// </summary>
    Completed = 2
}