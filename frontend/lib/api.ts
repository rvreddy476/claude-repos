import { User, ChatRoom, Message, CreateUserDto, CreateChatRoomDto } from '@/types/chat';
import { Post, Comment, CreatePostDto, CreateCommentDto } from '@/types/feed';
import { API_BASE_URL } from './config';

const API_URL = `${API_BASE_URL}/api`;

export const api = {
  // Users
  async getUsers(): Promise<User[]> {
    const response = await fetch(`${API_URL}/users`);
    return response.json();
  },

  async getOnlineUsers(): Promise<User[]> {
    const response = await fetch(`${API_URL}/users/online`);
    if (!response.ok) {
      console.error('Failed to fetch online users');
      return [];
    }
    return response.json();
  },

  async getUserById(id: string): Promise<User> {
    const response = await fetch(`${API_URL}/users/${id}`);
    return response.json();
  },

  async getUserByUsername(username: string): Promise<User | null> {
    const response = await fetch(`${API_URL}/users/username/${username}`);
    if (!response.ok) {
      return null;
    }
    return response.json();
  },

  async createUser(user: CreateUserDto): Promise<User> {
    const response = await fetch(`${API_URL}/users`, {
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
    const response = await fetch(`${API_URL}/chatrooms`);
    if (!response.ok) {
      console.error('Failed to fetch chat rooms');
      return [];
    }
    return response.json();
  },

  async getChatRoomById(id: string): Promise<ChatRoom> {
    const response = await fetch(`${API_URL}/chatrooms/${id}`);
    return response.json();
  },

  async getUserChatRooms(userId: string): Promise<ChatRoom[]> {
    const response = await fetch(`${API_URL}/chatrooms/user/${userId}`);
    return response.json();
  },

  async createChatRoom(room: CreateChatRoomDto, createdBy: string): Promise<ChatRoom> {
    const response = await fetch(`${API_URL}/chatrooms?createdBy=${createdBy}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(room),
    });
    return response.json();
  },

  // Messages
  async getRoomMessages(chatRoomId: string, limit = 50, skip = 0): Promise<Message[]> {
    const response = await fetch(
      `${API_URL}/messages/room/${chatRoomId}?limit=${limit}&skip=${skip}`
    );
    return response.json();
  },

  async getMessageById(id: string): Promise<Message> {
    const response = await fetch(`${API_URL}/messages/${id}`);
    return response.json();
  },

  // Posts
  async getPosts(userId?: string, skip = 0, limit = 20): Promise<Post[]> {
    const params = new URLSearchParams();
    if (userId) params.append('userId', userId);
    params.append('skip', skip.toString());
    params.append('limit', limit.toString());

    const response = await fetch(`${API_URL}/posts?${params}`);
    if (!response.ok) {
      console.error('Failed to fetch posts');
      return [];
    }
    return response.json();
  },

  async getPostById(id: string, userId?: string): Promise<Post | null> {
    const params = userId ? `?userId=${userId}` : '';
    const response = await fetch(`${API_URL}/posts/${id}${params}`);
    if (!response.ok) {
      return null;
    }
    return response.json();
  },

  async getUserPosts(userId: string, currentUserId?: string, skip = 0, limit = 20): Promise<Post[]> {
    const params = new URLSearchParams();
    if (currentUserId) params.append('currentUserId', currentUserId);
    params.append('skip', skip.toString());
    params.append('limit', limit.toString());

    const response = await fetch(`${API_URL}/posts/user/${userId}?${params}`);
    if (!response.ok) {
      console.error('Failed to fetch user posts');
      return [];
    }
    return response.json();
  },

  async createPost(userId: string, post: CreatePostDto): Promise<Post> {
    const response = await fetch(`${API_URL}/posts?userId=${userId}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(post),
    });
    if (!response.ok) {
      const error = await response.text();
      throw new Error(`Failed to create post: ${error}`);
    }
    return response.json();
  },

  async togglePostLike(postId: string, userId: string): Promise<{ isLiked: boolean; likesCount: number }> {
    const response = await fetch(`${API_URL}/posts/${postId}/like?userId=${userId}`, {
      method: 'POST',
    });
    if (!response.ok) {
      throw new Error('Failed to toggle like');
    }
    return response.json();
  },

  async deletePost(postId: string): Promise<void> {
    const response = await fetch(`${API_URL}/posts/${postId}`, {
      method: 'DELETE',
    });
    if (!response.ok) {
      throw new Error('Failed to delete post');
    }
  },

  // Comments
  async getPostComments(postId: string, userId?: string, skip = 0, limit = 3): Promise<Comment[]> {
    const params = new URLSearchParams();
    if (userId) params.append('userId', userId);
    params.append('skip', skip.toString());
    params.append('limit', limit.toString());

    const response = await fetch(`${API_URL}/comments/post/${postId}?${params}`);
    if (!response.ok) {
      console.error('Failed to fetch comments');
      return [];
    }
    return response.json();
  },

  async getPostCommentsCount(postId: string): Promise<number> {
    const response = await fetch(`${API_URL}/comments/post/${postId}/count`);
    if (!response.ok) {
      return 0;
    }
    return response.json();
  },

  async createComment(userId: string, comment: CreateCommentDto): Promise<Comment> {
    const response = await fetch(`${API_URL}/comments?userId=${userId}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(comment),
    });
    if (!response.ok) {
      const error = await response.text();
      throw new Error(`Failed to create comment: ${error}`);
    }
    return response.json();
  },

  async toggleCommentLike(commentId: string, userId: string): Promise<{ isLiked: boolean; likesCount: number }> {
    const response = await fetch(`${API_URL}/comments/${commentId}/like?userId=${userId}`, {
      method: 'POST',
    });
    if (!response.ok) {
      throw new Error('Failed to toggle comment like');
    }
    return response.json();
  },

  async deleteComment(commentId: string): Promise<void> {
    const response = await fetch(`${API_URL}/comments/${commentId}`, {
      method: 'DELETE',
    });
    if (!response.ok) {
      throw new Error('Failed to delete comment');
    }
  },

  // Media Upload
  async uploadMedia(file: File, userId: string): Promise<{ url: string; fileName: string; contentType: string; size: number }> {
    const formData = new FormData();
    formData.append('file', file);

    const response = await fetch(`${API_URL}/media/upload?userId=${userId}`, {
      method: 'POST',
      body: formData,
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to upload media');
    }

    return response.json();
  },

  async uploadMultipleMedia(files: File[], userId: string): Promise<Array<{ url: string; fileName: string; contentType: string; size: number }>> {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append('files', file);
    });

    const response = await fetch(`${API_URL}/media/upload-multiple?userId=${userId}`, {
      method: 'POST',
      body: formData,
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to upload media');
    }

    return response.json();
  },

  async deleteMedia(fileUrl: string): Promise<void> {
    const response = await fetch(`${API_URL}/media?fileUrl=${encodeURIComponent(fileUrl)}`, {
      method: 'DELETE',
    });

    if (!response.ok) {
      throw new Error('Failed to delete media');
    }
  },
};
