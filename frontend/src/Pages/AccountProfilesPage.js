import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import useAxiosPrivate from "../Hooks/useAxiosPrivate";
import Profile from "../Components/Profile";
import styles from "../Styles/accountProfilesPage.module.css";

function AccountProfilesPage() {
  const [profiles, setProfiles] = useState([]);
  const [error, setError] = useState("");

  const axiosPrivate = useAxiosPrivate();
  const navigate = useNavigate();

  const fetchProfiles = useCallback(async () => {
    try {
      const response = await axiosPrivate.get("Profile/all");
      setProfiles(response.data.profiles || []);
    } catch (err) {
      console.error(err);
      setError("Unable to load profiles. Please try again.");
    }
  }, [axiosPrivate]);

  useEffect(() => {
    fetchProfiles();
  }, [fetchProfiles]);

  return (
    <div className={styles.accountProfilesPage}>
      <header className={styles.profilesHeader}>
        <div>
          <h1>Profiles</h1>
          <p>
            View the profiles on this account and edit profile details for each
            user.
          </p>
        </div>
      </header>

      {error && <div className={styles.profileError}>{error}</div>}

      <div className={styles.profilesGrid}>
        {profiles.map((profile) => (
          <article key={profile.id} className={styles.profileCard}>
            <div className={styles.profileCardTop}>
              <Profile profile={profile} showEdit={false} showName={false} />
            </div>
            <div className={styles.profileCardBody}>
              <div className={styles.profileName}>{profile.name}</div>
              <div className={styles.profileMeta}>
                {profile.isKidsProfile ? "Kids profile" : "Standard profile"}
              </div>
            </div>
            <button
              className={styles.editButton}
              onClick={() => navigate(`/account/profiles/${profile.id}/edit`)}
            >
              Edit profile
            </button>
          </article>
        ))}
      </div>
    </div>
  );
}

export default AccountProfilesPage;
