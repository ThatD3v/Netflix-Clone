import { useEffect, useState } from "react";
import "../Styles/HomePage.css";
import { CiSearch } from "react-icons/ci";
import { MdAccountBox } from "react-icons/md";
import { ImExit } from "react-icons/im";
import { FaInfoCircle, FaUserCircle, FaPlay, FaUserEdit } from "react-icons/fa";
import { NavLink, useLocation, useNavigate } from "react-router-dom";
import useAxiosPrivate from "../Hooks/useAxiosPrivate";
import useAuth from "../Hooks/useAuth";
import Carousel from "../Components/Carousel";
import DropdownProfile from "../Components/DropdownProfile";

function HomePage() {
  const [error, setError] = useState(false);
  const [heroBanner, setHeroBanner] = useState(null);
  const [rows, setRows] = useState([]);
  const [scrolled, setScrolled] = useState(false);

  const { auth, setAuth } = useAuth();

  const location = useLocation();
  const navigate = useNavigate();
  const axiosPrivate = useAxiosPrivate();

  const selectedProfile =
    location.state?.profile ??
    JSON.parse(localStorage.getItem("selectedProfile") || "null");

  const avatarUrl = selectedProfile?.avatarUrl || "";

  const heroStyle = {
    backgroundImage: heroBanner?.bannerUrl
      ? `linear-gradient(
          to bottom,
          rgba(0, 0, 0, 0.85) 0%,
          rgba(0, 0, 0, 0.35) 10%
        ), linear-gradient(
      to bottom,
      rgba(20, 20, 20, 0) 80%, 
      rgba(20, 20, 20, 0.7) 85%,
      #141414 100%
    ), url(${heroBanner.bannerUrl})`
      : "none",
  };

  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > 100);
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
            <li>
              <NavLink to={"/home"}>Home</NavLink>
            </li>
            <li>
              <NavLink to={"/shows"}>Shows</NavLink>
            </li>
            <li>
              <NavLink to={"/movies"}>Movies</NavLink>
            </li>
            <li>
              <NavLink to={"/new&popular"}>New & Popular</NavLink>
            </li>
            <li>
              <NavLink to={"mylist"}>My List</NavLink>
            </li>
          </ul>
        </div>

        <div className="rightSide">
          <CiSearch className="navbarIcons" />

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
            <div className="dropdownContent">
              <ul>
                <li>
                  {auth?.profiles.map((profile) => (
                    <DropdownProfile
                      key={profile.id}
                      profile={profile}
                      onClick={() => {
                        setAuth((prev) => ({ ...prev, profileId: profile.id }));
                        localStorage.setItem(
                          "selectedProfile",
                          JSON.stringify(profile),
                        );
                        navigate("/home", { state: { profile } });
                      }}
                    />
                  ))}
                </li>
                <li>
                  <NavLink to={"/account"}>
                    <div>
                      <FaUserEdit />
                      Manage Profiles
                    </div>
                  </NavLink>
                </li>
                <li>
                  <NavLink to={"/account"}>
                    <div>
                      <MdAccountBox />
                      Account
                    </div>
                  </NavLink>
                </li>
                <li>
                  <NavLink to={"/account"}>
                    <div>
                      <ImExit />
                      Sign out
                    </div>
                  </NavLink>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
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
      <div className="carouselContainer">
        {rows.map((row) => (
          <Carousel key={row.id} title={row.title} details={row.items} />
        ))}
      </div>
    </div>
  );
}

export default HomePage;
