import { useEffect, useState } from "react";
import "../Styles/HomePage.css";
import { CiSearch } from "react-icons/ci";
import { MdAccountBox } from "react-icons/md";
import { ImExit } from "react-icons/im";
import { FaUserCircle, FaUserEdit } from "react-icons/fa";
import { NavLink, useLocation, useNavigate } from "react-router-dom";
import "./HomeHeader.css";
import useAuth from "../Hooks/useAuth";
import DropdownProfile from "../Components/DropdownProfile";

function HomeHeader() {
  const [scrolled, setScrolled] = useState(false);
  const [error, setError] = useState(false);

  const { auth, setAuth } = useAuth();

  const location = useLocation();
  const navigate = useNavigate();

  const selectedProfile =
    location.state?.profile ??
    JSON.parse(localStorage.getItem("selectedProfile") || "null");

  const avatarUrl = selectedProfile?.avatarUrl || "";

  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > 100);
    };

    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  return (
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
                {auth?.profiles?.map((profile) => (
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
  );
}

export default HomeHeader;
