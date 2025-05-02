import { useQuery } from "@tanstack/react-query";
import { getEvents } from "@/services/events-service";

const useEvents = () => {
  const {
    data: events,
    isLoading,
    isError,
    error,
    refetch,
  } = useQuery({
    queryKey: ["events"],
    queryFn: getEvents,
    refetchOnWindowFocus: false,
    staleTime: 5 * 60 * 1000,
  });

  return {
    events,
    isLoading,
    isError,
    error,
    refetch,
  };
};

export default useEvents;
