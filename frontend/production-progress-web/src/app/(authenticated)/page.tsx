import Link from "next/link";

import {
  getProcesses,
} from "@/lib/process-api";

import {
  getTaskProgressByProcessId,
} from "@/lib/task-api";

import styles from "./dashboard.module.css";

/**
 * トップダッシュボード。
 */
export default async function Home() {
  // --------------------------------------------------
  // 工程一覧を取得
  // --------------------------------------------------

  const processes =
    await getProcesses();

  // --------------------------------------------------
  // 全工程の進捗を取得
  // --------------------------------------------------
  //
  // 各工程は互いに依存していないため、
  // Promise.allで並行して取得する。
  //
  const progresses =
    await Promise.all(
      processes.map(
        (process) =>
          getTaskProgressByProcessId(
            process.id
          )
      )
    );

  // --------------------------------------------------
  // システム全体の件数を集計
  // --------------------------------------------------

  const totalTasks =
    progresses.reduce(
      (total, progress) =>
        total + progress.total,
      0
    );

  const totalNotStarted =
    progresses.reduce(
      (total, progress) =>
        total + progress.notStarted,
      0
    );

  const totalInProgress =
    progresses.reduce(
      (total, progress) =>
        total + progress.inProgress,
      0
    );

  const totalCompleted =
    progresses.reduce(
      (total, progress) =>
        total + progress.completed,
      0
    );

  // --------------------------------------------------
  // 全体の完了率
  // --------------------------------------------------

  const completionRate =
    totalTasks === 0
      ? 0
      : Math.round(
          (
            totalCompleted /
            totalTasks
          ) * 100
        );

  return (
    <main className={styles.container}>

      {/* =====================================
          タイトル
          ===================================== */}
      <header className={styles.header}>
        <h1 className={styles.title}>
          工程進捗管理システム
        </h1>

        <p className={styles.description}>
          工程全体の作業状況を確認できます。
        </p>
      </header>

      {/* =====================================
          全体サマリー
          ===================================== */}
      <section
        className={styles.summaryGrid}
      >

        {/* 工程数 */}
        <div
          className={styles.summaryCard}
        >
          <div
            className={styles.summaryLabel}
          >
            工程数
          </div>

          <div
            className={styles.summaryValue}
          >
            {processes.length}
          </div>
        </div>

        {/* 全作業数 */}
        <div
          className={styles.summaryCard}
        >
          <div
            className={styles.summaryLabel}
          >
            全作業数
          </div>

          <div
            className={styles.summaryValue}
          >
            {totalTasks}
          </div>
        </div>

        {/* 未着手 */}
        <div
          className={styles.summaryCard}
        >
          <div
            className={styles.summaryLabel}
          >
            未着手
          </div>

          <div
            className={styles.summaryValue}
          >
            {totalNotStarted}
          </div>
        </div>

        {/* 作業中 */}
        <div
          className={styles.summaryCard}
        >
          <div
            className={styles.summaryLabel}
          >
            作業中
          </div>

          <div
            className={styles.summaryValue}
          >
            {totalInProgress}
          </div>
        </div>

        {/* 完了 */}
        <div
          className={styles.summaryCard}
        >
          <div
            className={styles.summaryLabel}
          >
            完了
          </div>

          <div
            className={styles.summaryValue}
          >
            {totalCompleted}
          </div>
        </div>

        {/* 完了率 */}
        <div
          className={styles.summaryCard}
        >
          <div
            className={styles.summaryLabel}
          >
            完了率
          </div>

          <div
            className={styles.summaryValue}
          >
            {completionRate}%
          </div>
        </div>

      </section>

      {/* =====================================
          メニュー
          ===================================== */}
      <section className={styles.menu}>
        <Link
          href="/processes"
          className={styles.primaryLink}
        >
          工程進捗を見る
        </Link>
      </section>
    </main>
  );
}