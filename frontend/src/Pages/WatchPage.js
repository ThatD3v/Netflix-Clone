import styles from "../Styles/watchPage.module.css";

import { useLocation } from "react-router-dom";

function WatchPage() {
  const location = useLocation();
  const { trailerUrl, title } = location.state || {};

  if (!trailerUrl) return <p>Video not found.</p>;

  return (
    <div className={styles.watchpage}>
      <h1>{title}</h1>
      <div className={styles.videoWrapper}>
        <iframe
          src={trailerUrl}
          title={title}
          allow="autoplay; fullscreen"
          allowFullScreen
        />
      </div>
    </div>
  );
}

export default WatchPage;
