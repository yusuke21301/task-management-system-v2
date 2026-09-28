import Link from "next/link";
import { createProcessAction } from "../actions";
import styles from "../processes.module.css";

/**
 * 工程新規登録画面
 */
export default function NewProcessPage() {
  return (
    <main className={styles.container}>
      {/* =========================================
          画面タイトル
          ========================================= */}
      <div className={styles.header}>
        <h1 className={styles.title}>工程登録</h1>

        <p className={styles.description}>
          新しい工程を登録します。
        </p>
      </div>

      {/* =========================================
          工程登録フォーム
          ========================================= */}
      <form action={createProcessAction} className={styles.form}>
        {/* 工程名 */}
        <div className={styles.formGroup}>
          <label htmlFor="processName" className={styles.label}>
            工程名
          </label>

          <input
            id="processName"
            name="processName"
            type="text"
            className={styles.input}
            required
            maxLength={100}
            placeholder="例：成形工程"
          />
        </div>

        {/* 表示順 */}
        <div className={styles.formGroup}>
          <label htmlFor="displayOrder" className={styles.label}>
            表示順
          </label>

          <input
            id="displayOrder"
            name="displayOrder"
            type="number"
            className={styles.input}
            defaultValue={0}
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
              defaultChecked
            />

            有効
          </label>
        </div>

        {/* =========================================
            操作ボタン
            ========================================= */}
        <div className={styles.formActions}>
          <button type="submit" className={styles.primaryButton}>
            登録
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