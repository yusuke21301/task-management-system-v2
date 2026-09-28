import AppHeader from "./app-header";

/**
 * ログイン後画面の共通レイアウト。
 *
 * このフォルダ配下のページには、
 * AppHeaderが自動的に表示される。
 */
export default function AuthenticatedLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <>
      {/* 共通ヘッダー */}
      <AppHeader />

      {/* 各画面 */}
      {children}
    </>
  );
}