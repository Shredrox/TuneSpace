import { toast } from "sonner";

const useToast = (message: string, duration: number = 10000) => {
  toast(message, {
    duration: duration,
    style: { height: "100px" },
    cancel: {
      label: "Dismiss",
      onClick: () => {},
    },
  });
};

export default useToast;
