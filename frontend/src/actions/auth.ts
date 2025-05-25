"use server";

import { registerSchema } from "@/schemas/register.schema";
import httpClient from "@/services/http-client";
import { ENDPOINTS } from "@/utils/constants";

export const registerUser = async (data: FormData) => {
  const formData = Object.fromEntries(data);
  const parsed = registerSchema.safeParse(formData);

  if (!parsed.success) {
    throw new Error("Invalid data");
  }

  await httpClient.post(`${ENDPOINTS.REGISTER}`, JSON.stringify(parsed.data), {
    headers: { "Content-Type": "application/json" },
  });
};
