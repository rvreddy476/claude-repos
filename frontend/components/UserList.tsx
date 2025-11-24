import React from 'react';
import { User } from '@/types/chat';
import { UserCircle } from 'lucide-react';

interface UserListProps {
  users: User[];
  currentUserId?: string;
  onUserClick?: (user: User) => void;
}

export const UserList: React.FC<UserListProps> = ({ users, currentUserId, onUserClick }) => {
  // Filter out current user from the list
  const otherUsers = users.filter((user) => user.id !== currentUserId);

  return (
    <div className="h-full bg-white border-l border-gray-200">
      <div className="p-4 border-b border-gray-200">
        <h2 className="text-xl font-bold text-gray-900">Online Users</h2>
        <p className="text-xs text-gray-500 mt-1">{otherUsers.length} online</p>
      </div>
      <div className="overflow-y-auto custom-scrollbar h-[calc(100%-73px)]">
        {otherUsers.length === 0 ? (
          <div className="p-4 text-center text-gray-500 text-sm">
            No other users online
          </div>
        ) : (
          otherUsers.map((user) => (
            <button
              key={user.id}
              onClick={() => onUserClick?.(user)}
              className="w-full p-4 flex items-center space-x-3 border-b border-gray-100 hover:bg-gray-50 transition-colors cursor-pointer text-left"
            >
            <div className="relative">
              {user.avatarUrl ? (
                <img
                  src={user.avatarUrl}
                  alt={user.displayName}
                  className="w-10 h-10 rounded-full"
                />
              ) : (
                <UserCircle size={40} className="text-gray-400" />
              )}
              {user.isOnline && (
                <div className="absolute bottom-0 right-0 w-3 h-3 bg-green-500 rounded-full border-2 border-white"></div>
              )}
            </div>
              <div className="flex-1">
                <div className="font-semibold text-gray-900">
                  {user.displayName}
                </div>
                <div className="text-xs text-gray-500">@{user.username}</div>
                <div className="text-xs text-primary-500 mt-1">Click to chat</div>
              </div>
            </button>
          ))
        )}
      </div>
    </div>
  );
};
