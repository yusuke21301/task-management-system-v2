import styles from "./loading.module.css";

/**
 * ページのデータ取得中に表示する画面。
 *
 * Server Componentのレンダリングや
 * APIからのデータ取得を待っている間に表示される。
 */
export default function Loading() {
  return (
    <main className={styles.container}>
      <div className={styles.content}>

        {/* 読み込み中を表すアニメーション */}
        <div className={styles.spinner} />

        <p className={styles.message}>
          データを読み込んでいます...
        </p>

      </div>
    </main>
  );
}