import { useState, useEffect, useCallback } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import httpClient from "@/services/http-client";
import useSocket from "@/hooks/useSocket";
import useAuth from "@/hooks/auth/useAuth";
import { getUserNotifications } from "@/services/notification-service";

const useNotifications = () => {
  const queryClient = useQueryClient();
  const { auth } = useAuth();
  const { setUserNotifications } = useSocket();

  const [unreadCount, setUnreadCount] = useState(0);

  const {
    data: notifications,
    isLoading: isNotificationsLoading,
    isError: isNotificationsError,
    error: notificationsError,
    refetch,
  } = useQuery({
    queryKey: ["notifications", auth.username],
    queryFn: () => getUserNotifications(auth.username!),
    enabled: !!auth.username,
  });

  useEffect(() => {
    if (notifications) {
      setUserNotifications(notifications);
    }
  }, [notifications]);

  const markAsRead = useMutation({
    mutationFn: async (notificationId: string) => {
      return httpClient.put(`/Notification/mark-as-read/${notificationId}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["notifications", auth.username],
      });
    },
  });

  const markAllAsRead = useMutation({
    mutationFn: async () => {
      return httpClient.put(`/Notification/mark-all-as-read/${auth.username}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["notifications", auth.username],
      });
    },
  });

  const deleteNotification = useMutation({
    mutationFn: async (notificationId: string) => {
      return httpClient.delete(`/Notification/${notificationId}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["notifications", auth.username],
      });
    },
  });

  const handleMarkAsRead = useCallback(
    (notificationId: string) => {
      markAsRead.mutate(notificationId);
    },
    [markAsRead]
  );

  const handleMarkAllAsRead = useCallback(() => {
    markAllAsRead.mutate();
  }, [markAllAsRead]);

  const handleDeleteNotification = useCallback(
    (notificationId: string) => {
      deleteNotification.mutate(notificationId);
    },
    [deleteNotification]
  );

  return {
    notifications,
    unreadCount,
    isLoading: isNotificationsLoading,
    isError: isNotificationsError,
    error: notificationsError,
    refetch,
    markAsRead: handleMarkAsRead,
    markAllAsRead: handleMarkAllAsRead,
    deleteNotification: handleDeleteNotification,
  };
};

export default useNotifications;
