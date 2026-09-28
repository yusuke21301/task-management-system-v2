import Link from "next/link";
import { notFound } from "next/navigation";

import {
  getTask,
} from "@/lib/task-api";

import {
  getProcess,
} from "@/lib/process-api";

import {
  updateTaskAction,
} from "../../actions";

import styles from "../../tasks.module.css";

/**
 * URLパラメータ。
 *
 * /processes/1/tasks/10/edit
 *            ↑       ↑
 *            id      taskId
 */
type Props = {
  params: Promise<{
    id: string;
    taskId: string;
  }>;
};

/**
 * 作業編集画面。
 */
export default async function EditTaskPage({
  params,
}: Props) {
  // --------------------------------------------------
  // URLパラメータを取得
  // --------------------------------------------------
  const {
    id,
    taskId,
  } = await params;

  const processId =
    Number(id);

  const targetTaskId =
    Number(taskId);

  // --------------------------------------------------
  // IDの妥当性を確認
  // --------------------------------------------------
  if (
    !Number.isInteger(processId) ||
    processId <= 0 ||
    !Number.isInteger(targetTaskId) ||
    targetTaskId <= 0
  ) {
    notFound();
  }

  // --------------------------------------------------
  // 工程情報と作業情報を取得
  // --------------------------------------------------
  const [
    process,
    task,
  ] = await Promise.all([
    getProcess(processId),
    getTask(targetTaskId),
  ]);

  // --------------------------------------------------
  // URLの工程IDと、
  // 作業が所属する工程IDが一致するか確認
  // --------------------------------------------------
  if (
    task.processId !== processId
  ) {
    notFound();
  }

  return (
    <main className={styles.container}>

      {/* =====================================
          画面タイトル
          ===================================== */}
      <header className={styles.header}>
        <h1 className={styles.title}>
          作業編集
        </h1>

        <p className={styles.description}>
          {process.processName}
          の作業情報を変更します。
        </p>
      </header>

      {/* =====================================
          作業編集フォーム
          ===================================== */}
      <form
        action={updateTaskAction}
        className={styles.form}
      >
        {/* 更新対象の作業ID */}
        <input
          type="hidden"
          name="taskId"
          value={task.id}
        />

        {/* 所属工程ID */}
        <input
          type="hidden"
          name="processId"
          value={process.id}
        />

        {/* =================================
            作業名
            ================================= */}
        <div className={styles.formGroup}>
          <label
            htmlFor="taskName"
            className={styles.label}
          >
            作業名
          </label>

          <input
            id="taskName"
            name="taskName"
            type="text"
            className={styles.input}
            defaultValue={task.taskName}
            maxLength={200}
            required
          />
        </div>

        {/* =================================
            予定日
            ================================= */}
        <div className={styles.formGroup}>
          <label
            htmlFor="plannedDate"
            className={styles.label}
          >
            予定日
          </label>

          <input
            id="plannedDate"
            name="plannedDate"
            type="date"
            className={styles.input}
            defaultValue={
              task.plannedDate
                ? task.plannedDate.substring(
                    0,
                    10
                  )
                : ""
            }
          />
        </div>

        {/* =================================
            ステータス
            ================================= */}
        <div className={styles.formGroup}>
          <label
            htmlFor="status"
            className={styles.label}
          >
            ステータス
          </label>

          <select
            id="status"
            name="status"
            className={styles.input}
            defaultValue={task.status}
          >
            <option value="NotStarted">
              未着手
            </option>

            <option value="InProgress">
              作業中
            </option>

            <option value="Completed">
              完了
            </option>
          </select>
        </div>

        {/* =================================
            操作ボタン
            ================================= */}
        <div className={styles.formActions}>
          <button
            type="submit"
            className={styles.primaryButton}
          >
            更新
          </button>

          <Link
            href={`/processes/${processId}/tasks`}
            className={styles.secondaryButton}
          >
            キャンセル
          </Link>
        </div>

      </form>

    </main>
  );
}