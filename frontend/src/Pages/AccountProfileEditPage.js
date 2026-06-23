import { useCallback, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import useAxiosPrivate from "../Hooks/useAxiosPrivate";
import styles from "../Styles/accountProfileEditPage.module.css";

const dicebearStyles = [
  "adventurer",
  "bottts",
  "lorelei",
  "personas",
  "micah",
  "big-ears",
  "thumbs",
];

function AccountProfileEditPage() {
  const { id } = useParams();
  const [profile, setProfile] = useState(null);
  const [name, setName] = useState("");
  const [isKidsProfile, setIsKidsProfile] = useState(false);
  const [avatarUrl, setAvatarUrl] = useState("");
  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);
  const [avatarHistory, setAvatarHistory] = useState([]);

  const axiosPrivate = useAxiosPrivate();
  const navigate = useNavigate();

  const fetchProfile = useCallback(async () => {
    try {
      const response = await axiosPrivate.get("Profile/all");
      const selected = response.data.profiles?.find(
        (item) => String(item.id) === String(id),
      );
      if (!selected) {
        setError("Unable to find that profile.");
        return;
      }
      setProfile(selected);
      setName(selected.name || "");
      setIsKidsProfile(Boolean(selected.isKidsProfile));
      const initialUrl =
        selected.avatarUrl || getAvatarUrl(String(selected.id), "adventurer");
      setAvatarUrl(initialUrl);
      setAvatarHistory([initialUrl]);
    } catch (err) {
      console.error(err);
      setError("Could not load profile details.");
    }
  }, [axiosPrivate, id]);

  useEffect(() => {
    fetchProfile();
  }, [fetchProfile]);

  function getAvatarUrl(seedValue, styleValue) {
    return `https://api.dicebear.com/10.x/${styleValue}/svg?seed=${encodeURIComponent(seedValue)}`;
  }

  function selectAvatar(style, seed) {
    const url = getAvatarUrl(seed, style);
    setAvatarUrl(url);
    setAvatarHistory((prev) => {
      const updated = [url, ...prev.filter((u) => u !== url)].slice(0, 4);
      return updated;
    });
  }

  async function handleSave(e) {
    if (e) e.preventDefault();
    if (!profile) return;
    if (!name.trim()) {
      setError("Profile name is required.");
      return;
    }
    setSaving(true);
    try {
      await axiosPrivate.put(`Profile/${profile.id}`, {
        name: name.trim(),
        avatarUrl,
      });
      navigate("/account/profiles");
    } catch (err) {
      console.error(err);
      setError("Unable to save profile changes.");
    } finally {
      setSaving(false);
    }
  }

  function handleCancel() {
    navigate("/account/profiles");
  }

  function handleCustomSeed(customSeed) {
    if (customSeed.trim()) {
      const style = avatarUrl.includes("/adventurer/")
        ? "adventurer"
        : avatarUrl.includes("/bottts/")
          ? "bottts"
          : avatarUrl.includes("/lorelei/")
            ? "lorelei"
            : avatarUrl.includes("/personas/")
              ? "personas"
              : avatarUrl.includes("/micah/")
                ? "micah"
                : avatarUrl.includes("/big-ears/")
                  ? "big-ears"
                  : "thumbs";
      selectAvatar(style, customSeed);
    }
  }

  if (error) {
    return (
      <div className={styles.profileEditPage}>
        <div className={styles.editHeader}>
          <button className={styles.backBtn} onClick={handleCancel}>
            ← Back
          </button>
        </div>
        <div className={styles.profileError}>{error}</div>
      </div>
    );
  }

  if (!profile) {
    return <div className={styles.profileEditPage}>Loading profile...</div>;
  }

  return (
    <div className={styles.profileEditPage}>
      <div className={styles.editHeader}>
        <button className={styles.backBtn} onClick={handleCancel}>
          ← Back
        </button>
        <div className={styles.headerTitle}>
          <h1>Choose a profile icon</h1>
          <p>
            For <strong>{profile.name}</strong>
          </p>
          <img
            src={avatarUrl}
            alt="Current avatar"
            className={styles.headerAvatar}
          />
        </div>
      </div>

      <div className={styles.editContainer}>
        <aside className={styles.previewSidebar}>
          <div className={styles.previewBox}>
            <img
              src={avatarUrl}
              alt="Profile avatar"
              className={styles.previewImg}
            />
          </div>

          <div className={styles.profileNameEdit}>
            <label>Profile name</label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Profile name"
              className={styles.seedInput}
            />
          </div>

          {/* <div className={styles.kidsToggle}>
            <label>
              <input
                type="checkbox"
                checked={isKidsProfile}
                onChange={(e) => setIsKidsProfile(e.target.checked)}
              />
              <span>Kids profile</span>
            </label>
          </div> */}

          <div className={styles.footerButtons}>
            <button
              className={styles.saveBtn}
              onClick={handleSave}
              disabled={saving}
            >
              {saving ? "Saving..." : "Save Profile"}
            </button>
            <button className={styles.cancelBtn} onClick={handleCancel}>
              Cancel
            </button>
          </div>
        </aside>

        <main className={styles.avatarContent}>
          {avatarHistory.length > 0 && (
            <section className={styles.avatarSection}>
              <h2>History</h2>
              <div className={styles.avatarGrid}>
                {avatarHistory.map((url, idx) => (
                  <button
                    key={idx}
                    className={`avatarItem ${avatarUrl === url ? "active" : ""}`}
                    onClick={() => setAvatarUrl(url)}
                    title="Click to select"
                  >
                    <img src={url} alt={`History ${idx}`} />
                  </button>
                ))}
              </div>
            </section>
          )}

          {/* <section className={styles.avatarSection}>
            <h2>Custom seed</h2>
            <div className={styles.customSeedForm}>
              <input
                type="text"
                placeholder="Enter custom seed (e.g., your name)"
                onKeyPress={(e) => {
                  if (e.key === "Enter") {
                    handleCustomSeed(e.target.value);
                    e.target.value = "";
                  }
                }}
                className={styles.seedInput}
              />
              <button
                className={styles.applyBtn}
                onClick={(e) => {
                  const input = e.target.previousElementSibling;
                  handleCustomSeed(input.value);
                  input.value = "";
                }}
              >
                Apply
              </button>
            </div>
          </section> */}

          {dicebearStyles.map((style) => (
            <section key={style} className={styles.avatarSection}>
              <h2>{style.charAt(0).toUpperCase() + style.slice(1)}</h2>
              <div className={styles.avatarGrid}>
                {[...Array(8)].map((_, idx) => {
                  const seed = `${profile.id}-${style}-${idx}`;
                  const url = getAvatarUrl(seed, style);
                  return (
                    <button
                      key={`${style}-${idx}`}
                      className={`avatarItem ${avatarUrl === url ? "active" : ""}`}
                      onClick={() => selectAvatar(style, seed)}
                      title={`${style} - variant ${idx + 1}`}
                    >
                      <img
                        src={url}
                        alt={`${style} avatar ${idx}`}
                        loading="lazy"
                      />
                    </button>
                  );
                })}
              </div>
            </section>
          ))}
        </main>
      </div>
    </div>
  );
}

export default AccountProfileEditPage;
