import { cookies } from "next/headers";

/**
 * ASP.NET Core側のWorkTaskStatusに対応する。
 */
export type WorkTaskStatus =
  | "NotStarted"
  | "InProgress"
  | "Completed";

/**
 * ASP.NET Core側のTaskDtoに対応する型。
 */
export type TaskDto = {
  id: number;
  taskName: string;
  status: WorkTaskStatus;
  plannedDate: string | null;
  createdAt: string;
  updatedAt: string;

  // 所属している工程
  processId: number;
  processName: string | null;
};

/**
 * GET /api/Tasks のページングレスポンス。
 *
 * ASP.NET Core側では作業一覧を
 * itemsの中に格納して返している。
 */
type TaskPagedResponse = {
  items: TaskDto[];

  page: number;
  pageSize: number;

  totalCount: number;
  totalPages: number;
};

/**
 * 工程ごとの作業進捗。
 */
export type TaskProgress = {
  total: number;
  notStarted: number;
  inProgress: number;
  completed: number;
};

/**
 * 作業登録時にAPIへ送信するデータ。
 */
export type CreateTaskRequest = {
  taskName: string;
  processId: number;
  status: WorkTaskStatus;
  plannedDate: string | null;
};

/**
 * 作業を新規登録する。
 */
export async function createTask(
  request: CreateTaskRequest
): Promise<TaskDto> {
  const cookieStore =
    await cookies();

  const accessToken =
    cookieStore.get("access_token")?.value;

  if (!accessToken) {
    throw new Error(
      "アクセストークンがありません。"
    );
  }

  const apiBaseUrl =
    process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error(
      "API_BASE_URLが設定されていません。"
    );
  }

  const response =
    await fetch(
      `${apiBaseUrl}/api/Tasks`,
      {
        method: "POST",

        headers: {
          "Content-Type":
            "application/json",

          Authorization:
            `Bearer ${accessToken}`,
        },

        body: JSON.stringify(request),
      }
    );

  if (!response.ok) {
    throw new Error(
      "作業の登録に失敗しました。"
    );
  }

  return response.json();
}

/**
 * 指定した工程に所属する作業一覧を取得する。
 *
 * @param processId 工程ID
 */
export async function getTasksByProcessId(
  processId: number
): Promise<TaskDto[]> {
  // -----------------------------------------------
  // CookieからJWTを取得
  // -----------------------------------------------

  const cookieStore = await cookies();

  const accessToken =
    cookieStore.get("access_token")?.value;

  if (!accessToken) {
    throw new Error(
      "アクセストークンがありません。"
    );
  }

  // -----------------------------------------------
  // API接続先
  // -----------------------------------------------

  const apiBaseUrl = process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error(
      "API_BASE_URLが設定されていません。"
    );
  }

  // -----------------------------------------------
  // ASP.NET Core APIを呼び出す
  // -----------------------------------------------
  //
  // 例：
  // GET /api/Tasks?processId=2
  //
  const response = await fetch(
    `${apiBaseUrl}/api/Tasks?processId=${processId}`,
    {
      method: "GET",

      headers: {
        // JWTをASP.NET Coreへ送信する
        Authorization: `Bearer ${accessToken}`,
      },

      // 常に最新の作業一覧を取得する
      cache: "no-store",
    }
  );

  // -----------------------------------------------
  // HTTPエラー
  // -----------------------------------------------

  if (!response.ok) {
    throw new Error(
      `作業一覧の取得に失敗しました。status=${response.status}`
    );
  }

  // APIからページングされたレスポンスを取得する
    const data =
    (await response.json()) as TaskPagedResponse;

    // 実際の作業一覧はitemsに入っている
    return data.items;
}

/**
 * 指定した工程の全作業を取得して、
 * ステータス別の件数を集計する。
 *
 * GET /api/Tasks はページングされているため、
 * 必要に応じて2ページ目以降も取得する。
 */
export async function getTaskProgressByProcessId(
  processId: number
): Promise<TaskProgress> {
  // -----------------------------------------------
  // JWT取得
  // -----------------------------------------------

  const cookieStore = await cookies();

  const accessToken =
    cookieStore.get("access_token")?.value;

  if (!accessToken) {
    throw new Error(
      "アクセストークンがありません。"
    );
  }

  // -----------------------------------------------
  // API URL取得
  // -----------------------------------------------

  const apiBaseUrl =
    process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error(
      "API_BASE_URLが設定されていません。"
    );
  }

  // 1回に取得する件数
  const pageSize = 100;

  /**
   * 指定ページを取得する内部関数。
   */
  async function fetchPage(
    page: number
  ): Promise<TaskPagedResponse> {
    const response = await fetch(
      `${apiBaseUrl}/api/Tasks?processId=${processId}&page=${page}&pageSize=${pageSize}`,
      {
        method: "GET",

        headers: {
          Authorization:
            `Bearer ${accessToken}`,
        },

        cache: "no-store",
      }
    );

    if (!response.ok) {
      throw new Error(
        `作業進捗の取得に失敗しました。status=${response.status}`
      );
    }

    return (
      await response.json()
    ) as TaskPagedResponse;
  }

  // -----------------------------------------------
  // まず1ページ目を取得
  // -----------------------------------------------

  const firstPage =
    await fetchPage(1);

  // 全作業をここへ格納する
  const allTasks = [
    ...firstPage.items,
  ];

  // -----------------------------------------------
  // 2ページ目以降が存在する場合
  // -----------------------------------------------

  if (firstPage.totalPages > 1) {
    const requests = [];

    for (
      let page = 2;
      page <= firstPage.totalPages;
      page++
    ) {
      requests.push(
        fetchPage(page)
      );
    }

    // 複数ページを並行取得する
    const remainingPages =
      await Promise.all(requests);

    for (
      const page of remainingPages
    ) {
      allTasks.push(
        ...page.items
      );
    }
  }

  // -----------------------------------------------
  // ステータス別に集計
  // -----------------------------------------------

  const notStarted =
    allTasks.filter(
      (task) =>
        task.status === "NotStarted"
    ).length;

  const inProgress =
    allTasks.filter(
      (task) =>
        task.status === "InProgress"
    ).length;

  const completed =
    allTasks.filter(
      (task) =>
        task.status === "Completed"
    ).length;

  return {
    total: allTasks.length,
    notStarted,
    inProgress,
    completed,
  };
}

/**
 * 作業更新時にAPIへ送信するデータ。
 */
export type UpdateTaskRequest = {
  taskName: string;
  processId: number;
  status: WorkTaskStatus;
  plannedDate: string | null;
};

/**
 * 指定したIDの作業を1件取得する。
 *
 * 作業編集画面の初期表示で使用する。
 */
export async function getTask(
  id: number
): Promise<TaskDto> {
  const cookieStore =
    await cookies();

  const accessToken =
    cookieStore.get("access_token")?.value;

  if (!accessToken) {
    throw new Error(
      "アクセストークンがありません。"
    );
  }

  const apiBaseUrl =
    process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error(
      "API_BASE_URLが設定されていません。"
    );
  }

  const response =
    await fetch(
      `${apiBaseUrl}/api/Tasks/${id}`,
      {
        headers: {
          Authorization:
            `Bearer ${accessToken}`,
        },

        cache: "no-store",
      }
    );

  if (!response.ok) {
    throw new Error(
      "作業情報の取得に失敗しました。"
    );
  }

  return response.json();
}

/**
 * 作業情報を更新する。
 */
export async function updateTask(
  id: number,
  request: UpdateTaskRequest
): Promise<void> {
  const cookieStore =
    await cookies();

  const accessToken =
    cookieStore.get("access_token")?.value;

  if (!accessToken) {
    throw new Error(
      "アクセストークンがありません。"
    );
  }

  const apiBaseUrl =
    process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error(
      "API_BASE_URLが設定されていません。"
    );
  }

  const response =
    await fetch(
      `${apiBaseUrl}/api/Tasks/${id}`,
      {
        method: "PUT",

        headers: {
          "Content-Type":
            "application/json",

          Authorization:
            `Bearer ${accessToken}`,
        },

        body: JSON.stringify(request),
      }
    );

  if (!response.ok) {
    throw new Error(
      "作業の更新に失敗しました。"
    );
  }
}

/**
 * 指定した作業を削除する。
 */
export async function deleteTask(
  id: number
): Promise<void> {
  const cookieStore =
    await cookies();

  const accessToken =
    cookieStore.get("access_token")?.value;

  if (!accessToken) {
    throw new Error(
      "アクセストークンがありません。"
    );
  }

  const apiBaseUrl =
    process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error(
      "API_BASE_URLが設定されていません。"
    );
  }

  const response =
    await fetch(
      `${apiBaseUrl}/api/Tasks/${id}`,
      {
        method: "DELETE",

        headers: {
          Authorization:
            `Bearer ${accessToken}`,
        },
      }
    );

  if (!response.ok) {
    throw new Error(
      `作業の削除に失敗しました。status=${response.status}`
    );
  }
}