import { NavLink, Outlet, useNavigate } from "react-router-dom";
import {
  AiOutlineHome,
  AiOutlineCreditCard,
  AiOutlineLock,
  AiOutlineDesktop,
  AiOutlineUser,
} from "react-icons/ai";
import styles from "../Styles/accountLayout.module.css";

const navItems = [
  { to: "", label: "Overview", icon: <AiOutlineHome /> },
  { to: "membership", label: "Membership", icon: <AiOutlineCreditCard /> },
  { to: "security", label: "Security", icon: <AiOutlineLock /> },
  { to: "devices", label: "Devices", icon: <AiOutlineDesktop /> },
  { to: "profiles", label: "Profiles", icon: <AiOutlineUser /> },
];

function AccountLayout() {
  const navigate = useNavigate();

  return (
    <div className={styles.accountLayout}>
      <div className={styles.toolbar}>
        <button className={styles.backButton} onClick={() => navigate("/home")}>
          ← Back to Netflix
        </button>
      </div>
      <div className={styles.container}>
        <aside className={styles.sidebar}>
          <div className={styles.sidebarHeading}>
            <span>Account</span>
          </div>
          <nav className={styles.navList}>
            {navItems.map(({ to, label, icon }) => (
              <NavLink
                key={label}
                to={to}
                end={to === ""}
                className={({ isActive }) =>
                  `${styles.navLink} ${isActive ? styles.active : ""}`
                }
              >
                <span className={styles.icon}>{icon}</span>
                <span>{label}</span>
              </NavLink>
            ))}
          </nav>
        </aside>
        <main className={styles.mainContent}>
          <Outlet />
        </main>
      </div>
    </div>
  );
}

export default AccountLayout;
