import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  getEvents,
  createEvent,
  getEventsByBandId,
} from "@/services/events-service";

const useEvents = (bandId: string) => {
  const queryClient = useQueryClient();

  const {
    data: events,
    isEventsLoading,
    isEventsError,
    eventsError,
  } = useQuery({
    queryKey: ["events"],
    queryFn: getEvents,
  });

  const {
    data: bandEvents,
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ["bandEvents", bandId],
    queryFn: () => getEventsByBandId(bandId),
    enabled: !!bandId,
  });

  const {
    mutateAsync: addEvent,
    isPending: isCreating,
    isError: isCreateError,
    error: createError,
  } = useMutation({
    mutationFn: createEvent,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["bandEvents", bandId] });
    },
  });

  return {
    events,
    bandEvents,
    isLoading,
    isError,
    error,
    addEvent,
    isCreating,
    isCreateError,
    createError,
  };
};

export default useEvents;
