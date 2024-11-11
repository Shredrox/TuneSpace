import Auth from "./Auth";

export default interface AuthContextType{
  auth: Auth;
  setAuth: React.Dispatch<React.SetStateAction<Auth>>;
}
