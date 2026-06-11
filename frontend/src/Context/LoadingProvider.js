import {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import axios, { axiosPrivate } from "../api/axios";

const LoadingContext = createContext(null);

export function LoadingProvider({ children }) {
  const [loadingCount, setLoadingCount] = useState(0);
  const pendingCount = useRef(0);

  useEffect(() => {
    const start = (config) => {
      pendingCount.current += 1;
      setLoadingCount(pendingCount.current);
      return config;
    };

    const finish = () => {
      pendingCount.current = Math.max(0, pendingCount.current - 1);
      setLoadingCount(pendingCount.current);
    };

    const req1 = axios.interceptors.request.use(
      (config) => start(config),
      (error) => {
        finish();
        return Promise.reject(error);
      },
    );

    const res1 = axios.interceptors.response.use(
      (response) => {
        finish();
        return response;
      },
      (error) => {
        finish();
        return Promise.reject(error);
      },
    );

    const req2 = axiosPrivate.interceptors.request.use(
      (config) => start(config),
      (error) => {
        finish();
        return Promise.reject(error);
      },
    );

    const res2 = axiosPrivate.interceptors.response.use(
      (response) => {
        finish();
        return response;
      },
      (error) => {
        finish();
        return Promise.reject(error);
      },
    );

    return () => {
      axios.interceptors.request.eject(req1);
      axios.interceptors.response.eject(res1);
      axiosPrivate.interceptors.request.eject(req2);
      axiosPrivate.interceptors.response.eject(res2);
    };
  }, []);

  const value = useMemo(
    () => ({ isLoading: loadingCount > 0 }),
    [loadingCount],
  );

  return (
    <LoadingContext.Provider value={value}>{children}</LoadingContext.Provider>
  );
}

export function useLoading() {
  const context = useContext(LoadingContext);
  if (!context) {
    throw new Error("useLoading must be used inside LoadingProvider");
  }
  return context;
}
