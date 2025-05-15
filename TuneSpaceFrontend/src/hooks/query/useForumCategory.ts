"use client";

import { useQuery } from "@tanstack/react-query";
import { fetchCategory } from "@/services/forum-service";

const useForumCategory = (categoryId: string) => {
  const {
    data: category,
    isLoading,
    isError,
    error,
    refetch,
  } = useQuery({
    queryKey: ["forumCategory", categoryId],
    queryFn: () => fetchCategory(categoryId),
    enabled: !!categoryId,
  });

  return {
    category,
    isLoading,
    isError,
    error,
    refetch,
  };
};

export default useForumCategory;
