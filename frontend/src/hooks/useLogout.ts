import httpClient from "@/services/http-client";
import useAuth from "./useAuth";
import { ENDPOINTS } from "@/utils/constants";

const useLogout = () => {
  const { setAuth } = useAuth();

  const logout = async () => {
    setAuth({});

    try {
      await httpClient.post(`${ENDPOINTS.LOGOUT}`, {});
    } catch (error) {
      console.log(error);
    }
  };

  return logout;
};

export default useLogout;
