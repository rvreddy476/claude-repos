export interface User {
  id: string;
  username: string;
  displayName: string;
  avatarUrl?: string;
  isOnline: boolean;
  lastSeenAt: Date;
}

export interface ChatRoom {
  id: string;
  name: string;
  description?: string;
  participantIds: string[];
  createdAt: Date;
  createdBy: string;
  isPrivate: boolean;
}

export interface Message {
  id: string;
  chatRoomId: string;
  senderId: string;
  senderName: string;
  content: string;
  timestamp: Date;
  type: MessageType;
  isEdited: boolean;
  editedAt?: Date;
}

export enum MessageType {
  Text = 0,
  Image = 1,
  File = 2,
  System = 3,
}

export interface CreateUserDto {
  username: string;
  displayName: string;
  avatarUrl?: string;
}

export interface CreateChatRoomDto {
  name: string;
  description?: string;
  isPrivate: boolean;
  participantIds?: string[];
}

export interface SendMessageDto {
  chatRoomId: string;
  content: string;
  type?: MessageType;
}
