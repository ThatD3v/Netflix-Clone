import { useEffect, useState } from "react";
import useAxiosPrivate from "../Hooks/useAxiosPrivate";

function ProfilesPage() {
  const [profile, setProfile] = useState([]);
  const [error, setError] = useState("");
  const axiosPrivate = useAxiosPrivate();

  useEffect(() => {
    const fetchProfiles = async () => {
      try {
        const response = await axiosPrivate.get("Subscription/Profile/all");

        setProfile(response.data);
      } catch (err) {
        setError("Failed to load plans");
        console.error(err);
      }
    };

    fetchProfiles();
  }, [axiosPrivate]);

  if (error) {
    return <h2>{error}</h2>;
  }

  return <div></div>;
}

export default ProfilesPage;
