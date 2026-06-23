import { FaInfoCircle, FaPlay } from "react-icons/fa";
import "./HomeHero.css";

function HomeHero({ heroBanner }) {
  const heroStyle = {
    backgroundImage: heroBanner?.bannerUrl
      ? `linear-gradient(to bottom, rgba(0, 0, 0, 0.85) 0%, rgba(0, 0, 0, 0.35) 10%), 
       linear-gradient(to bottom, transparent 60%, #141414 100%), 
       url(${heroBanner.bannerUrl})`
      : "none",
  };
  return (
    <div className="background" style={heroStyle}>
      <div className="bannerOverlay">
        <div>
          <h3>Most Liked</h3>
          <p>{heroBanner?.description}</p>
          <div className="buttons">
            <div>
              <button id="lightButton">
                <div>
                  <FaPlay className="buttonIcon" />
                </div>
                <div style={{ width: "0.5rem" }}></div>
                Play
              </button>
            </div>
            <div>
              <button id="darkButton">
                <div>
                  <FaInfoCircle className="buttonIcon" color="white" />
                </div>
                <div style={{ width: "0.5rem" }}></div>
                More Info
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default HomeHero;
