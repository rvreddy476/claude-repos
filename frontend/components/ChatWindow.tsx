import React, { useEffect, useRef } from 'react';
import { Message } from '@/types/chat';
import { ChatMessage } from './ChatMessage';
import { MessageInput } from './MessageInput';

interface ChatWindowProps {
  roomName: string;
  messages: Message[];
  currentUserId: string;
  onSendMessage: (content: string) => void;
  onTyping: () => void;
  onStoppedTyping: () => void;
  typingUsers: string[];
}

export const ChatWindow: React.FC<ChatWindowProps> = ({
  roomName,
  messages,
  currentUserId,
  onSendMessage,
  onTyping,
  onStoppedTyping,
  typingUsers,
}) => {
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  return (
    <div className="flex flex-col h-full bg-white">
      {/* Header */}
      <div className="p-4 border-b border-gray-200 bg-white shadow-sm">
        <h2 className="text-xl font-bold text-gray-900">{roomName}</h2>
      </div>

      {/* Messages */}
      <div className="flex-1 overflow-y-auto p-4 custom-scrollbar bg-gray-50">
        {messages.length === 0 ? (
          <div className="flex items-center justify-center h-full">
            <p className="text-gray-500">No messages yet. Start the conversation!</p>
          </div>
        ) : (
          <>
            {messages.map((message) => (
              <ChatMessage
                key={message.id}
                message={message}
                isOwnMessage={message.senderId === currentUserId}
              />
            ))}
            {typingUsers.length > 0 && (
              <div className="text-sm text-gray-500 italic mb-2">
                {typingUsers.join(', ')} {typingUsers.length === 1 ? 'is' : 'are'} typing...
              </div>
            )}
            <div ref={messagesEndRef} />
          </>
        )}
      </div>

      {/* Input */}
      <MessageInput
        onSendMessage={onSendMessage}
        onTyping={onTyping}
        onStoppedTyping={onStoppedTyping}
      />
    </div>
  );
};
