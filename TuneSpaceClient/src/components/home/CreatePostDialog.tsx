import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/Dialog"
import { Button } from "@/components/Button";
import { Input } from "@/components/Input";
import { Textarea } from "../Textarea";

const CreatePostDialog = () => {
  return (
    <Dialog>
      <DialogTrigger asChild>
        <Button variant="outline">Create Post</Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Create Post</DialogTitle>
          <DialogDescription>
            Create a post with a song
          </DialogDescription>
        </DialogHeader>
        <div className="grid gap-4 py-4">
        <div className="grid grid-cols-4 items-center gap-4">
            <label htmlFor="name" className="text-right">
              Song
            </label>
            <Input
              id="name"
              className="col-span-3"
              placeholder="Link"
            />
          </div>
          <div className="grid grid-cols-4 items-center gap-4">
            <label htmlFor="name" className="text-right">
              Title
            </label>
            <Input
              id="name"
              className="col-span-3"
              placeholder="Title"
            />
          </div>
          <div className="grid grid-cols-4 items-center gap-4">
            <label htmlFor="username" className="text-right">
              Content
            </label>
            <Textarea className="col-span-3" placeholder="Content..."></Textarea>
          </div>
          <div className="grid grid-cols-4 items-center gap-4">
            <label htmlFor="username" className="text-right">
              Tags
            </label>
            <Button className="w-8 h-8 rounded-full">+</Button>
          </div>
        </div>
        <DialogFooter>
          <Button type="submit">Confirm</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

export default CreatePostDialog
