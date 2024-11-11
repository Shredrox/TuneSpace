import { Link, useLocation } from 'react-router-dom'
import { IoHome } from "react-icons/io5";
import { IoChatbubbles } from "react-icons/io5";
import { RiPlayListFill } from "react-icons/ri";
import { Button } from './Button';

const Sidebar = () => {
  const location = useLocation();
  
  const isActive = (path : string) =>{
    return location.pathname.includes(path);
  }

  const linksData = [
    { id: 1, to: '/home', text: 'Home', icon: <IoHome/> },
    { id: 2, to: '/playlists', text: 'Playlists', icon: <RiPlayListFill/> },
    { id: 3, to: '/chats', text: 'Chats', icon: <IoChatbubbles/> }
  ];

  return (
    <aside className="bg-secondary h-screen fixed top-16 left-0 p-4 min-w-[150px]">
      <ul className='flex flex-col gap-2'>
        {linksData.map((link) =>
          <li key={link.id}>
            <Button 
              key={link.id} 
              asChild
              variant={isActive(link.to) ? "default" : "outline"}
              className='flex gap-1'
              >
              <Link key={link.id} to={link.to}>
                {link.icon}
                {link.text}
              </Link>
            </Button>
          </li>
        )}
      </ul>
    </aside>
  )
}

export default Sidebar
