import { useContext, useState } from "react";
import axios from "../api/axios";
import "../Styles/CheckPage.css";
import AuthContext from "../Context/AuthProvider";
import { useNavigate } from "react-router-dom";

const CHECK_URL = "Auth/check";

function CheckPage() {
  const navigate = useNavigate();
  const { auth, setAuth } = useContext(AuthContext);

  const [email, setEmail] = useState("");

  async function handleCheck(e) {
    e.preventDefault();
    try {
      const response = await axios.post(
        CHECK_URL,
        JSON.stringify({ identifier: email }),
        {
          headers: { "Content-Type": "Application/json" },
          withCredentials: true,
        },
      );
      setAuth({ email });
      console.log(JSON.stringify(response?.data));
      console.log(JSON.stringify(auth));
      if (response.data.exists) {
        navigate("/login");
      } else {
        navigate("/register");
      }
    } catch (error) {
      console.error(error);
    }
  }

  return (
    <div className="checkPage">
      <header>
        <img src="Netflix Logo/Netflix.png" alt="Netflix" width={"150px"} />
      </header>
      <div className="check-section">
        <div className="text">
          <h1>Enter your info to sign in</h1>
          <h3>Or get started with a new account</h3>
        </div>
        <form onSubmit={handleCheck}>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Email address"
            required
            autoComplete="off"
          />
          <br />
          <button>Continue</button>
        </form>
      </div>
    </div>
  );
}

export default CheckPage;
