import { Link } from "react-router-dom";
import styles from "../Styles/accountPage.module.css";

function AccountPage() {
  return (
    <div className={styles.accountPage}>
      <div className={styles.accountHeader}>
        <div>
          <h1>Account</h1>
          <p>
            Manage your plan, security, profile controls, and device access.
          </p>
        </div>
      </div>

      <div className={styles.overviewCard}>
        <h2>Membership details</h2>
        <p>
          Your plan and billing are managed here. Check your current
          subscription, payment method, and device access.
        </p>
      </div>

      <div className={styles.quickLinks}>
        <Link className={styles.linkButton} to="membership">
          Change Plan
        </Link>
        <Link className={styles.linkButton} to="security">
          Update Password
        </Link>
        <Link className={styles.linkButton} to="devices">
          Manage Devices
        </Link>
        <Link className={styles.linkButton} to="profiles">
          Manage Profiles
        </Link>
      </div>

      <div className={styles.profiles}>
        <Link className={styles.profileLink} to="profiles">
          Manage Profiles and permissions
        </Link>
      </div>
    </div>
  );
}

export default AccountPage;
