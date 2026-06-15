import { useEffect, useState } from "react";
import "../Styles/HomePage.css";
import { CiSearch, CiBellOn } from "react-icons/ci";
import { FaInfoCircle, FaUserCircle, FaPlay } from "react-icons/fa";
import { useLocation, useNavigate } from "react-router-dom";
import useAxiosPrivate from "../Hooks/useAxiosPrivate";
import useAuth from "../Hooks/useAuth";

function HomePage() {
  const [error, setError] = useState(false);
  const [heroBanner, setHeroBanner] = useState(null);
  const [rows, setRows] = useState(null);
  const [scrolled, setScrolled] = useState(false);

  const { auth, setAuth } = useAuth();

  const location = useLocation();
  const navigate = useNavigate();
  const axiosPrivate = useAxiosPrivate();

  const selectedProfile =
    location.state?.profile ??
    JSON.parse(localStorage.getItem("selectedProfile") || "null");

  const avatarUrl = selectedProfile?.avatarUrl || "";

  const heroStyle = heroBanner?.bannerUrl
    ? {
        backgroundImage: `linear-gradient(
          to bottom,
          rgba(0, 0, 0, 0.85) 0%,
          rgba(0, 0, 0, 0.35) 10%
        ), url(${heroBanner.bannerUrl})`,
      }
    : {};

  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > 0);
    };

    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  useEffect(() => {
    const fetchHomepage = async () => {
      try {
        const response = await axiosPrivate.get(
          `Browse/home?profileId=${auth.profileId}`,
        );

        setHeroBanner(response.data?.data?.heroBanner ?? null);
        setRows(response.data?.data?.rows ?? []);
        console.log(response.data);
        console.log(auth);
      } catch (err) {
        setError("Failed to load homepage");
        console.error(err);
      }
    };

    if (!auth.profileId && selectedProfile?.id) {
      setAuth((prev) => ({ ...prev, profileId: selectedProfile.id }));
    }

    fetchHomepage();
  }, [
    setHeroBanner,
    setRows,
    auth,
    setAuth,
    auth.profileId,
    selectedProfile.id,
    axiosPrivate,
  ]);

  return (
    <div className="homepage">
      <div className="background" style={heroStyle}>
        <div
          className={`homeHeader ${scrolled ? "navbar-scrolled" : "navbar-top"}`}
        >
          <div className="leftSide">
            <ul>
              <li>
                <img
                  style={{ cursor: "pointer" }}
                  onClick={() => navigate("/")}
                  src="Netflix Logo/Netflix.png"
                  alt="Netflix"
                  height={"25px"}
                />
              </li>
              <li>Home</li>
              <li>Shows</li>
              <li>Movies</li>
              <li>New & Popular</li>
              <li>My List</li>
            </ul>
          </div>

          <div className="rightSide">
            <CiSearch className="navbarIcons" />
            <CiBellOn className="navbarIcons" />

            <div className="avatarWrapper">
              {!error && avatarUrl ? (
                <img
                  src={avatarUrl}
                  alt={selectedProfile?.name || "Profile"}
                  onError={() => setError(true)}
                />
              ) : (
                <FaUserCircle className="navbarIcons" />
              )}
            </div>
          </div>
        </div>
        <div className="bannerOverlay">
          <div>
            <h3>Most Liked</h3>
            <p>
              Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
              eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut
              enim ad minim veniam, quis nostrud exercitation ullamco laboris
              nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in
              reprehenderit in voluptate velit esse cillum dolore eu fugiat
              nulla pariatur. Excepteur sint occaecat cupidatat non proident,
              sunt in culpa qui officia deserunt mollit anim id est laborum.
            </p>
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
    </div>
  );
}

export default HomePage;
