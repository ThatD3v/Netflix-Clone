import { useCallback, useEffect, useState } from "react";
import useAxiosPrivate from "../Hooks/useAxiosPrivate";
import "../Styles/ProfilePage.css";
import Profile from "../Components/Profile";
import { GoPlusCircle } from "react-icons/go";
import { useNavigate } from "react-router-dom";
import Modal from "../Components/Modal";
import { FaToggleOff, FaToggleOn } from "react-icons/fa6";

const styles = ["adventurer", "bottts", "lorelei", "personas"];

function ManageProfilesPage() {
  const [profiles, setProfiles] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [error, setError] = useState("");
  const [kidsProfile, setKidsProfile] = useState(false);
  const [name, setName] = useState("");

  const axiosPrivate = useAxiosPrivate();
  const navigate = useNavigate();

  const fetchProfiles = useCallback(async () => {
    try {
      const response = await axiosPrivate.get("Profile/all");
      setProfiles(response.data.profiles);
      console.log(response.data);
    } catch (err) {
      setError("Failed to load profiles");
      console.error(err);
    }
  }, [axiosPrivate]);

  function getAvatarUrl(name) {
    const randomStyle = styles[Math.floor(Math.random() * styles.length)];
    return `https://api.dicebear.com/10.x/${randomStyle}/svg?seed=${name}`;
  }

  useEffect(() => {
    fetchProfiles();
  }, [fetchProfiles]);

  async function handleAddProfile(e) {
    e.preventDefault();
    const avatarUrl = getAvatarUrl(name);
    try {
      const response = await axiosPrivate.post("Profile/create", {
        name,
        avatarUrl,
        isKidsProfile: kidsProfile,
      });
      await fetchProfiles();
      setIsModalOpen(false);
      console.log(response.data);
    } catch (error) {
      setError("Failed to add profile.");
      console.error(error);
    }
  }

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
      <Modal open={isModalOpen} close={() => setIsModalOpen(false)}>
        <div className="modalContent">
          <h1>Add a profile</h1>
          <h4>Add a profile for another person watching Netflix</h4>
          <form onSubmit={handleAddProfile}>
            <input
              value={name}
              onChange={(e) => setName(e.target.value)}
              type="text"
              placeholder="Name"
              required
            />
            <br />
            <br />
            <br />
            <hr />
            <br />
            <div className="kidsProfileContainer">
              <div className="kidsProfileLeft">
                <h1>Kids Profile</h1>
                <h4>Only see kids-friendly TV shows and movies</h4>
              </div>
              <div className="checkboxRight">
                {kidsProfile ? (
                  <FaToggleOn
                    className="onCheckbox"
                    onClick={() => setKidsProfile(!kidsProfile)}
                  />
                ) : (
                  <FaToggleOff
                    className="offCheckbox"
                    onClick={() => setKidsProfile(!kidsProfile)}
                  />
                )}
              </div>
            </div>
            <div className="buttonsDiv">
              <button type="submit" id="modalSave">
                Save
              </button>
              <button
                id="modalCancel"
                type="button"
                onClick={() => setIsModalOpen(false)}
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      </Modal>
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
