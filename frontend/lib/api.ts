import { User, ChatRoom, Message, CreateUserDto, CreateChatRoomDto } from '@/types/chat';

const API_BASE_URL = 'https://localhost:8080/api';

export const api = {
  // Users
  async getUsers(): Promise<User[]> {
    const response = await fetch(`${API_BASE_URL}/users`);
    return response.json();
  },

  async getOnlineUsers(): Promise<User[]> {
    const response = await fetch(`${API_BASE_URL}/users/online`);
    if (!response.ok) {
      console.error('Failed to fetch online users');
      return [];
    }
    return response.json();
  },

  async getUserById(id: string): Promise<User> {
    const response = await fetch(`${API_BASE_URL}/users/${id}`);
    return response.json();
  },

  async getUserByUsername(username: string): Promise<User | null> {
    const response = await fetch(`${API_BASE_URL}/users/username/${username}`);
    if (!response.ok) {
      return null;
    }
    return response.json();
  },

  async createUser(user: CreateUserDto): Promise<User> {
    const response = await fetch(`${API_BASE_URL}/users`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(user),
    });
    if (!response.ok) {
      const error = await response.text();
      throw new Error(`Failed to create user: ${error}`);
    }
    return response.json();
  },

  // Chat Rooms
  async getChatRooms(): Promise<ChatRoom[]> {
    const response = await fetch(`${API_BASE_URL}/chatrooms`);
    if (!response.ok) {
      console.error('Failed to fetch chat rooms');
      return [];
    }
    return response.json();
  },

  async getChatRoomById(id: string): Promise<ChatRoom> {
    const response = await fetch(`${API_BASE_URL}/chatrooms/${id}`);
    return response.json();
  },

  async getUserChatRooms(userId: string): Promise<ChatRoom[]> {
    const response = await fetch(`${API_BASE_URL}/chatrooms/user/${userId}`);
    return response.json();
  },

  async createChatRoom(room: CreateChatRoomDto, createdBy: string): Promise<ChatRoom> {
    const response = await fetch(`${API_BASE_URL}/chatrooms?createdBy=${createdBy}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(room),
    });
    return response.json();
  },

  // Messages
  async getRoomMessages(chatRoomId: string, limit = 50, skip = 0): Promise<Message[]> {
    const response = await fetch(
      `${API_BASE_URL}/messages/room/${chatRoomId}?limit=${limit}&skip=${skip}`
    );
    return response.json();
  },

  async getMessageById(id: string): Promise<Message> {
    const response = await fetch(`${API_BASE_URL}/messages/${id}`);
    return response.json();
  },
};
