import { FieldValues, Path, UseFormRegister } from "react-hook-form";

interface InputProps<T extends FieldValues> {
  type: string; 
  placeholder: string; 
  register: UseFormRegister<T>; 
  name: Path<T>; 
  error: string | undefined;
}

const FormInput = <T extends FieldValues> ({ 
  type, 
  placeholder, 
  register, 
  name, 
  error 
} : InputProps<T>) => {
  return (
    <>
      <input 
        className="bg-[#0d0a15] rounded-2xl p-3" 
        type={type} 
        placeholder={placeholder} 
        {...register(name)}
      />
      <p className="text-orange-600">{error}</p>
    </>
  )
}

export default FormInput
