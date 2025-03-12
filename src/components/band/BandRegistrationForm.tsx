"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import FormInput from "../FormInput";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { Textarea } from "../shadcn/Textarea";
import { Button } from "../shadcn/Button";
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
} from "../shadcn/Select";
import useLocationData from "@/hooks/useLocationData";
import { Input } from "../shadcn/Input";
import { Avatar, AvatarFallback, AvatarImage } from "../shadcn/Avatar";
import { registerBand } from "@/actions/band-actions";

const BandRegistrationForm = ({ locationData }: { locationData: any }) => {
  const [formStep, setFormStep] = useState(0);
  const [imagePreview, setImagePreview] = useState("");
  const [error, setError] = useState("");

  const router = useRouter();

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
    const { ...request } = data;

    const combinedRequest = {
      ...request,
      location: `${selectedCity}, ${selectedCountry}`,
      picture: request.picture[0],
    };

    try {
      const bandId = await registerBand(combinedRequest);
      router.push(`/band/dashboard/${bandId}`);
    } catch (error: any) {
      handleRequestError(error);
    }
  };

  const handleRequestError = (error: any) => {
    if (!error?.response) {
      setError("No response from server");
    } else {
      switch (error.response.status) {
        case 409:
          setError("Username is already taken");
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
    <div className="flex flex-col items-center bg-card border rounded-xl p-4 w-[420px] h-fit">
      {formStep === 0 ? (
        <div className="flex flex-col gap-4 items-center justify-center w-2/3">
          <span>
            First, log in or create a user account. This account will be used to
            manage your band.
          </span>
          <AccessForm shouldRedirect={false} />
          <Button onClick={() => setFormStep(formStep + 1)}>Next</Button>
        </div>
      ) : (
        <div className="flex justify-center h-fit flex-col gap-4 items-center">
          <h2 className="text-xl">Enter band information</h2>
          <form
            onSubmit={handleSubmit(onSubmit)}
            noValidate
            className="flex flex-col justify-center items-center bg-gray-15 gap-2 p-4 white-98 w-full rounded-2xl"
          >
            <div className="w-full flex flex-col gap-1 items-center">
              <label className="w-full flex justify-left text-sm pl-2">
                Band Picture
              </label>
              <Input
                type="file"
                accept="image/*"
                {...register("picture")}
                onChange={(e) => {
                  if (e.target.files?.[0]) {
                    setImagePreview(URL.createObjectURL(e.target.files[0]));
                  }
                }}
              />
              {errors.picture && (
                <p className="text-red-500">{errors.picture.message}</p>
              )}
              {imagePreview && (
                <Avatar className="w-1/2 h-auto mt-2">
                  <AvatarImage src={imagePreview} width={500} />
                  <AvatarFallback>BandPicture</AvatarFallback>
                </Avatar>
              )}
            </div>

            <div className="w-full flex flex-col gap-1">
              <label className="w-full flex justify-left text-sm pl-2">
                Name
              </label>
              <FormInput
                type="text"
                placeholder="..."
                register={register}
                name="name"
                error={errors.name?.message}
              />
            </div>
            <div className="w-full flex flex-col gap-1">
              <label className="w-full flex justify-left text-sm pl-2">
                Genre
              </label>
              <FormInput
                type="text"
                placeholder="..."
                register={register}
                name="genre"
                error={errors.genre?.message}
              />
            </div>
            <div className="w-full flex flex-col gap-1">
              <label className="w-full flex justify-left text-sm pl-2">
                Description
              </label>
              <Textarea
                {...register("description")}
                className="w-full"
                placeholder="..."
              ></Textarea>
            </div>

            <div className="w-full flex flex-col gap-2">
              <label className="w-full flex justify-left text-sm pl-2">
                Location
              </label>
              <Select onValueChange={(value) => setSelectedCountry(value)}>
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Country" />
                </SelectTrigger>
                <SelectContent>
                  {countries.map((country: any) => (
                    <SelectItem key={country.value} value={country.value}>
                      {country.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {selectedCountry && (
                <Select onValueChange={(value) => setSelectedCity(value)}>
                  <SelectTrigger className="w-full">
                    <SelectValue placeholder="City" />
                  </SelectTrigger>
                  <SelectContent>
                    {cities.map((city: any) => (
                      <SelectItem key={city.value} value={city.value}>
                        {city.label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              )}
            </div>
            <Button>Sign Up</Button>
            <p className="text-orange-600">{error}</p>
          </form>
        </div>
      )}
    </div>
  );
};

export default BandRegistrationForm;
