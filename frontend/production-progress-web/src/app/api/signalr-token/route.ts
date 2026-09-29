import { cookies } from 'next/headers';
import { NextResponse } from 'next/server';

/**
 * SignalR接続用のJWTを返す。
 *
 * access_tokenはHttpOnly Cookieなので、
 * ブラウザJavaScriptから直接読むことはできない。
 * Next.jsサーバー側でCookieを読み取って返す。
 */
export async function GET() {
  const cookieStore = await cookies();

  const accessToken =
    cookieStore.get('access_token')?.value;

  if (!accessToken) {
    return NextResponse.json(
      { message: '認証されていません。' },
      { status: 401 }
    );
  }

  return NextResponse.json(
    { accessToken },
    {
      headers: {
        // JWTをキャッシュさせない
        'Cache-Control': 'no-store',
      },
    }
  );
}