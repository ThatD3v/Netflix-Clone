import { Outlet } from "react-router-dom";
import HomeHeader from "./HomeHeader";

function HomeLayout() {
  return (
    <>
      <HomeHeader />
      <main>
        <Outlet />
      </main>
    </>
  );
}

export default HomeLayout;
