import { decodeJwt, jwtVerify } from "jose";
import { NextRequest } from "next/server";

export const isAuthenticated = async (
  request: NextRequest
): Promise<boolean> => {
  const token = request.cookies.get("AccessToken")?.value;
  if (!token) {
    return false;
  }

  try {
    const { payload } = await jwtVerify(
      token,
      new TextEncoder().encode(process.env.JWT_SECRET!)
    );

    return Boolean(payload?.sub);
  } catch (err) {
    console.error("JWT validation failed:", err);
    return false;
  }
};

export const isTokenExpired = (token?: string) => {
  if (!token) {
    return true;
  }

  try {
    const decoded = decodeJwt(token);
    const exp = decoded.exp;
    if (!exp) {
      return true;
    }

    return Date.now() >= exp * 1000 - 60000;
  } catch {
    return true;
  }
};
