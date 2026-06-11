import { useNavigate } from "react-router-dom";
import "../Styles/PlansPage.css";
import Plan from "../Components/Plan";
import { useEffect, useState } from "react";
import axios from "../api/axios";
import useAxiosPrivate from "../Hooks/useAxiosPrivate";
import useAuth from "../Hooks/useAuth";
import PaystackPop from "@paystack/inline-js";

function PlansPage() {
  const gradients = [
    "radial-gradient(140.76% 131.96% at 100% 100%, rgb(33, 114, 227) 0%, rgba(74, 42, 150, 0.5) 73.57%, rgba(74, 42, 150, 0) 100%), rgb(29, 82, 157)",
    "radial-gradient(140.76% 131.96% at 100% 100%, rgb(109, 59, 227) 0%, rgba(74, 42, 150, 0.5) 73.57%, rgba(74, 42, 150, 0) 100%), rgb(29, 82, 157)",
    "radial-gradient(140.76% 131.96% at 100% 100%, rgb(176, 56, 220) 0%, rgba(74, 42, 150, 0.5) 73.57%, rgba(74, 42, 150, 0) 100%), rgb(29, 82, 157)",
    "radial-gradient(140.76% 131.96% at 100% 100%, rgb(229, 9, 20) 0%, rgba(74, 42, 150, 0.5) 73.57%, rgba(74, 42, 150, 0) 100%), rgb(29, 82, 157)",
  ];

  const axiosPrivate = useAxiosPrivate("");
  const { auth } = useAuth();
  const navigate = useNavigate();

  const [plans, setPlans] = useState([]);
  const [error, setError] = useState("");
  const [selectedPlan, setSelectedPlan] = useState(null);

  const handlePayment = async () => {
    try {
      const initPaymentResponse = await axiosPrivate.post(
        "Subscription/initialize-payment",
        JSON.stringify({ planId: selectedPlan, email: auth.email }),
      );

      const { accessCode, reference } = initPaymentResponse.data;
      console.log(initPaymentResponse.data);

      if (!accessCode || !reference) {
        throw new Error("Missing access code or reference from backend");
      }

      const popup = new PaystackPop();

      popup.resumeTransaction(accessCode, {
        onSuccess: async (transaction) => {
          try {
            // 3) Ask your backend to verify using the reference
            const verifyRes = await axiosPrivate.get(
              `Subscription/verify-payment${reference}`,
              { withCredentials: true },
            );

            if (verifyRes.data?.status === true) {
              navigate("/plans");
            } else {
              setError("Payment could not be verified.");
            }
          } catch (err) {
            setError("Verification failed.");
          }
        },

        onCancel: () => {
          setError("Payment cancelled.");
        },
      });
    } catch (error) {
      setError("Failed to initialize payment");
      console.error(error);
    }
  };

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

    if (plans.length > 0 && !selectedPlan) {
      setSelectedPlan(plans[3].id);
    }

    fetchPlans();
  }, [plans, selectedPlan]);

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
          <h1>Choose the plan that's right for you</h1>
        </div>
        <div className="plans">
          {plans.map((plan, index) => (
            <Plan
              selected={selectedPlan === plan.id}
              onSelect={() => setSelectedPlan(plan.id)}
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
