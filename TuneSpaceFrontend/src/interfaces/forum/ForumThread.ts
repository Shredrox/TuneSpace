export interface ForumThread {
  id: string;
  title: string;
  authorId: string;
  authorName: string;
  authorAvatar?: string;
  createdAt: Date;
  lastActivityAt: Date;
  repliesCount: number;
  viewsCount: number;
  isPinned: boolean;
  isLocked: boolean;
}
