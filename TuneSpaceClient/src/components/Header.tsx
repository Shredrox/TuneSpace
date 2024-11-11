import { useNavigate } from "react-router-dom";
import useLogout from "../hooks/useLogout";
import { ModeToggle } from "./ModeToggle";
import { Avatar, AvatarFallback, AvatarImage } from "./Avatar";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/DropdownMenu"
import {
  LogOut,
  User,
} from "lucide-react"
import { FaPlay, FaPause, FaVolumeUp } from "react-icons/fa";
import { IoIosSkipBackward, IoIosSkipForward } from "react-icons/io";
import SearchBar from "./SearchBar";
import { Slider } from "./Slider";
import SpotifySearchBar from "./SpotifySearchBar";

const Header = () => {
  const navigate = useNavigate();

  const logout = useLogout();

  const handleLogout = async () => {
    await logout();
    navigate('/');
  }

  return (
    <header className="bg-[#18191b] backdrop-blur 
    border-b supports-[backdrop-filter]:bg-background/60 
    w-full flex justify-between items-center h-16 p-4 fixed top-0 z-50">
      <h3 className="scroll-m-20 text-2xl font-semibold tracking-tight">TuneSpace</h3>
      <div className="w-1/3 flex relative">
        {/* <SearchBar/> */}
        <SpotifySearchBar/>
      </div>
      <div className="w-1/3 h-full flex items-center gap-3">
        <img src="https://picsum.photos/1000/500" className="w-12 h-12 object-cover rounded"/>
        <div className="flex flex-col min-w-fit">
          <span className="text-xl">Dying Star</span>
          <span className="text-sm">Periphery</span>
        </div>
        
        <div className="w-[600px] h-full flex justify-center items-center gap-4">
          <Slider/>
          <div className="flex gap-4 items-center">
            <IoIosSkipBackward className="w-6 h-6"/>
            <FaPlay className="w-6 h-6"/>
            <IoIosSkipForward className="w-6 h-6"/>
          </div>
        </div>
        <FaVolumeUp className="w-12 h-12"/>
        <Slider className="w-48"/>
      </div>
      <div className="flex gap-2">
        <ModeToggle/>
        <DropdownMenu>
          <DropdownMenuTrigger>
            <Avatar>
              <AvatarImage src="https://github.com/shadcn.png"/>
              <AvatarFallback>CN</AvatarFallback>
            </Avatar>
          </DropdownMenuTrigger>
          <DropdownMenuContent onCloseAutoFocus={(e) => e.preventDefault()}>
            <DropdownMenuLabel>My Account</DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem onClick={() => navigate('/profile/hello')}>
              <User className="mr-2 h-4 w-4" />
              <span>Profile</span>
            </DropdownMenuItem>
            <DropdownMenuItem onClick={handleLogout}>
              <LogOut className="mr-2 h-4 w-4" />
              <span>Log out</span>
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </header>
  )
}

export default Header
