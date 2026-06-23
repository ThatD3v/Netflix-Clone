import { useState } from "react";
import styles from "./CarouselItem.module.css";
import {
  FaPlay,
  FaCheck,
  FaThumbsUp,
  FaCaretDown,
  FaPlus,
} from "react-icons/fa";
import { useNavigate } from "react-router-dom";

function CarouselItem({ detail }) {
  const [hovered, setHovered] = useState(false);
  const [myList, setMyList] = useState(false);

  const navigate = useNavigate();

  const handlePlay = () => {
    navigate(`/watch/${detail.id}`, {
      state: { trailerUrl: detail.trailerUrl, title: detail.title },
    });
  };

  const handleMouseEnter = () => {
    setTimeout(() => setHovered(true)); // 500ms delay
  };

  return (
    <div
      className={styles.container}
      onMouseEnter={handleMouseEnter}
      onMouseLeave={() => setHovered(false)}
    >
      <img
        src={detail.posterUrl}
        alt={detail.title}
        className={styles.poster}
      />
      {hovered && (
        <div className={styles.expandedCard}>
          <img src={detail.posterUrl} alt={detail.title} />
          <div className={styles.details}>
            <div className={styles.leftside}>
              <FaPlay
                className={styles.icon}
                title="Play"
                onClick={handlePlay}
              />
              {myList ? (
                <FaCheck className={styles.icon} title="Remove from my list" />
              ) : (
                <FaPlus className={styles.icon} title="Add to my list" />
              )}
              <FaThumbsUp className={styles.icon} title="Add to Liked" />
            </div>
            <FaCaretDown className={styles.icon} title="More Info" />
          </div>
        </div>
      )}
    </div>
  );
}

export default CarouselItem;
