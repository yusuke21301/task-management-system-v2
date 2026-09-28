"use server";

import { redirect } from "next/navigation";
import { revalidatePath } from "next/cache";
import {
  createProcess,
  updateProcess,
  getProcess,
} from "@/lib/process-api";

/**
 * 工程登録
 */
export async function createProcessAction(formData: FormData) {
  const processName =
    formData.get("processName")?.toString().trim() ?? "";

  const displayOrderText =
    formData.get("displayOrder")?.toString() ?? "0";

  const isActive = formData.get("isActive") === "on";

  const displayOrder = Number(displayOrderText);

  if (!processName) {
    throw new Error("工程名を入力してください。");
  }

  if (Number.isNaN(displayOrder)) {
    throw new Error("表示順には数値を入力してください。");
  }

  await createProcess({
    processName,
    displayOrder,
    isActive,
  });

  revalidatePath("/processes");

  redirect("/processes");
}

/**
 * 工程更新
 *
 * 編集画面から送信された内容を使用して、
 * 指定された工程を更新する。
 */
export async function updateProcessAction(formData: FormData) {
  // ---------------------------------------------------------
  // hidden項目から工程IDを取得
  // ---------------------------------------------------------
  const idText = formData.get("id")?.toString() ?? "";

  const id = Number(idText);

  if (Number.isNaN(id)) {
    throw new Error("工程IDが不正です。");
  }

  // ---------------------------------------------------------
  // フォーム入力値を取得
  // ---------------------------------------------------------
  const processName =
    formData.get("processName")?.toString().trim() ?? "";

  const displayOrderText =
    formData.get("displayOrder")?.toString() ?? "0";

  const isActive = formData.get("isActive") === "on";

  const displayOrder = Number(displayOrderText);

  // ---------------------------------------------------------
  // 入力チェック
  // ---------------------------------------------------------
  if (!processName) {
    throw new Error("工程名を入力してください。");
  }

  if (Number.isNaN(displayOrder)) {
    throw new Error("表示順には数値を入力してください。");
  }

  // ---------------------------------------------------------
  // ASP.NET Core APIへ更新要求を送信
  // ---------------------------------------------------------
  await updateProcess(id, {
    processName,
    displayOrder,
    isActive,
  });

  // ---------------------------------------------------------
  // 一覧画面のキャッシュを更新
  // ---------------------------------------------------------
  revalidatePath("/processes");

  // ---------------------------------------------------------
  // 更新後は工程一覧へ戻る
  // ---------------------------------------------------------
  redirect("/processes");
}

/**
 * 無効化されている工程を再有効化する。
 */
export async function reactivateProcessAction(
  formData: FormData
) {
  // ---------------------------------------------------------
  // 工程IDを取得
  // ---------------------------------------------------------
  const processId =
    Number(
      formData.get("processId")
    );

  // ---------------------------------------------------------
  // 工程IDをチェック
  // ---------------------------------------------------------
  if (
    !Number.isInteger(processId) ||
    processId <= 0
  ) {
    throw new Error(
      "工程IDが不正です。"
    );
  }

  // ---------------------------------------------------------
  // 現在の工程情報を取得
  // ---------------------------------------------------------
  //
  // PUT APIでは工程名や表示順も必要なので、
  // 現在値をAPIから取得する。
  //
  const process =
    await getProcess(processId);

  // ---------------------------------------------------------
  // isActiveだけtrueにして更新
  // ---------------------------------------------------------
  await updateProcess(
    processId,
    {
      processName:
        process.processName,

      displayOrder:
        process.displayOrder,

      isActive: true,
    }
  );

  // ---------------------------------------------------------
  // 工程一覧を再取得させる
  // ---------------------------------------------------------
  revalidatePath(
    "/processes"
  );

  // 無効工程一覧も再取得させる
  revalidatePath(
    "/processes/inactive"
  );
}