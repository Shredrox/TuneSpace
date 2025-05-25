import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  getMerchandiseByBandId,
  getMerchandiseImage,
  createMerchandise,
} from "@/services/merchandise-service";
import Merchandise from "@/interfaces/Merchandise";

const useMerchandise = (bandId: string) => {
  const queryClient = useQueryClient();

  const {
    data: merchandise,
    isLoading,
    isError,
    error,
    refetch,
  } = useQuery({
    queryKey: ["merchandise", bandId],
    queryFn: async () => {
      if (!bandId) return [];

      const merchandiseData = await getMerchandiseByBandId(bandId);

      const merchandiseWithImages = await Promise.all(
        merchandiseData.map(async (item: Merchandise) => {
          try {
            const imageUrl = await getMerchandiseImage(item.id);
            return { ...item, imageUrl };
          } catch (error) {
            console.error(
              `Failed to fetch image for merchandise ${item.id}:`,
              error
            );
            return item;
          }
        })
      );

      return merchandiseWithImages;
    },
    enabled: !!bandId,
  });

  const {
    mutateAsync: addMerchandise,
    isPending: isCreating,
    isError: isCreateError,
    error: createError,
  } = useMutation({
    mutationFn: createMerchandise,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["merchandise", bandId] });
    },
  });

  return {
    merchandise,
    isLoading,
    isError,
    error,
    refetch,
    addMerchandise,
    isCreating,
    isCreateError,
    createError,
  };
};

export default useMerchandise;
