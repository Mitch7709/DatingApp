export type Message = {
  id: string;
  senderId: string;
  senderDisplayName: string;
  senderPhotoUrl: string;
  recipientId: string;
  recipientDisplayName: string;
  recipientPhotoUrl: string;
  content: string;
  dateRead?: string;
  messageSent: string;
  currentUserSender?: boolean;
}
