"use client";

import { useActionState } from "react";
import {
  loginAction,
  type LoginState,
} from "./actions";

import styles from "./login.module.css";

/**
 * Server Actionの初期状態。
 *
 * 最初はエラーがないためnull。
 */
const initialState: LoginState = {
  error: null,
};

/**
 * ログインフォーム。
 *
 * 入力操作やログイン中表示が必要なので
 * Client Componentとして作成する。
 */
export default function LoginForm() {

  // --------------------------------------------------
  // Server Actionと画面状態を接続する
  // --------------------------------------------------
  //
  // state
  //   Server Actionから返ってきた状態
  //
  // formAction
  //   formのactionに指定する処理
  //
  // pending
  //   Server Action実行中ならtrue
  //
  const [state, formAction, pending] =
    useActionState(
      loginAction,
      initialState
    );

  return (
    <form action={formAction}>

      {/* ユーザー名 */}
      <div className={styles.formGroup}>
        <label
          className={styles.label}
          htmlFor="username"
        >
          ユーザー名
        </label>

        <input
          className={styles.input}
          id="username"
          name="username"
          type="text"
          autoComplete="username"
        />
      </div>

      {/* パスワード */}
      <div className={styles.formGroup}>
        <label
          className={styles.label}
          htmlFor="password"
        >
          パスワード
        </label>

        <input
          className={styles.input}
          id="password"
          name="password"
          type="password"
          autoComplete="current-password"
        />
      </div>

      {/* ログイン失敗時だけエラーを表示 */}
      {state.error && (
        <p className={styles.error} role="alert">
          {state.error}
        </p>
      )}

      {/* ログインボタン */}
      <button
        className={styles.loginButton}
        type="submit"

        // ログイン処理中は連打できないようにする
        disabled={pending}
      >
        {pending
          ? "ログイン中..."
          : "ログイン"}
      </button>

    </form>
  );
}