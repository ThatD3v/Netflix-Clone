import { useState } from "react";
import { FaUserCircle } from "react-icons/fa";

function DropdownProfile({ profile, onClick }) {
  const [error, setError] = useState(false);

  return (
    <div
      className="dropdownProfile"
      onClick={onClick}
      style={{ color: "white" }}
    >
      {error || !profile.avatarUrl ? (
        <FaUserCircle
          style={{ fontSize: "1.5rem", color: "rgba(255, 255, 255, 0.7)" }}
        />
      ) : (
        <img
          style={{
            color: "rgba(255, 255, 255, 0.7)",
            height: "50px",
            width: "50px",
          }}
          src={profile.avatarUrl}
          alt={profile.name}
          onError={() => setError(true)}
        />
      )}
      {profile.name}
    </div>
  );
}

export default DropdownProfile;
