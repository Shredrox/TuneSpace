/* eslint-disable @typescript-eslint/no-explicit-any */
"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import FormInput from "../form-input";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { Textarea } from "../shadcn/textarea";
import { Button } from "../shadcn/button";
import {
  BandRegisterInputs,
  bandRegisterSchema,
} from "@/schemas/band-register.schema";
import {
  SelectContent,
  SelectTrigger,
  Select,
  SelectItem,
  SelectValue,
} from "../shadcn/select";
import useLocationData from "@/hooks/useLocationData";
import { Input } from "../shadcn/input";
import { Avatar, AvatarFallback, AvatarImage } from "../shadcn/avatar";
import { registerBand } from "@/actions/band";
import useAuth from "@/hooks/auth/useAuth";
import { toast } from "sonner";
import { Card, CardContent } from "../shadcn/card";
import { Upload, Music, MapPin, FileText } from "lucide-react";
import { useQueryClient } from "@tanstack/react-query";

const BandRegistrationForm = ({ locationData }: { locationData: any }) => {
  const [imagePreview, setImagePreview] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const { auth } = useAuth();

  const router = useRouter();
  const queryClient = useQueryClient();

  const {
    selectedCountry,
    setSelectedCountry,
    selectedCity,
    setSelectedCity,
    countries,
    cities,
  } = useLocationData(locationData);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<BandRegisterInputs>({
    resolver: zodResolver(bandRegisterSchema),
  });

  const onSubmit = async (data: BandRegisterInputs) => {
    if (!auth?.id) {
      setError("You must be logged in to create a band profile");
      return;
    }

    setIsSubmitting(true);
    setError("");

    const { ...request } = data;

    const combinedRequest = {
      ...request,
      location: `${selectedCity}, ${selectedCountry}`,
      picture: request.picture[0],
      userId: auth.id,
    };

    try {
      await registerBand(combinedRequest);

      queryClient.invalidateQueries({
        queryKey: ["band", auth.id],
      });

      toast.success("Band profile created successfully! Welcome to TuneSpace!");
      router.push("/band/dashboard");
    } catch (error: any) {
      handleRequestError(error);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleRequestError = (error: any) => {
    if (!error?.response) {
      setError("No response from server. Please check your connection.");
    } else {
      switch (error.response.status) {
        case 409:
          setError(
            "Band name is already taken. Please choose a different name."
          );
          break;
        case 400:
          setError("Please check your input and try again.");
          break;
        default:
          setError("An unexpected error occurred. Please try again.");
      }
    }
  };

  return (
    <div className="w-full max-w-2xl mx-auto">
      <form onSubmit={handleSubmit(onSubmit)} noValidate className="space-y-6">
        <Card className="p-6 border-dashed border-2 border-muted-foreground/25 hover:border-primary/50 transition-colors">
          <CardContent className="p-0">
            <div className="flex flex-col items-center space-y-4">
              <div className="flex items-center gap-2 text-lg font-medium">
                <Upload className="h-5 w-5 text-primary" />
                Band Picture
              </div>

              {imagePreview ? (
                <Avatar className="w-32 h-32 rounded-lg border-4 border-primary/20">
                  <AvatarImage src={imagePreview} className="object-cover" />
                  <AvatarFallback className="text-2xl bg-primary/10">
                    <Music className="h-8 w-8" />
                  </AvatarFallback>
                </Avatar>
              ) : (
                <div className="w-32 h-32 rounded-lg border-4 border-dashed border-muted-foreground/30 flex items-center justify-center bg-muted/20">
                  <Upload className="h-8 w-8 text-muted-foreground/50" />
                </div>
              )}

              <Input
                type="file"
                accept="image/*"
                {...register("picture")}
                onChange={(e) => {
                  if (e.target.files?.[0]) {
                    setImagePreview(URL.createObjectURL(e.target.files[0]));
                  }
                }}
                className="max-w-xs cursor-pointer"
              />

              {errors.picture && (
                <p className="text-sm text-destructive font-medium">
                  {errors.picture.message}
                </p>
              )}

              <p className="text-xs text-muted-foreground text-center">
                Upload a high-quality image that represents your band
              </p>
            </div>
          </CardContent>
        </Card>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="space-y-2">
            <label className="text-sm font-medium flex items-center gap-2">
              <Music className="h-4 w-4 text-primary" />
              Band Name
            </label>
            <FormInput
              type="text"
              placeholder="Enter your band name"
              register={register}
              name="name"
              error={errors.name?.message}
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium flex items-center gap-2">
              <Music className="h-4 w-4 text-primary" />
              Genre
            </label>
            <FormInput
              type="text"
              placeholder="e.g., Rock, Jazz, Electronic"
              register={register}
              name="genre"
              error={errors.genre?.message}
            />
          </div>
        </div>

        <div className="space-y-2">
          <label className="text-sm font-medium flex items-center gap-2">
            <FileText className="h-4 w-4 text-primary" />
            Band Description
          </label>
          <Textarea
            {...register("description")}
            className="min-h-[120px] resize-none"
            placeholder="Tell us about your band's story, style, and what makes you unique..."
          />
          {errors.description && (
            <p className="text-sm text-destructive font-medium">
              {errors.description.message}
            </p>
          )}
        </div>

        <div className="space-y-4">
          <label className="text-sm font-medium flex items-center gap-2">
            <MapPin className="h-4 w-4 text-primary" />
            Location
          </label>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Select onValueChange={(value) => setSelectedCountry(value)}>
                <SelectTrigger>
                  <SelectValue placeholder="Select Country" />
                </SelectTrigger>
                <SelectContent>
                  {countries.map((country: any) => (
                    <SelectItem key={country.value} value={country.value}>
                      {country.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            {selectedCountry && (
              <div className="space-y-2">
                <Select onValueChange={(value) => setSelectedCity(value)}>
                  <SelectTrigger>
                    <SelectValue placeholder="Select City" />
                  </SelectTrigger>
                  <SelectContent>
                    {cities.map((city: any) => (
                      <SelectItem key={city.value} value={city.value}>
                        {city.label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            )}
          </div>
        </div>

        {error && (
          <Card className="border-destructive/50 bg-destructive/5">
            <CardContent className="p-4">
              <p className="text-sm text-destructive font-medium">{error}</p>
            </CardContent>
          </Card>
        )}

        <Button
          type="submit"
          className="w-full h-12 text-lg font-medium bg-gradient-to-r from-purple-600 to-pink-600 hover:from-purple-700 hover:to-pink-700 transition-all duration-300 transform"
          disabled={isSubmitting}
        >
          {isSubmitting ? (
            <div className="flex items-center gap-2">
              <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              Creating Your Band Profile...
            </div>
          ) : (
            "Create Band Profile"
          )}
        </Button>
      </form>
    </div>
  );
};

export default BandRegistrationForm;
