export default interface ChatPreview {
  id: string;
  user1Id: string;
  user1Name: string;
  user2Id: string;
  user2Name: string;
  //recipientAvatar?: string;
  lastMessage: string;
  lastMessageTime: Date;
  unreadCount: number;
}
