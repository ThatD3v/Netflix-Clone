import { useEffect, useState } from "react";
import useAxiosPrivate from "../Hooks/useAxiosPrivate";
import "../Styles/ProfilePage.css";
import Profile from "../Components/Profile";
import { GoPlusCircle } from "react-icons/go";
import { useNavigate } from "react-router-dom";

function ManageProfilesPage() {
  const [profiles, setProfiles] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [error, setError] = useState("");

  const axiosPrivate = useAxiosPrivate();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchProfiles = async () => {
      try {
        const response = await axiosPrivate.get("Profile/all");
        setProfiles(response.data.profiles);
        console.log(response.data);
      } catch (err) {
        setError("Failed to load profiles");
        console.error(err);
      }
    };

    fetchProfiles();
  }, [axiosPrivate]);

  if (error) {
    return <h2>{error}</h2>;
  }

  return (
    <div className="profilesPage">
      <h1>Who's watching?</h1>
      <div className="profileList">
        {profiles.map((profile) => (
          <Profile key={profile.id} profile={profile} showEdit={true} />
        ))}
        {profiles.length < 5 && (
          <div className="addProfile">
            <GoPlusCircle
              className="icon"
              onClick={() => setIsModalOpen(!isModalOpen)}
              style={{ fontSize: "10rem", color: "rgba(255, 255, 255, 0.7)" }}
            />
            <p>Add Profile</p>
          </div>
        )}
      </div>
      <button
        style={{ backgroundColor: "rgba(255, 255, 255, 0.8)", color: "black" }}
        onClick={() => navigate("/profiles")}
      >
        Done
      </button>
    </div>
  );
}

export default ManageProfilesPage;
