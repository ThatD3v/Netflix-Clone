import React from "react";
import "../Components/Header.css";

function Header() {
  return (
    <div className="landing-header">
      <div className="img">
        <img src="Netflix Logo/Netflix.png" alt="Netflix" width={"150px"} />
      </div>
      <div className="signIn">
        <button>Sign in</button>
      </div>
    </div>
  );
}

export default Header;
