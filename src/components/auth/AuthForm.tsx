"use client";

import { cn } from "@/lib/utils";
import { Button } from "@/components/shadcn/Button";
import { Card, CardContent } from "@/components/shadcn/Card";
import Link from "next/link";
import { useState } from "react";
import useAuth from "@/hooks/useAuth";
import Login from "./Login";
import Register from "./Register";

export function AuthForm({ className, ...props }: React.ComponentProps<"div">) {
  const [formStep, setFormStep] = useState(0);
  const [isLogin, setIsLogin] = useState(true);

  const { setAuth } = useAuth();

  return (
    <div className={cn("flex flex-col gap-6", className)} {...props}>
      {formStep === 0 ? (
        <>
          {isLogin ? (
            <Card className="overflow-hidden">
              <CardContent className="grid p-0 md:grid-cols-2">
                <Login shouldRedirect={true} setIsLogin={setIsLogin} />
                <div className="relative hidden bg-muted md:block">
                  <img
                    src="https://picsum.photos/700"
                    alt="Image"
                    className="absolute inset-0 h-full w-full object-cover dark:brightness-[0.3] dark:grayscale"
                  />
                </div>
              </CardContent>
            </Card>
          ) : (
            <Card className="overflow-hidden">
              <CardContent className="grid p-0 md:grid-cols-2">
                <div className="relative hidden bg-muted md:block">
                  <img
                    src="https://picsum.photos/700"
                    alt="Image"
                    className="absolute inset-0 h-full w-full object-cover dark:brightness-[0.3] dark:grayscale"
                  />
                </div>
                <Register setFormStep={setFormStep} setIsLogin={setIsLogin} />
              </CardContent>
            </Card>
          )}
        </>
      ) : (
        <div className="flex justify-center items-center gap-4">
          <Card className="flex justify-center items-center gap-4">
            <CardContent className="flex justify-center items-center gap-4 p-8">
              <div className="flex flex-col gap-4 justify-center items-center">
                <span className="text-lg">Are you in a band?</span>{" "}
                <div className="flex gap-4">
                  {" "}
                  <Button>
                    <Link href="band/register">Yes</Link>
                  </Button>
                  <Button onClick={() => setAuth({ accessToken: "123" })}>
                    <Link href="/band/dashboard/019575c8-a8f5-7c71-91a8-6640c87b2b7e">
                      No
                    </Link>
                  </Button>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      )}
      <div className="text-balance text-center text-xs text-muted-foreground [&_a]:underline [&_a]:underline-offset-4 hover:[&_a]:text-primary">
        By clicking continue, you agree to our <a href="#">Terms of Service</a>{" "}
        and <a href="#">Privacy Policy</a>.
      </div>
    </div>
  );
}
