import { useMutation, useQueryClient } from "@tanstack/react-query";
import { logout } from "@/services/auth-service";
import useAuth from "./useAuth";

const useLogout = () => {
  const { clearAuth } = useAuth();
  const queryClient = useQueryClient();

  const handleLogout = useMutation({
    mutationFn: () => logout(),
    onSuccess: () => {
      clearAuth();
      queryClient.clear();
    },
  });

  return {
    logout: handleLogout.mutateAsync,
    isLoading: handleLogout.isPending,
    error: handleLogout.error,
  };
};

export default useLogout;
