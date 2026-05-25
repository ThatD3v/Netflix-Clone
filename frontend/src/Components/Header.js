import React from "react";
import "../Components/Header.css";

function Header() {
  return (
    <div className="landing-header">
      <div className="img">
        <img
          src="Netflix Logo/Netflix_Logo_RGB.png"
          alt="Netflix"
          width={"200px"}
        />
      </div>
      <div className="signIn">
        <button>Sign in</button>
      </div>
    </div>
  );
}

export default Header;
