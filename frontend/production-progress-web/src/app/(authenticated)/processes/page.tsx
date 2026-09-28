import Link from "next/link";

import {
  getProcesses,
} from "@/lib/process-api";

import {
  getTaskProgressByProcessId,
} from "@/lib/task-api";

import styles from "./processes.module.css";

/**
 * 工程一覧・進捗ダッシュボード。
 */
export default async function ProcessesPage() {
  // -----------------------------------------------
  // 工程一覧を取得
  // -----------------------------------------------

  const processes =
    await getProcesses();

  // -----------------------------------------------
  // 有効な工程だけを表示対象にする
  // -----------------------------------------------
  //
  // isActive = false の工程は、
  // 通常の工程進捗画面には表示しない。
  //
  const activeProcesses =
    processes.filter(
      (process) => process.isActive
    );

  // -----------------------------------------------
  // 各工程の進捗を取得
  // -----------------------------------------------
  //
  // 工程ごとの取得処理は互いに依存しないので、
  // Promise.allで並行実行する。
  //
  const processesWithProgress =
    await Promise.all(
      activeProcesses.map(
        async (process) => {
          const progress =
            await getTaskProgressByProcessId(
              process.id
            );

          return {
            ...process,
            progress,
          };
        }
      )
    );

  return (
    <main className={styles.container}>

      {/* =====================================
          画面上部
          ===================================== */}
      <header className={styles.header}>
        <h1 className={styles.title}>
          工程進捗
        </h1>

        <p className={styles.description}>
          各工程の作業進捗を確認できます。
        </p>
      </header>

      {/* =====================================
          画面上部の操作
          ===================================== */}
      <div className={styles.topActions}>
        <Link
          href="/processes/new"
          className={styles.primaryButton}
        >
          工程を新規登録
        </Link>

        {/* 無効工程管理 */}
        <Link
          href="/processes/inactive"
          className={styles.secondaryButton}
        >
          無効工程の管理
        </Link>
      </div>

      {/* =====================================
          工程一覧
          ===================================== */}
      {processesWithProgress.map(
        (process) => {
          const {
            progress,
          } = process;

          // ---------------------------------
          // 完了率を計算
          // ---------------------------------
          const completionRate =
            progress.total === 0
              ? 0
              : Math.round(
                  (
                    progress.completed /
                    progress.total
                  ) * 100
                );

          return (
            <div
              key={process.id}
              className={styles.card}
            >
              {/* =================================
                  工程進捗画面へのリンク
                  ================================= */}
              <Link
                href={
                  `/processes/${process.id}/tasks`
                }
                className={styles.cardMain}
              >
                {/* 工程名 */}
                <h2
                  className={
                    styles.processName
                  }
                >
                  {process.processName}
                </h2>

                {/* 全作業数 */}
                <div
                  className={styles.total}
                >
                  全作業：
                  {progress.total}件
                </div>

                {/* ステータスごとの件数 */}
                <div
                  className={
                    styles.statusList
                  }
                >
                  <span
                    className={
                      styles.statusItem
                    }
                  >
                    未着手：
                    {progress.notStarted}
                  </span>

                  <span
                    className={
                      styles.statusItem
                    }
                  >
                    作業中：
                    {progress.inProgress}
                  </span>

                  <span
                    className={
                      styles.statusItem
                    }
                  >
                    完了：
                    {progress.completed}
                  </span>
                </div>

                {/* =================================
                    完了率
                    ================================= */}
                <div
                  className={
                    styles.progressArea
                  }
                >
                  <div
                    className={
                      styles.progressBar
                    }
                  >
                    <div
                      className={
                        styles.progressValue
                      }
                      style={{
                        width:
                          `${completionRate}%`,
                      }}
                    />
                  </div>

                  <span
                    className={
                      styles.progressText
                    }
                  >
                    {completionRate}%
                  </span>
                </div>
              </Link>

              {/* =================================
                  工程操作
                  ================================= */}
              <div className={styles.cardActions}>
                <Link
                  href={
                    `/processes/${process.id}/edit`
                  }
                  className={styles.editLink}
                >
                  編集
                </Link>
              </div>
            </div>
          );
        }
      )}

      <div className={styles.footer}>
        <Link href="/">
          ← トップへ戻る
        </Link>
      </div>

    </main>
  );
}