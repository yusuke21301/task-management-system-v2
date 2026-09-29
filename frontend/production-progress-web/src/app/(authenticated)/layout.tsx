import AppHeader from "./app-header";
import SignalRConnection from "../../components/SignalRConnection";

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

      {/* ASP.NET CoreのSignalR Hubへ接続する */}
        <SignalRConnection />
      {/* 各画面 */}
      {children}
    </>
  );
}