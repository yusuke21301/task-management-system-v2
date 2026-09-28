import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

/**
 * ページへアクセスされる前に実行される処理。
 *
 * access_token Cookieの有無を確認して、
 * 未ログインユーザーが保護ページへ
 * アクセスできないようにする。
 */
export default function proxy(request: NextRequest) {
  // CookieからJWTを取得する
  const accessToken =
    request.cookies.get("access_token")?.value;

  // 現在アクセスしているパス
  const pathname = request.nextUrl.pathname;

  // ログイン画面かどうか
  const isLoginPage = pathname === "/login";

  // --------------------------------------------------
  // 未ログイン
  // --------------------------------------------------
  // access_tokenがない状態でトップページへアクセスしたら、
  // ログイン画面へ移動する。
  if (!accessToken && !isLoginPage) {
    return NextResponse.redirect(
      new URL("/login", request.url)
    );
  }

  // --------------------------------------------------
  // ログイン済み
  // --------------------------------------------------
  // ログイン済みでログイン画面へアクセスしたら、
  // トップページへ移動する。
  if (accessToken && isLoginPage) {
    return NextResponse.redirect(
      new URL("/", request.url)
    );
  }

  // それ以外は通常通り処理する
  return NextResponse.next();
}

/**
 * Proxyを実行するURL。
 */
export const config = {
  matcher: ["/", "/login", "/processes/:path*"],
};