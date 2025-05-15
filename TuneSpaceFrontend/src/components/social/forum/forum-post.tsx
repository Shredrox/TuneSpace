import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
} from "@/components/shadcn/card";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@/components/shadcn/avatar";
import { Button } from "@/components/shadcn/button";
import { Badge } from "@/components/shadcn/badge";
import { Flag, MessageSquare, ThumbsUp, Share } from "lucide-react";
import { format, formatDistanceToNow } from "date-fns";
import ThreadPost from "@/interfaces/forum/ThreadPost";

interface ForumPostProps {
  post: ThreadPost;
  isOriginalPost?: boolean;
  onLike: () => void;
  onDislike: () => void;
}

const ForumPost = ({
  post,
  isOriginalPost = false,
  onLike,
  onDislike,
}: ForumPostProps) => {
  const {
    authorName,
    authorAvatar,
    authorRole,
    content,
    createdAt,
    likesCount,
    userHasLiked,
  } = post;

  const getRoleBadgeColor = (role?: string) => {
    switch (role) {
      case "band":
        return "bg-blue-500 text-white";
      case "admin":
        return "bg-red-500 text-white";
      case "moderator":
        return "bg-green-500 text-white";
      default:
        return "";
    }
  };

  return (
    <Card className={isOriginalPost ? "border-primary/30" : ""}>
      <CardHeader className="flex flex-row gap-4 pb-2">
        <div>
          <Avatar className="h-10 w-10">
            <AvatarImage src={authorAvatar} />
            <AvatarFallback>{authorName?.charAt(0)}</AvatarFallback>
          </Avatar>
        </div>
        <div className="flex flex-col">
          <div className="flex items-center gap-2">
            <span className="font-semibold">{authorName ?? ""}</span>
            {authorRole && authorRole !== "user" && (
              <Badge className={getRoleBadgeColor(authorRole)}>
                {authorRole.charAt(0).toUpperCase() + authorRole.slice(1)}
              </Badge>
            )}
            {isOriginalPost && <Badge variant="outline">Original Poster</Badge>}
          </div>
          <span className="text-xs text-muted-foreground">
            {formatDistanceToNow(new Date(createdAt), { addSuffix: true })}
            {" • "}
            {format(new Date(createdAt), "MMM d, yyyy 'at' HH:mm")}
          </span>
        </div>
      </CardHeader>
      <CardContent className="pt-2 pb-4">
        <div className="prose dark:prose-invert max-w-none">
          <p>{content}</p>
        </div>
      </CardContent>
      <CardFooter className="border-t pt-3 flex justify-between">
        <div className="flex items-center gap-4">
          <Button
            variant="ghost"
            size="sm"
            className="flex items-center gap-1"
            onClick={userHasLiked ? onDislike : onLike}
          >
            <ThumbsUp
              className={`h-4 w-4 ${
                userHasLiked ? "fill-primary text-primary" : ""
              }`}
            />
            <span>{likesCount || ""}</span>
          </Button>
          {/* TODO */}
          {/* <Button variant="ghost" size="sm" className="flex items-center gap-1">
            <MessageSquare className="h-4 w-4" />
            <span>Reply</span>
          </Button> */}
        </div>
        {/* TODO */}
        {/* <div className="flex items-center gap-2">
          <Button variant="ghost" size="sm" className="flex items-center gap-1">
            <Share className="h-4 w-4" />
            <span>Share</span>
          </Button>
          <Button variant="ghost" size="sm" className="flex items-center gap-1">
            <Flag className="h-4 w-4" />
            <span>Report</span>
          </Button>
        </div> */}
      </CardFooter>
    </Card>
  );
};

export default ForumPost;
