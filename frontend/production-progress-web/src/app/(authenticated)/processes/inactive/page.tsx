import Link from "next/link";

import {
  getProcesses,
} from "@/lib/process-api";

import {
  reactivateProcessAction,
} from "../actions";

import styles from "../processes.module.css";

/**
 * 無効工程管理画面。
 *
 * isActive = false の工程だけを表示し、
 * 再有効化できるようにする。
 */
export default async function InactiveProcessesPage() {
  // ---------------------------------------------------------
  // 工程一覧を取得
  // ---------------------------------------------------------
  const processes =
    await getProcesses();

  // ---------------------------------------------------------
  // 無効な工程だけを抽出
  // ---------------------------------------------------------
  const inactiveProcesses =
    processes.filter(
      (process) =>
        !process.isActive
    );

  return (
    <main className={styles.container}>

      {/* =====================================
          画面上部
          ===================================== */}
      <header className={styles.header}>
        <h1 className={styles.title}>
          無効工程
        </h1>

        <p className={styles.description}>
          無効化された工程の確認と
          再有効化ができます。
        </p>
      </header>

      {/* =====================================
          無効工程一覧
          ===================================== */}
      {inactiveProcesses.length === 0 ? (
        <div className={styles.emptyMessage}>
          無効化されている工程はありません。
        </div>
      ) : (
        <div
          className={
            styles.processTableWrapper
          }
        >
          <table
            className={
              styles.processTable
            }
          >
            <thead>
              <tr>
                <th>ID</th>

                <th>
                  工程名
                </th>

                <th>
                  表示順
                </th>

                <th>
                  操作
                </th>
              </tr>
            </thead>

            <tbody>
              {inactiveProcesses.map(
                (process) => (
                  <tr
                    key={process.id}
                  >
                    {/* ID */}
                    <td>
                      {process.id}
                    </td>

                    {/* 工程名 */}
                    <td>
                      {process.processName}
                    </td>

                    {/* 表示順 */}
                    <td>
                      {process.displayOrder}
                    </td>

                    {/* 操作 */}
                    <td>
                      <div
                        className={
                          styles.inactiveActions
                        }
                      >
                        {/* 工程編集 */}
                        <Link
                          href={
                            `/processes/${process.id}/edit`
                          }
                          className={
                            styles.editLink
                          }
                        >
                          編集
                        </Link>

                        {/* 再有効化 */}
                        <form
                          action={
                            reactivateProcessAction
                          }
                        >
                          <input
                            type="hidden"
                            name="processId"
                            value={
                              process.id
                            }
                          />

                          <button
                            type="submit"
                            className={
                              styles.reactivateButton
                            }
                          >
                            再有効化
                          </button>
                        </form>
                      </div>
                    </td>
                  </tr>
                )
              )}
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