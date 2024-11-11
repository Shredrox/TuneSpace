import { useQuery } from "@tanstack/react-query";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Input } from "./Input";
import { Card } from "./Card";
import Loading from "./fallback/Loading";
import { Avatar, AvatarFallback, AvatarImage } from "./Avatar";
import { getUsersByNameSearch } from "@/services/userService";

const SearchBar = () => {
  const [search, setSearch] = useState('');

  const navigate = useNavigate();

  const {data: searchUsers, isError, error, refetch} = useQuery({ 
    queryKey: ["searchUsers", search],
    queryFn: () => getUsersByNameSearch(search),
    enabled: false,
    retry: 1,
  });

  useEffect(() => {
    const queryDelay = setTimeout(() => {
      if(search !== ''){
        refetch();
      }
    }, 1000)

    return () => clearTimeout(queryDelay)
  }, [search])

  return (
    <div className="w-full">
      <Input value={search} onChange={(e) => setSearch(e.target.value)} placeholder="🔎︎ Search"></Input>
      {search !== '' &&
      <Card className="w-full flex flex-col justify-center gap-3 
      top-12 max-h-[500px] absolute z-10 p-4 overflow-y-scroll overflow-auto rounded-2xl">
        {searchUsers?.length > 0 ?
        searchUsers?.map((user, index) =>
        <Card key={index} onClick={() => {navigate(`/profile/${user.name}`); setSearch('');}} className='p-4 flex items-center gap-3 cursor-pointer'>
          <Avatar>
            <AvatarImage src="https://github.com/shadcn.png"/>
            <AvatarFallback>{user.name}</AvatarFallback>
          </Avatar>
          {user.name}
        </Card>
        )
        : searchUsers?.length === 0 ? "User Not Found" 
        : isError ? <p>{error.message}</p>
        : <div className="h-24 flex justify-center items-center"><Loading/></div>
        }
      </Card>
      }
    </div>
  )
}

export default SearchBar
