"use client";

import { useParams, useRouter } from "next/navigation";
import { useState } from "react";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/shadcn/card";
import { Button } from "@/components/shadcn/button";
import { Input } from "@/components/shadcn/input";
import { Textarea } from "@/components/shadcn/textarea";
import { ArrowLeft, Send } from "lucide-react";
import Link from "next/link";
import useToast from "@/hooks/useToast";
import useForumCategory from "@/hooks/query/useForumCategory";
import useForumThreads from "@/hooks/query/useForumThreads";

export default function NewThreadPage() {
  const params = useParams();
  const router = useRouter();
  const categoryId = params.categoryId as string;

  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");

  const { category, isLoading } = useForumCategory(categoryId);
  const { createThread, isCreating } = useForumThreads(categoryId);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (title.trim() === "" || content.trim() === "") {
      useToast("Please fill in all fields", 3000);
      return;
    }

    try {
      await createThread({
        title,
        content,
        categoryId,
      });
      useToast("Thread created successfully", 3000);
      router.push(`/forums/${categoryId}`);
    } catch (error) {
      useToast("Failed to create thread. Please try again.", 3000);
    }
  };

  if (isLoading) {
    return (
      <div className="container max-w-3xl mx-auto py-8">
        <div className="flex justify-center py-20">
          <p>Loading...</p>
        </div>
      </div>
    );
  }

  if (!category) {
    return (
      <div className="container max-w-3xl mx-auto py-8">
        <div className="flex justify-center py-20">
          <p>Category not found</p>
        </div>
      </div>
    );
  }

  return (
    <div className="container max-w-3xl mx-auto py-8">
      <Link
        href={`/forums/${categoryId}`}
        className="inline-flex items-center text-primary hover:text-primary/80 mb-4"
      >
        <ArrowLeft className="h-4 w-4 mr-2" />
        Back to {category.name}
      </Link>

      <Card>
        <CardHeader>
          <CardTitle>Create New Thread in {category.name}</CardTitle>
        </CardHeader>

        <form onSubmit={handleSubmit}>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <label htmlFor="title" className="text-sm font-medium">
                Thread Title
              </label>
              <Input
                id="title"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder="Enter a descriptive title"
                disabled={isCreating}
                required
              />
            </div>

            <div className="space-y-2">
              <label htmlFor="content" className="text-sm font-medium">
                Content
              </label>
              <Textarea
                id="content"
                value={content}
                onChange={(e) => setContent(e.target.value)}
                placeholder="Share your thoughts, questions, or ideas..."
                className="min-h-[200px]"
                disabled={isCreating}
                required
              />
            </div>
          </CardContent>

          <CardFooter className="flex justify-between border-t py-4">
            <div className="text-sm text-muted-foreground">
              Please follow our community guidelines
            </div>
            <Button type="submit" disabled={isCreating}>
              {isCreating ? (
                "Creating thread..."
              ) : (
                <>
                  <Send className="h-4 w-4 mr-2" />
                  Create Thread
                </>
              )}
            </Button>
          </CardFooter>
        </form>
      </Card>
    </div>
  );
}
