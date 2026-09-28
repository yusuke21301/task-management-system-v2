import Link from "next/link";
import { notFound } from "next/navigation";

import {
  getProcess,
} from "@/lib/process-api";

import {
  createTaskAction,
} from "../actions";

import styles from "../tasks.module.css";

/**
 * URLパラメータ。
 *
 * /processes/1/tasks/new
 *            ↑
 *         processId
 */
type Props = {
  params: Promise<{
    id: string;
  }>;
};

/**
 * 作業新規登録画面。
 */
export default async function NewTaskPage({
  params,
}: Props) {
  // -----------------------------------------------
  // URLから工程IDを取得
  // -----------------------------------------------

  const { id } =
    await params;

  const processId =
    Number(id);

  if (Number.isNaN(processId)) {
    notFound();
  }

  // -----------------------------------------------
  // 工程情報を取得
  // -----------------------------------------------

  let process;

  try {
    process =
      await getProcess(processId);
  } catch {
    notFound();
  }

  return (
    <main className={styles.container}>

      {/* =====================================
          画面上部
          ===================================== */}
      <header className={styles.header}>
        <h1 className={styles.title}>
          作業登録
        </h1>

        <p className={styles.description}>
          {process.processName}
          に新しい作業を登録します。
        </p>
      </header>

      {/* =====================================
          作業登録フォーム
          ===================================== */}
      <form
        action={createTaskAction}
        className={styles.form}
      >

        {/* 登録対象の工程ID */}
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
            maxLength={200}
            required
            placeholder="例：材料準備"
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
          />

          <p className={styles.helpText}>
            予定日は未入力でも登録できます。
          </p>
        </div>

        {/* =================================
            初期ステータス
            ================================= */}
        <div className={styles.formGroup}>
          <span className={styles.label}>
            ステータス
          </span>

          <p className={styles.helpText}>
            新規登録時は「未着手」で登録されます。
          </p>
        </div>

        {/* =================================
            操作ボタン
            ================================= */}
        <div className={styles.formActions}>
          <button
            type="submit"
            className={styles.primaryButton}
          >
            登録
          </button>

          <Link
            href={
              `/processes/${process.id}/tasks`
            }
            className={styles.secondaryButton}
          >
            キャンセル
          </Link>
        </div>

      </form>
    </main>
  );
}