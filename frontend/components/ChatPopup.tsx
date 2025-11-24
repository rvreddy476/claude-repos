import React, { useState, useRef, useEffect } from 'react';
import { User } from '@/types/chat';
import { X, Minus, Send, UserCircle } from 'lucide-react';

interface DirectChatMessage {
  id: string;
  senderId: string;
  senderName: string;
  content: string;
  timestamp: Date;
}

interface ChatPopupProps {
  user: User;
  currentUserId: string;
  onClose: () => void;
  onMinimize: () => void;
  isMinimized: boolean;
}

export const ChatPopup: React.FC<ChatPopupProps> = ({
  user,
  currentUserId,
  onClose,
  onMinimize,
  isMinimized,
}) => {
  const [message, setMessage] = useState('');
  const [messages, setMessages] = useState<DirectChatMessage[]>([]);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const handleSend = () => {
    if (message.trim()) {
      const newMessage: DirectChatMessage = {
        id: Date.now().toString(),
        senderId: currentUserId,
        senderName: 'You',
        content: message.trim(),
        timestamp: new Date(),
      };
      setMessages((prev) => [...prev, newMessage]);
      setMessage('');
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  return (
    <div className="fixed bottom-0 right-20 w-80 bg-white shadow-2xl rounded-t-lg border border-gray-300 flex flex-col z-50">
      {/* Header */}
      <div className="bg-primary-500 text-white p-3 rounded-t-lg flex items-center justify-between">
        <div className="flex items-center space-x-2">
          {user.avatarUrl ? (
            <img
              src={user.avatarUrl}
              alt={user.displayName}
              className="w-8 h-8 rounded-full"
            />
          ) : (
            <UserCircle size={32} className="text-white" />
          )}
          <div>
            <div className="font-semibold">{user.displayName}</div>
            <div className="text-xs text-primary-100">
              {user.isOnline ? 'Online' : 'Offline'}
            </div>
          </div>
        </div>
        <div className="flex space-x-2">
          <button
            onClick={onMinimize}
            className="hover:bg-primary-600 p-1 rounded"
          >
            <Minus size={18} />
          </button>
          <button
            onClick={onClose}
            className="hover:bg-primary-600 p-1 rounded"
          >
            <X size={18} />
          </button>
        </div>
      </div>

      {!isMinimized && (
        <>
          {/* Messages */}
          <div className="flex-1 overflow-y-auto p-3 space-y-2 bg-gray-50" style={{ height: '300px' }}>
            {messages.length === 0 ? (
              <div className="text-center text-gray-500 text-sm mt-10">
                Start a conversation with {user.displayName}
              </div>
            ) : (
              messages.map((msg) => (
                <div
                  key={msg.id}
                  className={`flex ${
                    msg.senderId === currentUserId ? 'justify-end' : 'justify-start'
                  }`}
                >
                  <div
                    className={`max-w-[70%] px-3 py-2 rounded-lg ${
                      msg.senderId === currentUserId
                        ? 'bg-primary-500 text-white'
                        : 'bg-gray-200 text-gray-900'
                    }`}
                  >
                    <p className="text-sm break-words">{msg.content}</p>
                    <p
                      className={`text-xs mt-1 ${
                        msg.senderId === currentUserId
                          ? 'text-primary-100'
                          : 'text-gray-500'
                      }`}
                    >
                      {new Date(msg.timestamp).toLocaleTimeString('en-US', {
                        hour: '2-digit',
                        minute: '2-digit',
                      })}
                    </p>
                  </div>
                </div>
              ))
            )}
            <div ref={messagesEndRef} />
          </div>

          {/* Input */}
          <div className="p-2 border-t border-gray-200 bg-white">
            <div className="flex space-x-2">
              <input
                type="text"
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                onKeyPress={handleKeyPress}
                placeholder="Type a message..."
                className="flex-1 px-3 py-2 border border-gray-300 rounded-full focus:outline-none focus:ring-2 focus:ring-primary-500 text-sm"
              />
              <button
                onClick={handleSend}
                disabled={!message.trim()}
                className="bg-primary-500 text-white p-2 rounded-full hover:bg-primary-600 disabled:bg-gray-300 disabled:cursor-not-allowed"
              >
                <Send size={16} />
              </button>
            </div>
          </div>
        </>
      )}
    </div>
  );
};
