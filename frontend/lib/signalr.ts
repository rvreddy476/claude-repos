import * as signalR from '@microsoft/signalr';
import { SIGNALR_HUB_URL } from './config';

export class ChatHubConnection {
  private connection: signalR.HubConnection | null = null;
  private userId: string = '';

  async start(userId: string): Promise<void> {
    this.userId = userId;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${SIGNALR_HUB_URL}?userId=${userId}`, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build();

    try {
      await this.connection.start();
      console.log('SignalR Connected');
    } catch (err) {
      console.error('Error connecting to SignalR:', err);
      throw err;
    }
  }

  async stop(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
    }
  }

  async joinRoom(roomId: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('JoinRoom', roomId);
    }
  }

  async leaveRoom(roomId: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('LeaveRoom', roomId);
    }
  }

  async sendMessage(chatRoomId: string, content: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('SendMessage', {
        chatRoomId,
        content,
        type: 0,
      });
    }
  }

  async sendTyping(roomId: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('UserTyping', roomId);
    }
  }

  async sendStoppedTyping(roomId: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('UserStoppedTyping', roomId);
    }
  }

  onReceiveMessage(callback: (message: any) => void): void {
    if (this.connection) {
      this.connection.on('ReceiveMessage', callback);
    }
  }

  onUserOnline(callback: (userId: string) => void): void {
    if (this.connection) {
      this.connection.on('UserOnline', callback);
    }
  }

  onUserOffline(callback: (userId: string) => void): void {
    if (this.connection) {
      this.connection.on('UserOffline', callback);
    }
  }

  onUserJoinedRoom(callback: (userId: string, roomId: string) => void): void {
    if (this.connection) {
      this.connection.on('UserJoinedRoom', callback);
    }
  }

  onUserLeftRoom(callback: (userId: string, roomId: string) => void): void {
    if (this.connection) {
      this.connection.on('UserLeftRoom', callback);
    }
  }

  onUserTyping(callback: (userName: string, roomId: string) => void): void {
    if (this.connection) {
      this.connection.on('UserTyping', callback);
    }
  }

  onUserStoppedTyping(callback: (userName: string, roomId: string) => void): void {
    if (this.connection) {
      this.connection.on('UserStoppedTyping', callback);
    }
  }

  getConnectionState(): signalR.HubConnectionState {
    return this.connection?.state ?? signalR.HubConnectionState.Disconnected;
  }
}

export const chatHub = new ChatHubConnection();
