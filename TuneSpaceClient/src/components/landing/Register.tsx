import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import axios from "../../axios/axios";
import { useEffect, useState } from "react";
import FormInput from "../FormInput";

const registerSchema = z.object({
  name: z.string().trim().min(2, "Username must be at least 2 characters"),
  email: z.string().email().trim(),
  password: z.string().min(4, "Password must be at least 4 characters"),
  confirmPassword: z.string(),
}).refine((data) => data.password === data.confirmPassword,{
  message: "Passwords do not match",
  path: ["confirmPassword"],
})

type Inputs = z.infer<typeof registerSchema>

const Register = () => {
  const [error, setError] = useState('');

  const { 
    register, 
    handleSubmit, 
    formState: { errors }, 
    watch 
  } = useForm<Inputs>({
    resolver: zodResolver(registerSchema)
  });

  const watchName = watch("name");
  const watchEmail = watch("email");
  const watchPassword = watch("password");
  const watchConfirmPassword = watch("confirmPassword");

  useEffect(() =>{
    setError('');
  },[watchName, watchEmail, watchPassword, watchConfirmPassword])

  const onSubmit = async (data: Inputs) => {
    const { confirmPassword, ...request} = data;

    try {
      const response = await axios.post('Auth/register',
        JSON.stringify(request),
        {
          headers: {'Content-Type': 'application/json'}
        }
      );
      console.log(response.data);
    } catch (error : any) {
      if(!error?.response){
        setError('No response');
      }else if(error.response?.status === 409){
        setError('Username is already taken');
      }else if(error.response?.status === 400){
        setError('An error occurred');
      }else{
        setError('Error');
      }
    }
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      noValidate
      className="flex flex-col justify-center items-center bg-gray-15 
      gap-2 p-4 white-98 w-1/2 rounded-2xl"
      >
      <h2 className="form-h2 white-98">Register</h2>

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

      <button className="bg-[#0d0a15] rounded-2xl p-2">Sign Up</button>
      <p className="text-orange-600">{error}</p>
    </form>
  )
}

export default Register
