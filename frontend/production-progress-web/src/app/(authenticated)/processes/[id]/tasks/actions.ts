"use server";

import { cookies } from "next/headers";
import { revalidatePath } from "next/cache";

import {
  redirect,
} from "next/navigation";

import {
  updateTask,
  createTask,
  deleteTask,
} from "@/lib/task-api";

import type {
  WorkTaskStatus,
} from "@/lib/task-api";

/**
 * 作業ステータスを更新するServer Action。
 */
export async function updateTaskStatus(
  formData: FormData
) {
  // -----------------------------------------------
  // formから値を取得
  // -----------------------------------------------

  const taskId = Number(
    formData.get("taskId")
  );

  const processId = Number(
    formData.get("processId")
  );

  const taskName = String(
    formData.get("taskName") ?? ""
  );

  const status = String(
    formData.get("status") ?? ""
  ) as WorkTaskStatus;

  const plannedDateValue = String(
    formData.get("plannedDate") ?? ""
  );

  // 空文字の場合はnullにする
  const plannedDate =
    plannedDateValue === ""
      ? null
      : plannedDateValue;

  // -----------------------------------------------
  // JWTを取得
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

  const apiBaseUrl =
    process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error(
      "API_BASE_URLが設定されていません。"
    );
  }

  // -----------------------------------------------
  // ASP.NET Core APIで作業を更新
  // -----------------------------------------------

  const response = await fetch(
    `${apiBaseUrl}/api/Tasks/${taskId}`,
    {
      method: "PUT",

      headers: {
        "Content-Type": "application/json",

        // JWT認証
        Authorization:
          `Bearer ${accessToken}`,
      },

      body: JSON.stringify({
        // 現在の作業名は変更しない
        taskName,

        // 現在所属している工程もそのまま送る
        processId,

        // 今回変更する項目
        status,

        // 予定日も現在値を維持する
        plannedDate,
      }),
    }
  );

  // -----------------------------------------------
  // 更新失敗
  // -----------------------------------------------

  if (!response.ok) {
    const errorText =
      await response.text();

    console.error(
      "作業更新APIエラー",
      response.status,
      errorText
    );

    throw new Error(
      `作業の更新に失敗しました。status=${response.status}`
    );
  }

  // -----------------------------------------------
  // 作業一覧を再取得させる
  // -----------------------------------------------

  revalidatePath(
    `/processes/${processId}/tasks`
  );
}

/**
 * 作業を新規登録するServer Action。
 */
export async function createTaskAction(
  formData: FormData
) {
  // -----------------------------------------------
  // フォームから入力値を取得
  // -----------------------------------------------

  const processIdText =
    formData
      .get("processId")
      ?.toString() ?? "";

  const taskName =
    formData
      .get("taskName")
      ?.toString()
      .trim() ?? "";

  const plannedDateText =
    formData
      .get("plannedDate")
      ?.toString() ?? "";

  // -----------------------------------------------
  // 工程IDを数値へ変換
  // -----------------------------------------------

  const processId =
    Number(processIdText);

  if (Number.isNaN(processId)) {
    throw new Error(
      "工程IDが不正です。"
    );
  }

  // -----------------------------------------------
  // 作業名をチェック
  // -----------------------------------------------

  if (!taskName) {
    throw new Error(
      "作業名を入力してください。"
    );
  }

  // -----------------------------------------------
  // APIへ登録要求を送信
  // -----------------------------------------------
  //
  // 新規作業は「未着手」で登録する。
  //

  await createTask({
    taskName,
    processId,

    status: "NotStarted",

    // 日付未入力の場合はnullを送る
    plannedDate:
      plannedDateText === ""
        ? null
        : plannedDateText,
  });

  // -----------------------------------------------
  // 作業一覧を再取得させる
  // -----------------------------------------------

  revalidatePath(
    `/processes/${processId}/tasks`
  );

  // -----------------------------------------------
  // 登録後は作業一覧へ戻る
  // -----------------------------------------------

  redirect(
    `/processes/${processId}/tasks`
  );
}

/**
 * 作業情報を更新するServer Action。
 */
export async function updateTaskAction(
  formData: FormData
) {
  // -----------------------------------------------
  // フォームからIDを取得
  // -----------------------------------------------

  const taskId =
    Number(
      formData.get("taskId")
    );

  const processId =
    Number(
      formData.get("processId")
    );

  if (Number.isNaN(taskId)) {
    throw new Error(
      "作業IDが不正です。"
    );
  }

  if (Number.isNaN(processId)) {
    throw new Error(
      "工程IDが不正です。"
    );
  }

  // -----------------------------------------------
  // 入力値を取得
  // -----------------------------------------------

  const taskName =
    formData
      .get("taskName")
      ?.toString()
      .trim() ?? "";

  const status =
    formData
      .get("status")
      ?.toString() ?? "";

  const plannedDateText =
    formData
      .get("plannedDate")
      ?.toString() ?? "";

  // -----------------------------------------------
  // 入力チェック
  // -----------------------------------------------

  if (!taskName) {
    throw new Error(
      "作業名を入力してください。"
    );
  }

  if (
    status !== "NotStarted" &&
    status !== "InProgress" &&
    status !== "Completed"
  ) {
    throw new Error(
      "ステータスが不正です。"
    );
  }

  // -----------------------------------------------
  // APIへ更新要求を送信
  // -----------------------------------------------

  await updateTask(
    taskId,
    {
      taskName,
      processId,

      status,

      plannedDate:
        plannedDateText === ""
          ? null
          : plannedDateText,
    }
  );

  // -----------------------------------------------
  // 作業一覧を再取得させる
  // -----------------------------------------------

  revalidatePath(
    `/processes/${processId}/tasks`
  );

  // 工程進捗にも影響するので、
  // 工程一覧も再取得させる
  revalidatePath(
    "/processes"
  );

  // -----------------------------------------------
  // 作業一覧へ戻る
  // -----------------------------------------------

  redirect(
    `/processes/${processId}/tasks`
  );
}

/**
 * 作業を削除するServer Action。
 */
export async function deleteTaskAction(
  formData: FormData
) {
  // -----------------------------------------------
  // フォームからIDを取得
  // -----------------------------------------------
  const taskId =
    Number(
      formData.get("taskId")
    );

  const processId =
    Number(
      formData.get("processId")
    );

  // -----------------------------------------------
  // IDチェック
  // -----------------------------------------------
  if (
    !Number.isInteger(taskId) ||
    taskId <= 0
  ) {
    throw new Error(
      "作業IDが不正です。"
    );
  }

  if (
    !Number.isInteger(processId) ||
    processId <= 0
  ) {
    throw new Error(
      "工程IDが不正です。"
    );
  }

  // -----------------------------------------------
  // APIで削除
  // -----------------------------------------------
  await deleteTask(taskId);

  // -----------------------------------------------
  // 一覧を再取得
  // -----------------------------------------------
  revalidatePath(
    `/processes/${processId}/tasks`
  );

  // 工程進捗にも影響するので更新
  revalidatePath("/processes");
}