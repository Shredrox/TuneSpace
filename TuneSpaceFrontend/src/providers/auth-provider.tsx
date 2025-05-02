"use client";

import { ReactNode, createContext, useState } from "react";
import AuthContextType from "@/interfaces/AuthContextType";
import Auth from "@/interfaces/Auth";

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthContext = createContext<AuthContextType | null>(null);

const AuthProvider = ({ children }: AuthProviderProps) => {
  const [auth, setAuth] = useState<Auth>({});

  const contextValue: AuthContextType = {
    auth,
    setAuth,
  };

  return (
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
};

export default AuthProvider;
