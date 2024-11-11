import axios from "../axios/axios";
import useAuth from "./useAuth";

const useLogout = () =>{
  const { setAuth } = useAuth();

  const logout = async () => {
    setAuth({}); 
    try{
      //const response = await axios.post('auth/logout',{});
    }catch(error){
      console.log(error);
    }
  }

  return logout;
}

export default useLogout
