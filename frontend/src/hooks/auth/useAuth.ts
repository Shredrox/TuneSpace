import { useAuthStore } from "@/stores/auth-store";

const useAuth = () => {
  const auth = useAuthStore((state) => state.auth);
  const setAuth = useAuthStore((state) => state.setAuth);
  const updateAuth = useAuthStore((state) => state.updateAuth);
  const clearAuth = useAuthStore((state) => state.clearAuth);

  return {
    auth,
    setAuth,
    updateAuth,
    clearAuth,
  };
};

export default useAuth;
