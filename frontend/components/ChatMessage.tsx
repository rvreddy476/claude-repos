import React from 'react';
import { Message } from '@/types/chat';

interface ChatMessageProps {
  message: Message;
  isOwnMessage: boolean;
}

export const ChatMessage: React.FC<ChatMessageProps> = ({ message, isOwnMessage }) => {
  const formattedTime = new Date(message.timestamp).toLocaleTimeString('en-US', {
    hour: '2-digit',
    minute: '2-digit',
  });

  return (
    <div className={`flex ${isOwnMessage ? 'justify-end' : 'justify-start'} mb-4`}>
      <div className={`max-w-[70%] ${isOwnMessage ? 'order-2' : 'order-1'}`}>
        {!isOwnMessage && (
          <div className="text-xs text-gray-600 mb-1 px-1">{message.senderName}</div>
        )}
        <div
          className={`rounded-2xl px-4 py-2 ${
            isOwnMessage
              ? 'bg-primary-500 text-white rounded-br-none'
              : 'bg-gray-200 text-gray-900 rounded-bl-none'
          }`}
        >
          <p className="text-sm break-words">{message.content}</p>
          <div
            className={`text-xs mt-1 ${
              isOwnMessage ? 'text-primary-100' : 'text-gray-500'
            }`}
          >
            {formattedTime}
            {message.isEdited && ' (edited)'}
          </div>
        </div>
      </div>
    </div>
  );
};
