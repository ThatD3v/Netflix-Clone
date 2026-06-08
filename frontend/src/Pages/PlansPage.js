import { useNavigate } from "react-router-dom";
import "../Styles/PlansPage.css";
import Plan from "../Components/Plan";
import { useEffect, useState } from "react";
import axios from "../api/axios";

function PlansPage() {
  const gradients = [
    "radial-gradient(140.76% 131.96% at 100% 100%, rgb(33, 114, 227) 0%, rgba(74, 42, 150, 0.5) 73.57%, rgba(74, 42, 150, 0) 100%), rgb(29, 82, 157)",
    "radial-gradient(140.76% 131.96% at 100% 100%, rgb(109, 59, 227) 0%, rgba(74, 42, 150, 0.5) 73.57%, rgba(74, 42, 150, 0) 100%), rgb(29, 82, 157)",
    "radial-gradient(140.76% 131.96% at 100% 100%, rgb(176, 56, 220) 0%, rgba(74, 42, 150, 0.5) 73.57%, rgba(74, 42, 150, 0) 100%), rgb(29, 82, 157)",
    "radial-gradient(140.76% 131.96% at 100% 100%, rgb(229, 9, 20) 0%, rgba(74, 42, 150, 0.5) 73.57%, rgba(74, 42, 150, 0) 100%), rgb(29, 82, 157)",
  ];

  const handlePayment = async () => {};

  const navigate = useNavigate();

  const [plans, setPlans] = useState([]);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchPlans = async () => {
      try {
        const response = await axios.get("Subscription/plans");

        setPlans(response.data.plans);
      } catch (err) {
        setError("Failed to load plans");
        console.error(err);
      }
    };

    fetchPlans();
  }, []);

  if (error) {
    return <h2>{error}</h2>;
  }

  return (
    <div className="plansPage">
      <header>
        <img src="Netflix Logo/Netflix.png" alt="Netflix" width={"150px"} />
        <button onClick={() => navigate("/check")}>Sign in</button>
      </header>
      <main>
        <div>
          Step 1 of 3<h1>Choose the plan that's right for you</h1>
        </div>
        <div className="plans">
          {plans.map((plan, index) => (
            <Plan
              key={plan.id}
              plan={plan}
              gradient={gradients[index % gradients.length]}
            />
          ))}
        </div>
        <button onClick={handlePayment}>Next</button>
        <br />
      </main>
    </div>
  );
}

export default PlansPage;
