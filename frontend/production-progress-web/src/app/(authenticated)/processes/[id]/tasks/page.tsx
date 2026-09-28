import Link from "next/link";
import { notFound } from "next/navigation";

import {
  getTasksByProcessId,
  type WorkTaskStatus,
} from "@/lib/task-api";

import {
  getProcesses,
} from "@/lib/process-api";

import {
  updateTaskStatus,
  deleteTaskAction,
} from "./actions";

import DeleteTaskButton
  from "./DeleteTaskButton";

import styles from "./tasks.module.css";

/**
 * ステータスを画面表示用の日本語へ変換する。
 */
function getStatusLabel(
  status: WorkTaskStatus
) {
  switch (status) {
    case "NotStarted":
      return "未着手";

    case "InProgress":
      return "作業中";

    case "Completed":
      return "完了";
  }
}

/**
 * ステータスに応じたCSSクラスを返す。
 */
function getStatusClass(
  status: WorkTaskStatus
) {
  switch (status) {
    case "NotStarted":
      return styles.notStarted;

    case "InProgress":
      return styles.inProgress;

    case "Completed":
      return styles.completed;
  }
}

/**
 * YYYY-MM-DD
 *
 * を
 *
 * YYYY/MM/DD
 *
 * に変換する。
 */
function formatDate(
  date: string | null
) {
  if (!date) {
    return "-";
  }

  return date.replaceAll("-", "/");
}

/**
 * 工程ごとの作業一覧画面。
 */
export default async function ProcessTasksPage(
  props: {
    params: Promise<{
      id: string;
    }>;
  }
) {
  // --------------------------------------------------
  // URLから工程IDを取得
  // --------------------------------------------------

  const params = await props.params;

  const processId =
    Number(params.id);

  // 工程IDとして不正な値なら404
  if (
    !Number.isInteger(processId) ||
    processId <= 0
  ) {
    notFound();
  }

  // --------------------------------------------------
  // 工程と作業を取得
  // --------------------------------------------------
  //
  // 2つのAPIは互いに依存していないため、
  // Promise.allで並行して取得する。
  //
  const [
    processes,
    tasks,
  ] = await Promise.all([
    getProcesses(),
    getTasksByProcessId(processId),
  ]);

  // URLで指定された工程を探す
  const process =
    processes.find(
      (item) => item.id === processId
    );

  // 存在しない工程IDなら404
  if (!process) {
    notFound();
  }

  return (
    <main className={styles.container}>

      {/* =====================================
          画面タイトル
          ===================================== */}
      <header className={styles.header}>
        <h1 className={styles.title}>
          {process.processName}
        </h1>

        <p className={styles.description}>
          作業一覧と進捗を確認できます。
        </p>
      </header>

      {/* =====================================
          画面上部の操作
          ===================================== */}
      <div className={styles.topActions}>
        <Link
          href={
            `/processes/${processId}/tasks/new`
          }
          className={styles.primaryButton}
        >
          作業を新規登録
        </Link>
      </div> 

      {/* =====================================
          作業一覧
          ===================================== */}

      {tasks.length === 0 ? (
        <p>
          この工程には作業が登録されていません。
        </p>
      ) : (
        <div className={styles.tableWrapper}>
          <table className={styles.table}>

            <thead>
              <tr>
                <th>ID</th>
                <th>作業名</th>
                <th>ステータス</th>
                <th>予定日</th>
                <th>ステータス変更</th>
                <th>操作</th>
              </tr>
            </thead>

            <tbody>
              {tasks.map((task) => (
                <tr key={task.id}>

                  {/* ID */}
                  <td>
                    {task.id}
                  </td>

                  {/* 作業名 */}
                  <td>
                    {task.taskName}
                  </td>

                  {/* 現在のステータス */}
                  <td>
                    <span
                      className={
                        `${styles.statusBadge} ${getStatusClass(task.status)}`
                      }
                    >
                      {getStatusLabel(task.status)}
                    </span>
                  </td>

                  {/* 予定日 */}
                  <td>
                    {formatDate(
                      task.plannedDate
                    )}
                  </td>

                  {/* =================================
                      ステータス更新
                      ================================= */}
                  <td>
                    <form
                      action={updateTaskStatus}
                      className={styles.buttonGroup}
                    >
                      {/* 更新APIへ渡す現在値 */}

                      <input
                        type="hidden"
                        name="taskId"
                        value={task.id}
                      />

                      <input
                        type="hidden"
                        name="processId"
                        value={task.processId}
                      />

                      <input
                        type="hidden"
                        name="taskName"
                        value={task.taskName}
                      />

                      <input
                        type="hidden"
                        name="plannedDate"
                        value={
                          task.plannedDate ?? ""
                        }
                      />

                      {/* 未着手 */}
                      <button
                        className={
                          styles.statusButton
                        }
                        type="submit"
                        name="status"
                        value="NotStarted"
                        disabled={
                          task.status ===
                          "NotStarted"
                        }
                      >
                        未着手
                      </button>

                      {/* 作業中 */}
                      <button
                        className={
                          styles.statusButton
                        }
                        type="submit"
                        name="status"
                        value="InProgress"
                        disabled={
                          task.status ===
                          "InProgress"
                        }
                      >
                        作業中
                      </button>

                      {/* 完了 */}
                      <button
                        className={
                          styles.statusButton
                        }
                        type="submit"
                        name="status"
                        value="Completed"
                        disabled={
                          task.status ===
                          "Completed"
                        }
                      >
                        完了
                      </button>

                    </form>
                  </td>

                  {/* =================================
                      操作
                      ================================= */}
                  <td>
                    <div className={styles.operationGroup}>
                      {/* 作業編集 */}
                      <Link
                        href={
                          `/processes/${processId}/tasks/${task.id}/edit`
                        }
                        className={styles.editLink}
                      >
                        編集
                      </Link>

                      {/* 作業削除 */}
                      <DeleteTaskButton
                        taskId={task.id}
                        processId={processId}
                        taskName={task.taskName}
                        deleteAction={deleteTaskAction}
                      />
                    </div>
                  </td>

                </tr>
              ))}
            </tbody>

          </table>
        </div>
      )}

      {/* =====================================
          戻る
          ===================================== */}
      <div className={styles.footer}>
        <Link href="/processes">
          ← 工程一覧へ戻る
        </Link>
      </div>

    </main>
  );
}