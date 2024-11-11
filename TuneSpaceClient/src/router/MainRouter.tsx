import { Route, createBrowserRouter, createRoutesFromElements } from "react-router-dom";
import Landing from "../pages/Landing";
import MainLayout from "../layouts/MainLayout";
import Home from "../pages/Home";
import ProtectedRoute from "../routes/ProtectedRoute";
import Profile from "../pages/Profile";
import Chats from "../pages/Chats";
import Playlists from "../pages/Playlists";

export const MainRouter = createBrowserRouter(
  createRoutesFromElements(
    <Route>
      <Route path="/" element={<Landing/>}/>
      <Route element={<MainLayout/>}>
        <Route element={<ProtectedRoute/>}>
         
        </Route>
        <Route path="/home" element={<Home/>}/>
        <Route path="/playlists" element={<Playlists/>}/>
        <Route path="/chats" element={<Chats/>}/>
        <Route path="/profile/:username" element={<Profile/>}/>
      </Route>
    </Route>
  )
);
