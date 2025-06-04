import { useMutation } from "@tanstack/react-query";
import { login, LoginData } from "@/services/auth-service";
import useAuth from "./useAuth";

const useLogin = () => {
  const { setAuth } = useAuth();

  const handleLogin = useMutation({
    mutationFn: (data: LoginData) => login(data),
    onSuccess: (userData) => {
      setAuth(userData);
    },
  });

  return {
    login: handleLogin.mutateAsync,
    isLoading: handleLogin.isPending,
    error: handleLogin.error,
  };
};

export default useLogin;
