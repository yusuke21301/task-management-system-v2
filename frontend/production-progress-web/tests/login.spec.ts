import { test, expect } from '@playwright/test';

test('adminでログインできる', async ({ page }) => {
  // =========================================
  // Arrange
  // ログイン画面を開く
  // =========================================

  await page.goto('/login');

  // =========================================
  // Act
  // ログイン情報を入力する
  // =========================================

  await page.getByLabel('ユーザー名').fill(
    process.env.E2E_USERNAME ?? 'admin'
  );

  await page.getByLabel('パスワード').fill(
    process.env.E2E_PASSWORD ?? ''
  );

  // ログインボタンをクリックする
  await page
    .getByRole('button', { name: 'ログイン' })
    .click();

  // =========================================
  // Assert
  // ログイン画面から遷移したことを確認する
  // =========================================

  await expect(page).not.toHaveURL(/\/login$/);
});