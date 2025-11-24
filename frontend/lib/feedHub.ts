import * as signalR from '@microsoft/signalr';
import { API_BASE_URL } from './config';
import { Post, Comment } from '@/types/feed';

const FEED_HUB_URL = `${API_BASE_URL}/feedHub`;

export class FeedHubConnection {
  private connection: signalR.HubConnection | null = null;

  async start(): Promise<void> {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(FEED_HUB_URL, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build();

    try {
      await this.connection.start();
      console.log('FeedHub SignalR Connected');
    } catch (err) {
      console.error('Error connecting to FeedHub:', err);
      throw err;
    }
  }

  async stop(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
    }
  }

  // Event listeners
  onNewPost(callback: (post: Post) => void): void {
    if (this.connection) {
      this.connection.on('NewPostCreated', callback);
    }
  }

  onNewComment(callback: (comment: Comment) => void): void {
    if (this.connection) {
      this.connection.on('NewCommentCreated', callback);
    }
  }

  onPostLikeUpdated(callback: (data: { postId: string; likesCount: number; isLiked: boolean; userId: string }) => void): void {
    if (this.connection) {
      this.connection.on('PostLikeUpdated', callback);
    }
  }

  onCommentLikeUpdated(callback: (data: { commentId: string; likesCount: number; isLiked: boolean; userId: string }) => void): void {
    if (this.connection) {
      this.connection.on('CommentLikeUpdated', callback);
    }
  }

  onPostDeleted(callback: (postId: string) => void): void {
    if (this.connection) {
      this.connection.on('PostDeleted', callback);
    }
  }

  onCommentDeleted(callback: (commentId: string) => void): void {
    if (this.connection) {
      this.connection.on('CommentDeleted', callback);
    }
  }
}
