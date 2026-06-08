import { useContext, useState } from "react";
import axios from "../api/axios";
import "../Styles/RegisterPage.css";
import AuthContext from "../Context/AuthProvider";
import { FaEye, FaEyeSlash } from "react-icons/fa";
import { useNavigate } from "react-router-dom";

const REGISTER_URL = "Auth/register";

function CheckPage() {
  const navigate = useNavigate();
  const { auth } = useContext(AuthContext);

  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);

  async function handleRegister(e) {
    e.preventDefault();
    try {
      const response = await axios.post(
        REGISTER_URL,
        { identifier: auth.email, password, rememberMe: true },
        {
          headers: { "Content-Type": "application/json" },
          withCredentials: true,
        },
      );
      console.log(JSON.stringify(response?.data));
      console.log(auth);
      console.log(password);
      if (response?.data?.hasSubscription) {
        navigate("/profiles");
      } else {
        navigate("/plans");
      }
    } catch (error) {
      console.error(error.response?.data);
      alert(error?.response?.data?.errors?.Password);
      if (
        error.response.data.message ===
        "An account with this email already exists. Please login."
      ) {
        navigate("/login");
      } else {
        return;
      }
    }
  }

  return (
    <div className="registerPage">
      <header>
        <img src="Netflix Logo/Netflix.png" alt="Netflix" width={"150px"} />
      </header>
      <div className="register-section">
        <div className="text">
          <h1>Enter your password to register</h1>
          <h3>{auth.email}</h3>
        </div>
        <form onSubmit={handleRegister}>
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
