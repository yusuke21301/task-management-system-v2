import LoginForm from "./login-form";
import styles from "./login.module.css";

/**
 * ログインページ
 */
export default function LoginPage() {
  return (
    <main className={styles.container}>

      <div className={styles.loginCard}>

        <h1 className={styles.title}>
          ログイン
        </h1>

        {/* 実際の入力フォーム */}
        <LoginForm />

      </div>

    </main>
  );
}