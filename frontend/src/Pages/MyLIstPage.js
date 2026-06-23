import "../Styles/HomePage.css";
// import { useNavigate } from "react-router-dom";
import useAuth from "../Hooks/useAuth";
import Carousel from "../Components/Carousel";
import HomeHero from "../Components/HomeHero";
import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import useAxiosPrivate from "../Hooks/useAxiosPrivate";

function MyListPage() {
  const [heroBanner, setHeroBanner] = useState(null);
  const { auth, setAuth } = useAuth();

  const location = useLocation();
  // const navigate = useNavigate();
  const axiosPrivate = useAxiosPrivate();

  const selectedProfile =
    location.state?.profile ??
    JSON.parse(localStorage.getItem("selectedProfile") || "null");

  useEffect(() => {
    if (!auth.profileId) {
      if (selectedProfile?.id) {
        setAuth((prev) => ({ ...prev, profileId: selectedProfile.id }));
      }
      return;
    }

    const fetchHomepage = async () => {
      try {
        const response = await axiosPrivate.get(
          `Browse/home?profileId=${auth.profileId}`,
        );
        setHeroBanner(response.data?.data?.heroBanner ?? null);
        setAuth((prev) => ({ ...prev, rows: response.data?.data?.rows ?? [] }));
      } catch (err) {
        console.error(err);
      }
    };

    fetchHomepage();
  }, [auth.profileId, selectedProfile?.id, axiosPrivate, setAuth]);

  // const navigate = useNavigate();

  return (
    <div className="homepage">
      <HomeHero heroBanner={heroBanner} />
      <div className="carouselContainer">
        {auth?.rows?.map((row) => (
          <Carousel key={row.id} title={row.title} details={row.items} />
        ))}
      </div>
    </div>
  );
}

export default MyListPage;
