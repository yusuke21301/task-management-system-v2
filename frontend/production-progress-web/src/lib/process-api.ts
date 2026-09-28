import { cookies } from "next/headers";

/**
 * 工程情報
 *
 * APIのProcessDtoに対応する型。
 */
export type ProcessDto = {
  id: number;
  processName: string;
  displayOrder: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
};

/**
 * 工程登録時にAPIへ送信するデータ
 */
export type CreateProcessRequest = {
  processName: string;
  displayOrder: number;
  isActive: boolean;
};

/**
 * 工程更新時にAPIへ送信するデータ
 */
export type UpdateProcessRequest = {
  processName: string;
  displayOrder: number;
  isActive: boolean;
};

/**
 * CookieからJWTを取得する。
 *
 * ログイン時にaccess_tokenという名前で
 * HttpOnly Cookieへ保存している。
 */
async function getAccessToken(): Promise<string> {
  const cookieStore = await cookies();

  const accessToken = cookieStore.get("access_token")?.value;

  if (!accessToken) {
    throw new Error("アクセストークンがありません。");
  }

  return accessToken;
}

/**
 * 工程一覧を取得する。
 */
export async function getProcesses(): Promise<ProcessDto[]> {
  const accessToken = await getAccessToken();

  const apiBaseUrl = process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error("API_BASE_URLが設定されていません。");
  }

  const response = await fetch(`${apiBaseUrl}/api/Processes`, {
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },

    // 毎回APIから最新データを取得する
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("工程一覧の取得に失敗しました。");
  }

  return response.json();
}

/**
 * 指定したIDの工程を1件取得する。
 *
 * 編集画面の初期表示で使用する。
 */
export async function getProcess(id: number): Promise<ProcessDto> {
  const accessToken = await getAccessToken();

  const apiBaseUrl = process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error("API_BASE_URLが設定されていません。");
  }

  const response = await fetch(`${apiBaseUrl}/api/Processes/${id}`, {
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },

    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("工程情報の取得に失敗しました。");
  }

  return response.json();
}

/**
 * 工程を新規登録する。
 */
export async function createProcess(
  request: CreateProcessRequest
): Promise<ProcessDto> {
  const accessToken = await getAccessToken();

  const apiBaseUrl = process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error("API_BASE_URLが設定されていません。");
  }

  const response = await fetch(`${apiBaseUrl}/api/Processes`, {
    method: "POST",

    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${accessToken}`,
    },

    body: JSON.stringify(request),
  });

  if (!response.ok) {
    throw new Error("工程の登録に失敗しました。");
  }

  return response.json();
}

/**
 * 工程情報を更新する。
 */
export async function updateProcess(
  id: number,
  request: UpdateProcessRequest
): Promise<void> {
  const accessToken = await getAccessToken();

  const apiBaseUrl = process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error("API_BASE_URLが設定されていません。");
  }

  const response = await fetch(`${apiBaseUrl}/api/Processes/${id}`, {
    method: "PUT",

    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${accessToken}`,
    },

    body: JSON.stringify(request),
  });

  if (!response.ok) {
    throw new Error("工程の更新に失敗しました。");
  }
}