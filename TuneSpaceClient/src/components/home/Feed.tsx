import { Avatar, AvatarFallback, AvatarImage } from "../Avatar";
import { Badge } from "../Badge";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "../Card"
import { FaHeart, FaRegHeart, FaCommentAlt } from "react-icons/fa";

const Feed = () => {
  return (
    <div className="bg-[#0f1012] p-4 w-[650px] min-h-fit rounded-2xl flex flex-col gap-4">
      {Array.from({length: 5}).map((_,index) =>(
      <Card key={index} className="min-h-56 cursor-pointer">
        <CardHeader>
          <CardTitle>Periphery - Dying Star</CardTitle>
          <CardDescription className="flex items-center gap-1">
            <Avatar className="w-6 h-6">
              <AvatarImage src="https://github.com/shadcn.png"/>
              <AvatarFallback>CN</AvatarFallback>
            </Avatar>
            <p className="text-primary-foreground">Meesho</p>
          </CardDescription>
        </CardHeader>
        <CardContent>
          <img className="w-full h-[300px] object-cover rounded" src="https://picsum.photos/1000/500" alt="" />
          <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
            Etiam non tincidunt velit, non efficitur leo.
            Praesent non volutpat justo. Morbi ut lectus porta, 
            faucibus nibh vehicula, tincidunt lacus. 
            Ut congue mattis mauris, ut placerat nunc 
            lacinia vitae. Nunc mattis 
            ligula non magna blandit, ac condimentum tortor tristique. 
            Mauris hendrerit et magna vitae eleifend. Pellentesque in est ipsum. 
            Aliquam non enim eu nibh posuere varius ac at ex. 
            Vivamus ornare tempor orci, id ornare nulla malesuada eu. 
            Pellentesque faucibus enim non bibendum laoreet. 
          </p>
          <div>
            <Badge className="mr-1 mb-1">Metal</Badge>
            <Badge className="mr-1">Progressive</Badge>
            <Badge className="mr-1">Progressive</Badge>
            <Badge className="mr-1">Progressive</Badge>
            <Badge className="mr-1">Progressive</Badge>
            <Badge className="mr-1 mb-1">Metal</Badge>
            <Badge className="mr-1">Progressive</Badge>
            <Badge className="mr-1">Progressive</Badge>
            <Badge className="mr-1">Progressive</Badge>
          </div>
        </CardContent>
        <CardFooter>
          <div className="flex gap-4">
            <div className="flex items-center gap-2">
              <FaRegHeart className="w-5 h-5 cursor-pointer"/> Like
            </div>
            <div className="flex items-center gap-2">
              <FaCommentAlt className="w-5 h-5 cursor-pointer"/> Comment
            </div>
          </div>
        </CardFooter>
      </Card>
      ))}
    </div>
  )
}

export default Feed
