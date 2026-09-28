"use server";

import { cookies } from "next/headers";
import { redirect } from "next/navigation";

/**
 * ASP.NET CoreのログインAPIから返されるデータ。
 *
 * C#側のLoginResponseに対応する。
 */
type LoginResponse = {
  userId: number;
  username: string;
  role: string;
  accessToken: string;
  expiresAt: string;
};

/**
 * ログイン画面へ返す状態。
 *
 * ログイン失敗時はerrorにメッセージを入れる。
 */
export type LoginState = {
  error: string | null;
};

/**
 * ログイン処理を行うServer Action。
 *
 * ブラウザから直接ASP.NET Coreを呼ぶのではなく、
 * Next.jsサーバーからASP.NET Core APIを呼び出す。
 */
export async function loginAction(
  _prevState: LoginState,
  formData: FormData
): Promise<LoginState> {

  // --------------------------------------------------
  // 入力値を取得
  // --------------------------------------------------

  const username = String(
    formData.get("username") ?? ""
  ).trim();

  const password = String(
    formData.get("password") ?? ""
  );

  // --------------------------------------------------
  // 簡単な入力チェック
  // --------------------------------------------------

  if (!username || !password) {
    return {
      error: "ユーザー名とパスワードを入力してください。",
    };
  }

  // .env.localからASP.NET Core APIのURLを取得する
  const apiBaseUrl = process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    return {
      error: "APIの接続先が設定されていません。",
    };
  }

  let response: Response;

  try {
    // --------------------------------------------------
    // ASP.NET CoreのログインAPIを呼び出す
    // --------------------------------------------------

    response = await fetch(
      `${apiBaseUrl}/api/Auth/login`,
      {
        method: "POST",

        headers: {
          "Content-Type": "application/json",
        },

        // C#側のLoginRequest
        // Username / Password に対応する
        body: JSON.stringify({
          username,
          password,
        }),
      }
    );
  } catch (error) {
    console.error("ログインAPIへの接続に失敗しました。", error);

    return {
      error: "APIサーバーに接続できませんでした。",
    };
  }

  // --------------------------------------------------
  // HTTPエラー処理
  // --------------------------------------------------

  if (!response.ok) {

    // ユーザー名またはパスワードが違う場合
    if (response.status === 401) {
      return {
        error: "ユーザー名またはパスワードが正しくありません。",
      };
    }

    console.error(
      `ログインAPIエラー: ${response.status}`
    );

    return {
      error: "ログイン処理でエラーが発生しました。",
    };
  }

  // --------------------------------------------------
  // APIのレスポンスを取得
  // --------------------------------------------------

  const data =
    (await response.json()) as LoginResponse;

  // --------------------------------------------------
  // JWTをCookieへ保存
  // --------------------------------------------------

  const cookieStore = await cookies();

  cookieStore.set(
    "access_token",
    data.accessToken,
    {
      // JavaScriptからCookieを読み取れないようにする
      httpOnly: true,

      // 同一サイトを基本とする
      sameSite: "lax",

      // 本番環境ではHTTPS通信だけに限定する
      secure: process.env.NODE_ENV === "production",

      // アプリ全体で使用する
      path: "/",

      // ASP.NET Core側で設定された
      // JWTの有効期限とCookieの有効期限を合わせる
      expires: new Date(data.expiresAt),
    }
  );

  // --------------------------------------------------
  // ログイン成功
  // --------------------------------------------------

  // 現時点ではトップページへ移動する
  redirect("/");
}