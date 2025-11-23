'use client';

import React, { useState, useEffect } from 'react';
import { LoginForm } from '@/components/LoginForm';
import { ChatRoomList } from '@/components/ChatRoomList';
import { ChatWindow } from '@/components/ChatWindow';
import { UserList } from '@/components/UserList';
import { User, ChatRoom, Message } from '@/types/chat';
import { api } from '@/lib/api';
import { chatHub } from '@/lib/signalr';

export default function Home() {
  const [currentUser, setCurrentUser] = useState<User | null>(null);
  const [users, setUsers] = useState<User[]>([]);
  const [chatRooms, setChatRooms] = useState<ChatRoom[]>([]);
  const [selectedRoomId, setSelectedRoomId] = useState<string | undefined>();
  const [messages, setMessages] = useState<Message[]>([]);
  const [typingUsers, setTypingUsers] = useState<string[]>([]);

  useEffect(() => {
    if (currentUser) {
      loadInitialData();
      setupSignalR();
    }
  }, [currentUser]);

  const loadInitialData = async () => {
    try {
      const [roomsData, usersData] = await Promise.all([
        api.getChatRooms(),
        api.getOnlineUsers(),
      ]);

      setChatRooms(roomsData);
      setUsers(usersData);

      if (roomsData.length > 0 && !selectedRoomId) {
        handleRoomSelect(roomsData[0].id);
      }
    } catch (error) {
      console.error('Error loading data:', error);
    }
  };

  const setupSignalR = async () => {
    if (!currentUser) return;

    try {
      await chatHub.start(currentUser.id);

      chatHub.onReceiveMessage((message: Message) => {
        if (message.chatRoomId === selectedRoomId) {
          setMessages((prev) => [...prev, message]);
        }
      });

      chatHub.onUserOnline(async (userId: string) => {
        const user = await api.getUserById(userId);
        setUsers((prev) => {
          const filtered = prev.filter((u) => u.id !== userId);
          return [...filtered, user];
        });
      });

      chatHub.onUserOffline((userId: string) => {
        setUsers((prev) =>
          prev.map((u) => (u.id === userId ? { ...u, isOnline: false } : u))
        );
      });

      chatHub.onUserTyping((userName: string, roomId: string) => {
        if (roomId === selectedRoomId) {
          setTypingUsers((prev) => {
            if (!prev.includes(userName)) {
              return [...prev, userName];
            }
            return prev;
          });
        }
      });

      chatHub.onUserStoppedTyping((userName: string, roomId: string) => {
        if (roomId === selectedRoomId) {
          setTypingUsers((prev) => prev.filter((name) => name !== userName));
        }
      });
    } catch (error) {
      console.error('Error setting up SignalR:', error);
    }
  };

  const handleLogin = async (username: string, displayName: string) => {
    try {
      // Check if user already exists
      let user = await api.getUserByUsername(username);

      if (!user) {
        // User doesn't exist, create new user
        console.log('Creating new user:', { username, displayName });
        user = await api.createUser({ username, displayName });
        console.log('User created successfully:', user);
      } else {
        console.log('User already exists:', user);
      }

      setCurrentUser(user);
    } catch (error) {
      console.error('Error logging in:', error);
      alert('Failed to login. Please try again.');
    }
  };

  const handleRoomSelect = async (roomId: string) => {
    setSelectedRoomId(roomId);
    setMessages([]);
    setTypingUsers([]);

    try {
      const roomMessages = await api.getRoomMessages(roomId);
      setMessages(roomMessages.reverse());

      await chatHub.joinRoom(roomId);
    } catch (error) {
      console.error('Error loading room messages:', error);
    }
  };

  const handleSendMessage = async (content: string) => {
    if (!selectedRoomId) return;

    try {
      await chatHub.sendMessage(selectedRoomId, content);
    } catch (error) {
      console.error('Error sending message:', error);
    }
  };

  const handleTyping = () => {
    if (selectedRoomId) {
      chatHub.sendTyping(selectedRoomId);
    }
  };

  const handleStoppedTyping = () => {
    if (selectedRoomId) {
      chatHub.sendStoppedTyping(selectedRoomId);
    }
  };

  if (!currentUser) {
    return <LoginForm onLogin={handleLogin} />;
  }

  const selectedRoom = chatRooms.find((r) => r.id === selectedRoomId);

  return (
    <div className="h-screen flex">
      {/* Left Sidebar - Chat Rooms */}
      <div className="w-80">
        <ChatRoomList
          rooms={chatRooms}
          selectedRoomId={selectedRoomId}
          onRoomSelect={handleRoomSelect}
        />
      </div>

      {/* Main Chat Area */}
      <div className="flex-1">
        {selectedRoom ? (
          <ChatWindow
            roomName={selectedRoom.name}
            messages={messages}
            currentUserId={currentUser.id}
            onSendMessage={handleSendMessage}
            onTyping={handleTyping}
            onStoppedTyping={handleStoppedTyping}
            typingUsers={typingUsers}
          />
        ) : (
          <div className="flex items-center justify-center h-full bg-gray-50">
            <p className="text-gray-500">Select a chat room to start messaging</p>
          </div>
        )}
      </div>

      {/* Right Sidebar - Users */}
      <div className="w-80">
        <UserList users={users} currentUserId={currentUser.id} />
      </div>
    </div>
  );
}
