import ThreadPost from "./ThreadPost";

export default interface ThreadData {
  id: string;
  title: string;
  categoryId: string;
  categoryName: string;
  posts: ThreadPost[];
}
