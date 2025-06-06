"use client";

import { useEffect, useState } from "react";
import { useSearchParams, useRouter } from "next/navigation";
import { confirmEmail } from "@/services/auth-service";
import { ROUTES } from "@/utils/constants";
import { CheckCircle, XCircle, Loader2 } from "lucide-react";

export default function ConfirmEmailPage() {
  const [status, setStatus] = useState<"loading" | "success" | "error">(
    "loading"
  );
  const [message, setMessage] = useState("");
  const searchParams = useSearchParams();
  const router = useRouter();

  useEffect(() => {
    const confirmUserEmail = async () => {
      const userId = searchParams.get("userId");
      const token = searchParams.get("token");

      if (!userId || !token) {
        setStatus("error");
        setMessage(
          "Invalid confirmation link. Please check your email for the correct link."
        );
        return;
      }

      try {
        const response = await confirmEmail(userId, token);
        setStatus("success");
        setMessage(response.message);

        setTimeout(() => {
          router.push(ROUTES.LOGIN);
        }, 3000);
      } catch (error: any) {
        setStatus("error");
        if (error?.response?.data?.message) {
          setMessage(error.response.data.message);
        } else if (error?.response?.status === 400) {
          setMessage(
            "Invalid or expired confirmation token. Please request a new confirmation email."
          );
        } else {
          setMessage(
            "An error occurred while confirming your email. Please try again."
          );
        }
      }
    };

    confirmUserEmail();
  }, [searchParams, router]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-indigo-900 via-purple-900 to-pink-900 flex items-center justify-center p-4">
      <div className="max-w-md w-full bg-white/10 backdrop-blur-md rounded-2xl p-8 shadow-xl border border-white/20">
        <div className="text-center">
          <div className="mb-6">
            {status === "loading" && (
              <Loader2 className="w-16 h-16 text-indigo-400 animate-spin mx-auto" />
            )}
            {status === "success" && (
              <CheckCircle className="w-16 h-16 text-green-400 mx-auto" />
            )}
            {status === "error" && (
              <XCircle className="w-16 h-16 text-red-400 mx-auto" />
            )}
          </div>

          <h1 className="text-2xl font-bold text-white mb-4">
            {status === "loading" && "Confirming your email..."}
            {status === "success" && "Email Confirmed!"}
            {status === "error" && "Confirmation Failed"}
          </h1>

          <p className="text-gray-300 mb-6">{message}</p>

          {status === "success" && (
            <p className="text-sm text-gray-400">
              Redirecting you to login in a few seconds...
            </p>
          )}

          {status === "error" && (
            <div className="space-y-3">
              <button
                onClick={() => router.push(ROUTES.LOGIN)}
                className="w-full bg-indigo-600 hover:bg-indigo-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
              >
                Go to Login
              </button>
              <button
                onClick={() => router.push(ROUTES.SIGNUP)}
                className="w-full bg-gray-600 hover:bg-gray-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
              >
                Back to Registration
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
