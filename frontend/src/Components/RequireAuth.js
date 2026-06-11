import { Navigate, Outlet, useLocation } from "react-router-dom";
import useAuth from "../Hooks/useAuth";

function RequireAuth() {
  const { auth } = useAuth();
  const location = useLocation();
  return auth?.email ? (
    <Outlet />
  ) : (
    <Navigate to="/check" state={{ from: location }} replace />
  );
}

export default RequireAuth;
