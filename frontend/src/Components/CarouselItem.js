import styles from "./CarouselItem.module.css";

function CarouselItem({ detail }) {
  return (
    <div className={styles.carouselItem}>
      <img src={detail.posterUrl} alt={detail.title} />
    </div>
  );
}

export default CarouselItem;
