import styles from "../Styles/accountPage.module.css";

function AccountSectionPage({ title, children }) {
  return (
    <div className={styles.accountPage}>
      <div className={styles.sectionCard}>
        <div className={styles.sectionHeader}>
          <h1>{title}</h1>
        </div>
        <div className={styles.sectionBody}>{children}</div>
      </div>
    </div>
  );
}

export default AccountSectionPage;
