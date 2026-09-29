"use client";

import { useEffect } from "react";
import {
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";

import { useRouter } from "next/navigation";

/**
 * ASP.NET CoreのSignalR Hubへの接続を確認するためのコンポーネント。
 *
 * 現時点では画面には何も表示せず、
 * ブラウザのコンソールで接続状態を確認する。
 */
export default function SignalRConnection() {
  
  // 現在表示しているNext.jsの画面を
  // 再取得するために使用する。
  const router = useRouter();

  // 環境変数からAPIのURLを取得する。
  // ローカル環境と本番環境で接続先を切り替えられるようにする。
  const apiUrl = process.env.NEXT_PUBLIC_API_URL;

  if (!apiUrl) {
    console.error(
      "NEXT_PUBLIC_API_URL が設定されていません。"
    );

    return;
  }

  useEffect(() => {

    // SignalRの接続情報を作成する。
    const connection = new HubConnectionBuilder()
      // ASP.NET Core側で設定したHubのURL
      .withUrl(`${apiUrl}/hubs/progress`, {
        // SignalR接続時にJWTを取得してBearer認証に使用する。
        accessTokenFactory: async () => {
          const response = await fetch(
            '/api/signalr-token',
            {
              cache: 'no-store',
            }
          );

          if (!response.ok) {
            throw new Error(
              'SignalR用アクセストークンを取得できませんでした。'
            );
          }

          const data = await response.json();

          return data.accessToken;
        },
      })

      // SignalRの通信状況をブラウザのコンソールに表示する。
      .configureLogging(LogLevel.Information)

      // 接続が一時的に切れた場合、自動再接続を行う。
      .withAutomaticReconnect()

      // HubConnectionを作成する。
      .build();

    // Taskの登録・更新・削除通知を受信する。
    connection.on("TasksChanged", () => {
      console.log("TasksChanged通知を受信");

      // 現在表示しているServer Componentを再取得する。
      router.refresh();
    });

    connection.onreconnecting((error) => {
      console.log("SignalR再接続中", error);
    });

    connection.onreconnected((connectionId) => {
      console.log(
        "SignalR再接続成功",
        connectionId
      );

      // 切断中に通知を取りこぼしている可能性があるので、
      // 最新状態を再取得する。
      router.refresh();
    });

    /**
     * SignalRへの接続を開始する。
     */
    const startConnection = async () => {
      try {
        await connection.start();

        console.log("SignalR接続成功");
      } catch (error) {
        console.error("SignalR接続失敗", error);
      }
    };

    startConnection();

    /**
     * コンポーネントが破棄されるときに
     * SignalR接続も終了する。
     */
    return () => {
      connection.stop();
    };
  }, []);

  // 今回は接続確認だけなので画面には何も表示しない。
  return null;
}