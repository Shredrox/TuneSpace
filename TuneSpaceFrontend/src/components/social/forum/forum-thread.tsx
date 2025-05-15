"use client";

import { useState } from "react";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardFooter,
} from "@/components/shadcn/card";
import { Button } from "@/components/shadcn/button";
import { Textarea } from "@/components/shadcn/textarea";
import { Reply } from "lucide-react";
import ForumPost from "./forum-post";
import ThreadPost from "@/interfaces/forum/ThreadPost";
import useForumThread from "@/hooks/query/useForumThread";

interface ForumThreadProps {
  threadId: string;
  title: string;
  categoryName: string;
  posts: ThreadPost[];
}

const ForumThread = ({
  threadId,
  title,
  categoryName,
  posts,
}: ForumThreadProps) => {
  const [replyContent, setReplyContent] = useState("");

  const { createPost, likePost, unlikePost, isCreating, isPending } =
    useForumThread(threadId);

  const handleSubmitReply = async () => {
    if (replyContent.trim() === "") {
      return;
    }
    try {
      await createPost({ content: replyContent, threadId });
      setReplyContent("");
    } catch (error) {
      console.error("Error creating post:", error);
    }
  };

  return (
    <div className="container max-w-5xl mx-auto py-8">
      <div className="mb-6">
        <div className="flex items-center gap-2 text-sm text-muted-foreground mb-2">
          <span>Forums</span> &gt; <span>{categoryName}</span>
        </div>
        <h1 className="text-3xl font-bold">{title}</h1>
      </div>

      <div className="space-y-6">
        {posts.map((post, index) => (
          <ForumPost
            key={post.id}
            post={post}
            isOriginalPost={index === 0}
            onLike={() => likePost(post.id)}
            onDislike={() => unlikePost(post.id)}
          />
        ))}
      </div>

      <Card className="mt-8">
        <CardHeader>
          <CardTitle className="text-lg">Post a Reply</CardTitle>
        </CardHeader>
        <CardContent>
          <Textarea
            placeholder="Write your response..."
            className="min-h-[150px]"
            value={replyContent}
            onChange={(e) => setReplyContent(e.target.value)}
          />
        </CardContent>
        <CardFooter className="flex justify-between">
          <div className="text-sm text-muted-foreground">
            Use respectful language and follow community guidelines
          </div>
          <Button
            onClick={handleSubmitReply}
            disabled={replyContent.trim() === ""}
          >
            <Reply className="h-4 w-4 mr-2" />
            Post Reply
          </Button>
        </CardFooter>
      </Card>
    </div>
  );
};

export default ForumThread;
