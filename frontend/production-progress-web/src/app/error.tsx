"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";

import styles from "./error.module.css";

export default function Error({
  error,
  reset,
}: {
  error: Error & {
    digest?: string;
  };

  reset: () => void;
}) {
  // App Routerを操作するために取得
  const router = useRouter();

  // エラー内容は開発者向けにログへ出力する
  useEffect(() => {
    console.error(
      "画面表示中にエラーが発生しました。",
      error
    );
  }, [error]);

  /**
   * 再試行処理
   *
   * 1. error.tsxのError Boundaryをリセット
   * 2. 現在のルートをサーバーから再取得
   */
  const handleRetry = () => {
    reset();

    router.refresh();
  };

  return (
    <main className={styles.container}>
      <div className={styles.card}>
        <h1 className={styles.title}>
          データを取得できませんでした
        </h1>

        <p className={styles.message}>
          サーバーとの通信中にエラーが発生しました。
        </p>

        <p className={styles.message}>
          しばらくしてから再度お試しください。
        </p>

        <button
          type="button"
          className={styles.retryButton}
          onClick={handleRetry}
        >
          再試行
        </button>
      </div>
    </main>
  );
}