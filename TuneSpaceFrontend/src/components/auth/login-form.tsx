"use client";

import { Dispatch, SetStateAction, useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import FormInput from "../form-input";
import { LoginInputs, loginSchema } from "@/schemas/login.schema";
import { useRouter } from "next/navigation";
import { BASE_URL, ENDPOINTS, SPOTIFY_ENDPOINTS } from "@/utils/constants";
import { Button } from "../shadcn/button";
import { FaSpotify } from "react-icons/fa";
import useAuth from "@/hooks/useAuth";
import httpClient from "@/services/http-client";

const Login = ({
  shouldRedirect,
  setIsLogin,
}: {
  shouldRedirect: boolean;
  setIsLogin: Dispatch<SetStateAction<boolean>>;
}) => {
  const { setAuth } = useAuth();

  const [error, setError] = useState("");
  const router = useRouter();

  useEffect(() => {
    const searchParams = new URLSearchParams(window.location.search);
    const tokenExpiryTime = searchParams.get("tokenExpiryTime");

    if (tokenExpiryTime) {
      setAuth((prevAuth) => ({
        ...prevAuth,
        spotifyTokenExpiry: tokenExpiryTime,
      }));

      router.replace(window.location.pathname);
    }
  }, [setAuth, router]);

  const handleSpotifyLogin = () => {
    router.push(`${BASE_URL}/${SPOTIFY_ENDPOINTS.LOGIN}`);
  };

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<LoginInputs>({
    resolver: zodResolver(loginSchema),
  });

  const watchEmail = watch("email");
  const watchPassword = watch("password");

  useEffect(() => {
    setError("");
  }, [watchEmail, watchPassword]);

  const onSubmit = async (data: LoginInputs) => {
    try {
      const response = await httpClient.post(
        `${BASE_URL}/${ENDPOINTS.LOGIN}`,
        JSON.stringify(data),
        {
          headers: { "Content-Type": "application/json" },
        }
      );
      setAuth({
        id: response?.data?.id,
        username: response?.data?.username,
        accessToken: response?.data?.accessToken,
        role: response?.data?.role,
      });

      if (shouldRedirect) {
        router.push("/");
      }
    } catch (error: any) {
      handleRequestError(error);
    }
  };

  const handleRequestError = (error: any) => {
    if (!error?.response) {
      setError("No response from server");
    } else {
      switch (error.response.status) {
        case 401:
          setError("Incorrect email or password");
          break;
        case 400:
          setError("An error occurred");
          break;
        default:
          setError("Unexpected error occurred");
      }
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} noValidate className="p-6 md:p-8">
      <div className="flex flex-col gap-6">
        <div className="flex flex-col items-center text-center">
          <h1 className="text-2xl font-bold">Welcome back</h1>
          <p className="text-balance text-muted-foreground">
            Login to your TuneSpace account
          </p>
        </div>
        <FormInput
          type="email"
          placeholder="Email"
          register={register}
          name="email"
          error={errors.email?.message}
        />
        <FormInput
          type="password"
          placeholder="Password"
          register={register}
          name="password"
          error={errors.password?.message}
        />
        <Button type="submit" className="w-full">
          Login
        </Button>
        <p className="text-orange-600">{error}</p>
        <div className="relative text-center text-sm after:absolute after:inset-0 after:top-1/2 after:z-0 after:flex after:items-center after:border-t after:border-border">
          <span className="relative z-10 bg-background px-2 text-muted-foreground">
            Or continue with
          </span>
        </div>
        <div className="grid grid-cols-2 gap-4">
          <Button variant="outline" className="w-full">
            <svg
              className="w-6"
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
            >
              <path
                d="M12.152 6.896c-.948 0-2.415-1.078-3.96-1.04-2.04.027-3.91 1.183-4.961 3.014-2.117 3.675-.546 9.103 1.519 12.09 1.013 1.454 2.208 3.09 3.792 3.039 1.52-.065 2.09-.987 3.935-.987 1.831 0 2.35.987 3.96.948 1.637-.026 2.676-1.48 3.676-2.948 1.156-1.688 1.636-3.325 1.662-3.415-.039-.013-3.182-1.221-3.22-4.857-.026-3.04 2.48-4.494 2.597-4.559-1.429-2.09-3.623-2.324-4.39-2.376-2-.156-3.675 1.09-4.61 1.09zM15.53 3.83c.843-1.012 1.4-2.427 1.245-3.83-1.207.052-2.662.805-3.532 1.818-.78.896-1.454 2.338-1.273 3.714 1.338.104 2.715-.688 3.559-1.701"
                fill="currentColor"
              />
            </svg>
            <span className="sr-only">Login with Apple</span>
          </Button>
          <Button
            variant="outline"
            className="w-full"
            onClick={handleSpotifyLogin}
          >
            <FaSpotify className="text-[#1DB954]" size={25} />
            <span className="sr-only">Login with Spotify</span>
          </Button>
        </div>
        <div className="text-center text-sm">
          Don&apos;t have an account?{" "}
          <a
            onClick={() => setIsLogin(false)}
            href="#"
            className="underline underline-offset-4"
          >
            Sign up
          </a>
        </div>
      </div>
    </form>
  );
};

export default Login;
