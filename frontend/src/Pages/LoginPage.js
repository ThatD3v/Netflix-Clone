import { useContext, useState } from "react";
import axios from "../api/axios";
import "../Styles/LoginPage.css";
import AuthContext from "../Context/AuthProvider";
import { FaEye, FaEyeSlash } from "react-icons/fa";
import { useNavigate } from "react-router-dom";

const LOGIN_URL = "Auth/login";

function CheckPage() {
  const { auth } = useContext(AuthContext);
  const navigate = useNavigate();

  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);

  async function handleLogin(e) {
    e.preventDefault();
    try {
      const response = await axios.post(
        LOGIN_URL,
        JSON.stringify({ identifier: auth.email, password }),
        {
          headers: { "Content-Type": "Application/json" },
          withCredentials: true,
        },
      );
      console.log(response?.data);
      console.log(auth);
      if (response.data.hasSubscription) {
        navigate("/profiles");
      } else {
        navigate("/plans");
      }
    } catch (error) {
      console.error(error);
      console.error(error.response.data);
    }
  }

  return (
    <div className="loginPage">
      <header>
        <img src="Netflix Logo/Netflix.png" alt="Netflix" width={"150px"} />
      </header>
      <div className="login-section">
        <div className="text">
          <h1>Enter your info to sign in</h1>
          <h3>{auth.email}</h3>
        </div>
        <form onSubmit={handleLogin}>
          <div className="password-container">
            <input
              type={showPassword ? "text" : "password"}
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
            <button
              type="button"
              onClick={() => setShowPassword((prev) => !prev)}
              className="password-toggle"
            >
              {showPassword ? <FaEyeSlash /> : <FaEye />}
            </button>
          </div>
          <br />
          <button className="submit-button">Continue</button>
        </form>
      </div>
    </div>
  );
}

export default CheckPage;
