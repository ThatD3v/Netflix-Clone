import { Outlet } from "react-router-dom";
import Loader from "./Loader";
import { useLoading } from "../Context/LoadingProvider";

function Layout() {
  const { isLoading } = useLoading();

  return (
    <>
      <Outlet />
      {isLoading && <Loader />}
    </>
  );
}

export default Layout;
