"use client";

import { cn } from "@/lib/utils";
import { Button } from "@/components/shadcn/button";
import { Card, CardContent } from "@/components/shadcn/card";
import Link from "next/link";
import { useState, useEffect } from "react";
import useAuth from "@/hooks/useAuth";
import Register from "./register-form";
import Login from "./login-form";
import useToast from "@/hooks/useToast";

export function AuthForm({
  className,
  defaultTab,
  ...props
}: React.ComponentProps<"div"> & { defaultTab?: string | null }) {
  const [formStep, setFormStep] = useState(0);
  const [isLogin, setIsLogin] = useState(defaultTab === "login");

  useEffect(() => {
    setIsLogin(defaultTab === "login");
  }, [defaultTab]);

  const { setAuth } = useAuth();

  return (
    <div className={cn("flex flex-col gap-6", className)} {...props}>
      {formStep === 0 ? (
        <>
          {isLogin ? (
            <Card className="overflow-hidden">
              <CardContent className="grid p-0 md:grid-cols-2">
                <Login shouldRedirect={false} setIsLogin={setIsLogin} />
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
                  <Button
                    onClick={() => {
                      setFormStep(0);
                      setIsLogin(true);
                      useToast(
                        "Registered successfully. You can now log in to your account."
                      );
                    }}
                    variant="outline"
                  >
                    No
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
