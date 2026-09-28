"use client";

import {
  useState,
} from "react";

import styles from "./tasks.module.css";

type Props = {
  taskId: number;
  processId: number;

  // 削除対象が分かるように
  // 作業名も受け取る
  taskName: string;

  // Server Action
  deleteAction: (
    formData: FormData
  ) => void | Promise<void>;
};

/**
 * 作業削除ボタン。
 *
 * 削除前に確認ダイアログを表示するため、
 * Client Componentとして実装する。
 */
export default function DeleteTaskButton({
  taskId,
  processId,
  taskName,
  deleteAction,
}: Props) {
  const [
    isOpen,
    setIsOpen,
  ] = useState(false);

  return (
    <>
      {/* =====================================
          削除ボタン
          ===================================== */}
      <button
        type="button"
        className={styles.deleteButton}
        onClick={() => {
          setIsOpen(true);
        }}
      >
        削除
      </button>

      {/* =====================================
          削除確認ダイアログ
          ===================================== */}
      {isOpen && (
        <div
          className={
            styles.dialogOverlay
          }
        >
          <div
            className={
              styles.dialog
            }
          >
            <h2
              className={
                styles.dialogTitle
              }
            >
              作業を削除
            </h2>

            <p
              className={
                styles.dialogMessage
              }
            >
              「{taskName}」を削除しますか？
            </p>

            <p
              className={
                styles.dialogWarning
              }
            >
              この操作は元に戻せません。
            </p>

            <div
              className={
                styles.dialogActions
              }
            >
              {/* キャンセル */}
              <button
                type="button"
                className={
                  styles.secondaryButton
                }
                onClick={() => {
                  setIsOpen(false);
                }}
              >
                キャンセル
              </button>

              {/* 実際の削除処理 */}
              <form
                action={deleteAction}
              >
                <input
                  type="hidden"
                  name="taskId"
                  value={taskId}
                />

                <input
                  type="hidden"
                  name="processId"
                  value={processId}
                />

                <button
                  type="submit"
                  className={
                    styles.deleteButton
                  }
                >
                  削除する
                </button>
              </form>
            </div>
          </div>
        </div>
      )}
    </>
  );
}