import React, { useContext, useEffect, useRef, useState } from "react";
import "../Styles/LandingPage.css";
import Header from "../Components/Header";
import FAQ from "../Components/Faq";
import { useNavigate } from "react-router-dom";
import axios from "../api/axios";
import AuthContext from "../Context/AuthProvider";

function LandingPage() {
  const { auth, setAuth } = useContext(AuthContext);

  const navigate = useNavigate();

  const carouselRef = useRef(null);

  const [email, setEmail] = useState("");
  const [showLeft, setShowLeft] = useState(false);
  const [showRight, setShowRight] = useState(true);

  const updateButtons = () => {
    const el = carouselRef.current;

    if (!el) return;

    setShowLeft(el.scrollLeft > 0);

    setShowRight(el.scrollLeft + el.clientWidth < el.scrollWidth - 1);
  };

  useEffect(() => {
    updateButtons();

    const el = carouselRef.current;

    el.addEventListener("scroll", updateButtons);

    return () => {
      el.removeEventListener("scroll", updateButtons);
    };
  }, []);

  const scrollLeft = () => {
    carouselRef.current.scrollBy({
      left: -400,
      behavior: "smooth",
    });
  };

  const scrollRight = () => {
    carouselRef.current.scrollBy({
      left: 400,
      behavior: "smooth",
    });
  };

  async function handleSubmit(e) {
    e.preventDefault();

    try {
      const response = await axios.post(
        "Auth/check",
        JSON.stringify({ identifier: email }),
        {
          headers: { "Content-Type": "application/json" },
          withCredentials: true,
        },
      );
      console.log(JSON.stringify(response?.data));
      console.log(auth);
      setAuth(email);
      if (response.data.exists) {
        navigate("/login");
      } else {
        navigate("/register");
      }
    } catch (error) {
      console.error(error);
    }
  }

  return (
    <div className="landing">
      <div className="background">
        <div className="headerContainer">
          <Header />
        </div>

        <div className="mainTitle">
          <h1>Unlimited movies, TV shows, and more</h1>
          <h4>Starts at ₦2,500. Cancel anytime.</h4>
          <p>
            Ready to watch? Enter your email to create or restart your
            membership.
          </p>
          <div className="form">
            <form onSubmit={handleSubmit}>
              <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Email address"
                autoComplete="off"
                required
              />
              <button>Get Started</button>
            </form>
          </div>
        </div>
        <div class="custom-shape-divider-bottom-1779451652">
          <svg
            data-name="Layer 1"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1200 120"
            preserveAspectRatio="none"
          >
            <path
              d="M0,0V46.29c47.79,22.2,103.59,32.17,158,28,70.36-5.37,136.33-33.31,206.8-37.5C438.64,32.43,512.34,53.67,583,72.05c69.27,18,138.3,24.88,209.4,13.08,36.15-6,69.85-17.84,104.45-29.34C989.49,25,1113-14.29,1200,52.47V0Z"
              opacity=".25"
              class="shape-fill"
            ></path>
            <path
              d="M0,0V15.81C13,36.92,27.64,56.86,47.69,72.05,99.41,111.27,165,111,224.58,91.58c31.15-10.15,60.09-26.07,89.67-39.8,40.92-19,84.73-46,130.83-49.67,36.26-2.85,70.9,9.42,98.6,31.56,31.77,25.39,62.32,62,103.63,73,40.44,10.79,81.35-6.69,119.13-24.28s75.16-39,116.92-43.05c59.73-5.85,113.28,22.88,168.9,38.84,30.2,8.66,59,6.17,87.09-7.5,22.43-10.89,48-26.93,60.65-49.24V0Z"
              opacity=".5"
              class="shape-fill"
            ></path>
            <path
              d="M0,0V5.63C149.93,59,314.09,71.32,475.83,42.57c43-7.64,84.23-20.12,127.61-26.46,59-8.63,112.48,12.24,165.56,35.4C827.93,77.22,886,95.24,951.2,90c86.53-7,172.46-45.71,248.8-84.81V0Z"
              class="shape-fill"
            ></path>
          </svg>
        </div>
      </div>
      <div className="carousel-section">
        <h1>Trending Now</h1>
        <div className="carousel-wrapper">
          {showLeft && (
            <button className="nav left" onClick={scrollLeft}>
              ◀
            </button>
          )}
          <div className="carousel" ref={carouselRef}>
            <div className="item">
              <img src="poster1.jpg" alt="beauty in black" />
            </div>
            <div className="item">
              <img src="poster2.jpg" alt="beauty in black" />
            </div>
            <div className="item">
              <img src="poster3.jpg" alt="beauty in black" />
            </div>
            <div className="item">
              <img src="poster1.jpg" alt="beauty in black" />
            </div>
            <div className="item">
              <img src="poster2.jpg" alt="beauty in black" />
            </div>
            <div className="item">
              <img src="poster3.jpg" alt="beauty in black" />
            </div>
            <div className="item">
              <img src="poster1.jpg" alt="beauty in black" />
            </div>
            <div className="item">
              <img src="poster2.jpg" alt="beauty in black" />
            </div>
            <div className="item">
              <img src="poster3.jpg" alt="beauty in black" />
            </div>
            <div className="item">
              <img src="poster1.jpg" alt="beauty in black" />
            </div>
          </div>
          {showRight && (
            <button className="nav right" onClick={scrollRight}>
              ▶
            </button>
          )}
        </div>
        <br />
        <br />
        <br />
        <br />
        <br />
      </div>
      <div className="join-reasons-section">
        <h2>More Reasons to Join</h2>
        <div className="join-reasons">
          <div>
            <h1>Enjoy on your TV</h1>
            <p>
              Watch on Smart TVs, Playstation, Xbox, Chromecast, Apple TV,
              Blu-ray players, and more.
            </p>
          </div>
          <div>
            <h1>Enjoy on your TV</h1>
            <p>
              Watch on Smart TVs, Playstation, Xbox, Chromecast, Apple TV,
              Blu-ray players, and more.
            </p>
          </div>
          <div>
            <h1>Enjoy on your TV</h1>
            <p>
              Watch on Smart TVs, Playstation, Xbox, Chromecast, Apple TV,
              Blu-ray players, and more.
            </p>
          </div>
          <div>
            <h1>Enjoy on your TV</h1>
            <p>
              Watch on Smart TVs, Playstation, Xbox, Chromecast, Apple TV,
              Blu-ray players, and more.
            </p>
          </div>
        </div>
        <br />
        <br />
        <br />
        <br />
      </div>
      <div className="faq-section">
        <h2>Frequently Asked Questions</h2>
        <FAQ />
      </div>
      <br />
      <br />
      <br />
    </div>
  );
}

export default LandingPage;
