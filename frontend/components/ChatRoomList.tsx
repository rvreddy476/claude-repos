import React from 'react';
import { ChatRoom } from '@/types/chat';
import { MessageSquare, Lock, Hash } from 'lucide-react';

interface ChatRoomListProps {
  rooms: ChatRoom[];
  selectedRoomId?: string;
  onRoomSelect: (roomId: string) => void;
}

export const ChatRoomList: React.FC<ChatRoomListProps> = ({
  rooms,
  selectedRoomId,
  onRoomSelect,
}) => {
  return (
    <div className="h-full bg-white border-r border-gray-200">
      <div className="p-4 border-b border-gray-200">
        <h2 className="text-xl font-bold text-gray-900">Chat Rooms</h2>
      </div>
      <div className="overflow-y-auto custom-scrollbar h-[calc(100%-73px)]">
        {rooms.map((room) => (
          <button
            key={room.id}
            onClick={() => onRoomSelect(room.id)}
            className={`w-full p-4 flex items-start space-x-3 hover:bg-gray-50 transition-colors border-b border-gray-100 ${
              selectedRoomId === room.id ? 'bg-primary-50' : ''
            }`}
          >
            <div className="mt-1">
              {room.isPrivate ? (
                <Lock size={20} className="text-gray-500" />
              ) : (
                <Hash size={20} className="text-gray-500" />
              )}
            </div>
            <div className="flex-1 text-left">
              <div className="font-semibold text-gray-900">{room.name}</div>
              {room.description && (
                <div className="text-sm text-gray-500 truncate">
                  {room.description}
                </div>
              )}
              <div className="text-xs text-gray-400 mt-1">
                {room.participantIds.length} participants
              </div>
            </div>
          </button>
        ))}
      </div>
    </div>
  );
};
