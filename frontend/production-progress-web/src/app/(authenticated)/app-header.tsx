import Link from "next/link";

import {
  logoutAction,
} from "@/app/actions/logout";

import styles from "./app-header.module.css";

/**
 * ログイン後の画面で共通表示するヘッダー。
 *
 * トップ・工程進捗へのナビゲーションと
 * ログアウト機能をまとめる。
 */
export default function AppHeader() {
  return (
    <header className={styles.header}>

      {/* =====================================
          システム名
          ===================================== */}
      <div className={styles.inner}>

        <Link
          href="/"
          className={styles.logo}
        >
          工程進捗管理システム
        </Link>

        {/* ===================================
            ナビゲーション
            =================================== */}
        <nav className={styles.nav}>

          <Link
            href="/"
            className={styles.navLink}
          >
            ダッシュボード
          </Link>

          <Link
            href="/processes"
            className={styles.navLink}
          >
            工程進捗
          </Link>

          {/* =================================
              ログアウト
              ================================= */}
          <form action={logoutAction}>
            <button
              type="submit"
              className={styles.logoutButton}
            >
              ログアウト
            </button>
          </form>

        </nav>

      </div>

    </header>
  );
}