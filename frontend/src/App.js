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
import ShowsPage from "./Pages/ShowsPage";
import HomeLayout from "./Components/HomeLayout";
import MoviesPage from "./Pages/MoviesPage";
import MyLIstPage from "./Pages/MyLIstPage";
import NewPage from "./Pages/NewPage";
import WatchPage from "./Pages/WatchPage";
import AccountPage from "./Pages/AccountPage";
import AccountLayout from "./Components/AccountLayout";
import AccountSectionPage from "./Pages/AccountSectionPage";
import AccountProfilesPage from "./Pages/AccountProfilesPage";
import AccountProfileEditPage from "./Pages/AccountProfileEditPage";
import RequireAuth from "./Components/RequireAuth";

function App() {
  return (
    <LoadingProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<LandingPage />} />
            <Route path="check" element={<CheckPage />} />
            <Route path="login" element={<LoginPage />} />
            <Route path="register" element={<RegisterPage />} />
            <Route path="plans" element={<PlansPage />} />
            <Route path="watch/:id" element={<WatchPage />} />
            <Route path="profiles" element={<ProfilesPage />} />
            <Route path="manageProfiles" element={<ManageProfilesPage />} />

            <Route element={<RequireAuth />}>
              <Route element={<HomeLayout />}>
                <Route path="home" element={<HomePage />} />
                <Route path="shows" element={<ShowsPage />} />
                <Route path="movies" element={<MoviesPage />} />
                <Route path="new&popular" element={<NewPage />} />
                <Route path="mylist" element={<MyLIstPage />} />
              </Route>
              <Route path="account" element={<AccountLayout />}>
                <Route index element={<AccountPage />} />
                <Route
                  path="membership"
                  element={
                    <AccountSectionPage title="Membership">
                      Manage your billing, plan details, and payment methods
                      from a single place.
                    </AccountSectionPage>
                  }
                />
                <Route
                  path="security"
                  element={
                    <AccountSectionPage title="Security">
                      Update your password, manage login settings, and review
                      account access.
                    </AccountSectionPage>
                  }
                />
                <Route
                  path="devices"
                  element={
                    <AccountSectionPage title="Devices">
                      See devices currently signed in and remove unwanted access
                      instantly.
                    </AccountSectionPage>
                  }
                />
                <Route path="profiles" element={<AccountProfilesPage />} />
                <Route
                  path="profiles/:id/edit"
                  element={<AccountProfileEditPage />}
                />
              </Route>
            </Route>
          </Route>
        </Routes>
      </BrowserRouter>
    </LoadingProvider>
  );
}

export default App;
