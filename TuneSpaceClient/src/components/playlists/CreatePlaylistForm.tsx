import { useForm } from "react-hook-form"
import FormInput from "../FormInput";
import { useState } from "react";
import axios from "../../axios/axios";

interface Inputs{
  name: string;
  description: string;
}

const CreatePlaylistForm = () => {
  const [error, setError] = useState('');

  const [isPublic, setIsPublic] = useState(false);
  const [isCollaborative, setIsCollaborative] = useState(false);

  const { 
    register,
    handleSubmit, 
    formState: { errors }, 
    watch 
  } = useForm<Inputs>();

  const onSubmit = async (data : Inputs) =>{

    const request = {
      name: data.name,
      description: data.description,
      public: isPublic
    }

    const response = await axios.post('/Spotify/create-playlist',
      JSON.stringify(request),
      {
        headers: {'Content-Type': 'application/json'}
      }
    );
  }

  return (
    <form 
      onSubmit={handleSubmit(onSubmit)} 
      className="p-4a flex flex-col items-center gap-4">
      <FormInput 
        type="text" 
        placeholder="Name" 
        register={register} 
        name="name" 
        error={errors.name?.message as string}
      />
      
      <FormInput 
        type="text" 
        placeholder="Description" 
        register={register} 
        name="description"  
        error={errors.description?.message as string}
      />

      <div className="flex gap-4">
        <span className="flex gap-2">
          <input type="checkbox" onChange={() => setIsPublic(!isPublic)} />
          <span>Public</span>
        </span>
        {/* <span className="flex gap-2">
          <input type="checkbox" onChange={() => setIsCollaborative(!isCollaborative)} />
          <span>Collaborative</span>
        </span> */}
      </div>

      <button type="submit">Confirm</button>
    </form>
  )
}

export default CreatePlaylistForm
