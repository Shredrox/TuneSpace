import { create } from "zustand";
import Auth from "@/interfaces/Auth";

interface AuthState {
  auth: Partial<Auth>;
  setAuth: (auth: Auth) => void;
  updateAuth: (updates: Partial<Auth>) => void;
  clearAuth: () => void;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  auth: {},

  setAuth: (auth: Auth) => {
    set({ auth });
  },

  updateAuth: (updates: Partial<Auth>) => {
    const currentAuth = get().auth;
    set({ auth: { ...currentAuth, ...updates } });
  },

  clearAuth: () => {
    set({ auth: {} });
  },
}));
