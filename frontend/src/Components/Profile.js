import { useState } from "react";
import "./Profile.css";
import { FaUserCircle } from "react-icons/fa";
import { CiEdit } from "react-icons/ci";

function Profile({ profile, showEdit }) {
  const [error, setError] = useState(false);

  return (
    <div className="profile">
      {showEdit && <CiEdit className="editIcon" />}
      {error || !profile.avatarUrl ? (
        <FaUserCircle
          className={`profileIcon ${showEdit ? "editing" : ""}`}
          style={{ fontSize: "10rem", color: "rgba(255, 255, 255, 0.7)" }}
        />
      ) : (
        <img
          className={`profileIcon ${showEdit ? "editing" : ""}`}
          style={{ color: "rgba(255, 255, 255, 0.7)" }}
          src={profile.avatarUrl}
          alt={profile.name}
          onError={() => setError(true)}
        />
      )}
      <p>{profile.name}</p>
    </div>
  );
}

export default Profile;
