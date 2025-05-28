import AuthForm from "@/components/auth/auth-form";

export default function SignupPage() {
  return (
    <div className="w-full min-h-screen flex justify-center flex-col items-center gap-12">
      <h1
        className="bg-clip-text text-transparent bg-gradient-to-r from-blue-500 to-purple-600 inline-block"
        style={{ fontSize: "4rem", lineHeight: "1.1" }}
      >
        TuneSpace
      </h1>
      <div className="flex justify-center items-center gap-4">
        <div className="flex flex-col justify-center items-center w-full">
          <AuthForm className="w-[750px]" type={"register"} />
        </div>
      </div>
    </div>
  );
}
