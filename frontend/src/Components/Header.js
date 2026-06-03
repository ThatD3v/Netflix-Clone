import React from "react";
import "../Components/Header.css";
import { useNavigate } from "react-router-dom";

function Header() {
  const navigate = useNavigate();

  return (
    <div className="landing-header">
      <div className="img">
        <img src="Netflix Logo/Netflix.png" alt="Netflix" width={"150px"} />
      </div>
      <div className="signIn">
        <button onClick={() => navigate("/check")}>Sign in</button>
      </div>
    </div>
  );
}

export default Header;
