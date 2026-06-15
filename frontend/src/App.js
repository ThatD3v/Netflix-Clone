import { BrowserRouter, Route, Routes } from "react-router-dom";
import LandingPage from "./Pages/LandingPage";
import CheckPage from "./Pages/CheckPage";
import LoginPage from "./Pages/LoginPage";
import RegisterPage from "./Pages/RegisterPage";
import PlansPage from "./Pages/PlansPage";
import ProfilesPage from "./Pages/ProfilesPage";
import ManageProfilesPage from "./Pages/ManageProfilesPage";
import Layout from "./Components/Layout";
import { LoadingProvider } from "./Context/LoadingProvider";
import HomePage from "./Pages/HomePage";
// import RequireAuth from "./Components/RequireAuth";

function App() {
  return (
    <LoadingProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<LandingPage />} />
            <Route path="/check" element={<CheckPage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/plans" element={<PlansPage />} />
            {/* <Route element={<RequireAuth />}> */}
            <Route path="/profiles" element={<ProfilesPage />} />
            <Route path="/home" element={<HomePage />} />
            <Route path="/manageProfiles" element={<ManageProfilesPage />} />
            {/* </Route> */}
          </Route>
        </Routes>
      </BrowserRouter>
    </LoadingProvider>
  );
}

export default App;
