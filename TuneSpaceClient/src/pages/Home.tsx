import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/Carousel"

import Feed from "../components/home/Feed"
import { Card, CardContent } from "@/components/Card"
import { IoIosTrendingUp } from "react-icons/io";
import CreatePostDialog from "@/components/home/CreatePostDialog";


const Home = () => {
  return (
    <div className="text-white flex">
      <div className="flex flex-col gap-4">
        <CreatePostDialog/>
        <Feed/>
      </div>
      
      <div className="px-4 w-[600px] fixed right-0 flex flex-col gap-4">
        <div className="pl-5 flex items-center gap-1">
          <IoIosTrendingUp className="w-6 h-full"/>
          <h3 className="scroll-m-20 text-2xl font-semibold tracking-tight">Trending</h3>
        </div>
        
        <Carousel className="ml-11 w-full max-w-sm">
          <CarouselContent className="-ml-1">
            {Array.from({ length: 5 }).map((_, index) => (
              <CarouselItem key={index} className="pl-1 md:basis-1/2 lg:basis-1/3">
                <div className="p-1 flex flex-col">
                  <Card>
                    <CardContent className="flex aspect-square items-center justify-center p-6">
                      Image
                    </CardContent>
                  </Card>
                  <span className="text-2xl font-semibold">Song {index + 1}</span>
                </div>
              </CarouselItem>
            ))}
          </CarouselContent>
          <CarouselPrevious />
          <CarouselNext />
        </Carousel>
      </div>
    </div>
  )
}

export default Home
