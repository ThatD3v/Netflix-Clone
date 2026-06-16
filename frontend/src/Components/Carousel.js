import styles from "./Carousel.module.css";
import CarouselItem from "./CarouselItem";

function Carousel({ title, details }) {
  if (!details) return null;

  return (
    <>
      <h1 id={styles.rowTitles}>{title}</h1>
      <div className={styles.carousel}>
        {details.map((detail) => (
          <CarouselItem key={detail.id} detail={detail} />
        ))}
      </div>
    </>
  );
}

export default Carousel;
