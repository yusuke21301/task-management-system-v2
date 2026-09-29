import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "工程進捗管理システム",
  description: "工程の進捗状況を管理するWebシステム",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="ja">
      <body>
        {children}
      </body>
    </html>
  );
}