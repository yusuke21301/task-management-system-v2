"use server";

import { cookies } from "next/headers";
import { redirect } from "next/navigation";

/**
 * ログアウト処理。
 *
 * ログイン時に保存したaccess_token Cookieを削除して、
 * ログイン画面へ戻す。
 */
export async function logoutAction() {
  // Cookieを操作するために取得する
  const cookieStore = await cookies();

  // JWTを保存しているCookieを削除する
  cookieStore.delete("access_token");

  // ログイン画面へ移動する
  redirect("/login");
}