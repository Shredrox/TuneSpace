"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Dispatch, SetStateAction, useEffect, useState } from "react";
import FormInput from "@/components/form-input";
import { RegisterInputs, registerSchema } from "@/schemas/register.schema";
import {
  BASE_URL,
  ENDPOINTS,
  SPOTIFY_ENDPOINTS,
  UserRole,
} from "@/utils/constants";
import { FaSpotify } from "react-icons/fa";
import { useRouter } from "next/navigation";
import { Button } from "../shadcn/button";
import useAuth from "@/hooks/useAuth";
import httpClient from "@/services/http-client";

const Register = ({
  setFormStep,
  setIsLogin,
}: {
  setFormStep: Dispatch<SetStateAction<number>>;
  setIsLogin: Dispatch<SetStateAction<boolean>>;
}) => {
  const [error, setError] = useState("");
  const router = useRouter();

  const { setAuth } = useAuth();

  const handleSpotifyLogin = () => {
    router.push(`${BASE_URL}/${SPOTIFY_ENDPOINTS.LOGIN}`);
  };

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<RegisterInputs>({
    resolver: zodResolver(registerSchema),
  });

  const watchName = watch("name");
  const watchEmail = watch("email");
  const watchPassword = watch("password");
  const watchConfirmPassword = watch("confirmPassword");

  useEffect(() => {
    setError("");
  }, [watchName, watchEmail, watchPassword, watchConfirmPassword]);

  const onSubmit = async (data: RegisterInputs) => {
    const { confirmPassword, ...rest } = data;
    const request = { ...rest, role: UserRole.Listener };

    try {
      const response = await httpClient.post(
        `${BASE_URL}/${ENDPOINTS.REGISTER}`,
        JSON.stringify(request),
        {
          headers: { "Content-Type": "application/json" },
        }
      );

      setFormStep(1);
      setAuth({ id: response?.data });
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
          <h1 className="text-2xl font-bold">Welcome to TuneSpace</h1>
          <p className="text-balance text-muted-foreground">
            Enter your info below to create your account
          </p>
        </div>
        <FormInput
          type="text"
          placeholder="Name"
          register={register}
          name="name"
          error={errors.name?.message}
        />
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
        <FormInput
          type="password"
          placeholder="Confirm Password"
          register={register}
          name="confirmPassword"
          error={errors.confirmPassword?.message}
        />
        <Button type="submit" className="w-full">
          Register
        </Button>
        <div className="relative text-center text-sm after:absolute after:inset-0 after:top-1/2 after:z-0 after:flex after:items-center after:border-t after:border-border">
          <span className="relative z-10 bg-background px-2 text-muted-foreground">
            Or register with
          </span>
        </div>
        <div className="grid grid-cols-1 gap-4">
          <Button
            variant="outline"
            className="w-full"
            onClick={handleSpotifyLogin}
          >
            <FaSpotify className="text-[#1DB954]" size={25} />
            <span className="sr-only">Login with Spotify</span>
          </Button>
        </div>
        <p className="text-orange-600">{error}</p>
        <div className="text-center text-sm">
          Already have an account?{" "}
          <a
            onClick={() => setIsLogin(true)}
            href="#"
            className="underline underline-offset-4"
          >
            Log in
          </a>
        </div>
      </div>
    </form>
  );
};

export default Register;
