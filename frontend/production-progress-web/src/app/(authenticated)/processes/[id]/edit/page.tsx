import Link from "next/link";
import { notFound } from "next/navigation";

import { getProcess } from "@/lib/process-api";
import { updateProcessAction } from "../../actions";

import styles from "../../processes.module.css";

/**
 * URLパラメータ
 *
 * /processes/1/edit
 *              ↑
 *              id
 */
type Props = {
  params: Promise<{
    id: string;
  }>;
};

/**
 * 工程編集画面
 */
export default async function EditProcessPage({ params }: Props) {
  // ---------------------------------------------------------
  // URLから工程IDを取得
  // ---------------------------------------------------------
  const { id } = await params;

  const processId = Number(id);

  // 数値として解釈できない場合は404にする
  if (Number.isNaN(processId)) {
    notFound();
  }

  // ---------------------------------------------------------
  // APIから現在の工程情報を取得
  // ---------------------------------------------------------
  let process;

  try {
    process = await getProcess(processId);
  } catch {
    // 対象工程が存在しない場合などは404画面を表示
    notFound();
  }

  return (
    <main className={styles.container}>
      {/* =========================================
          画面タイトル
          ========================================= */}
      <div className={styles.header}>
        <h1 className={styles.title}>工程編集</h1>

        <p className={styles.description}>
          工程情報を変更します。
        </p>
      </div>

      {/* =========================================
          工程編集フォーム
          ========================================= */}
      <form
        action={updateProcessAction}
        className={styles.form}
      >
        {/* 更新対象の工程ID */}
        <input
          type="hidden"
          name="id"
          value={process.id}
        />

        {/* 工程名 */}
        <div className={styles.formGroup}>
          <label
            htmlFor="processName"
            className={styles.label}
          >
            工程名
          </label>

          <input
            id="processName"
            name="processName"
            type="text"
            className={styles.input}
            defaultValue={process.processName}
            required
            maxLength={100}
          />
        </div>

        {/* 表示順 */}
        <div className={styles.formGroup}>
          <label
            htmlFor="displayOrder"
            className={styles.label}
          >
            表示順
          </label>

          <input
            id="displayOrder"
            name="displayOrder"
            type="number"
            className={styles.input}
            defaultValue={process.displayOrder}
            min={0}
            required
          />

          <p className={styles.helpText}>
            数字が小さい工程から順番に表示します。
          </p>
        </div>

        {/* 有効 / 無効 */}
        <div className={styles.formGroup}>
          <label className={styles.checkboxLabel}>
            <input
              name="isActive"
              type="checkbox"
              defaultChecked={process.isActive}
            />

            有効
          </label>
        </div>

        {/* =========================================
            操作ボタン
            ========================================= */}
        <div className={styles.formActions}>
          <button
            type="submit"
            className={styles.primaryButton}
          >
            更新
          </button>

          <Link
            href="/processes"
            className={styles.secondaryButton}
          >
            キャンセル
          </Link>
        </div>
      </form>
    </main>
  );
}