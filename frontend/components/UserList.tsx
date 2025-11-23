import React from 'react';
import { User } from '@/types/chat';
import { UserCircle } from 'lucide-react';

interface UserListProps {
  users: User[];
  currentUserId?: string;
}

export const UserList: React.FC<UserListProps> = ({ users, currentUserId }) => {
  return (
    <div className="h-full bg-white border-l border-gray-200">
      <div className="p-4 border-b border-gray-200">
        <h2 className="text-xl font-bold text-gray-900">Online Users</h2>
      </div>
      <div className="overflow-y-auto custom-scrollbar h-[calc(100%-73px)]">
        {users.map((user) => (
          <div
            key={user.id}
            className="p-4 flex items-center space-x-3 border-b border-gray-100"
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
                {user.id === currentUserId && (
                  <span className="text-xs text-gray-500 ml-2">(You)</span>
                )}
              </div>
              <div className="text-xs text-gray-500">@{user.username}</div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};
