'use client';

import React, { useState } from 'react';
import { User } from '@/types/chat';
import { CreatePostDto, PostType } from '@/types/feed';

interface PostCreatorProps {
  currentUser: User;
  onCreatePost: (post: CreatePostDto) => Promise<void>;
}

export const PostCreator: React.FC<PostCreatorProps> = ({ currentUser, onCreatePost }) => {
  const [content, setContent] = useState('');
  const [imageUrl, setImageUrl] = useState('');
  const [videoUrl, setVideoUrl] = useState('');
  const [showMediaInput, setShowMediaInput] = useState(false);
  const [mediaType, setMediaType] = useState<'image' | 'video' | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async () => {
    if (!content.trim() && !imageUrl && !videoUrl) {
      return;
    }

    setIsLoading(true);
    try {
      let type = PostType.Text;
      if (imageUrl && videoUrl) {
        type = PostType.Mixed;
      } else if (imageUrl) {
        type = PostType.Image;
      } else if (videoUrl) {
        type = PostType.Video;
      }

      await onCreatePost({
        content: content.trim() || undefined,
        imageUrl: imageUrl || undefined,
        videoUrl: videoUrl || undefined,
        type,
      });

      // Reset form
      setContent('');
      setImageUrl('');
      setVideoUrl('');
      setShowMediaInput(false);
      setMediaType(null);
    } catch (error) {
      console.error('Failed to create post:', error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="bg-white rounded-lg shadow p-4 mb-4">
      {/* User Profile */}
      <div className="flex items-start space-x-3">
        <div className="w-10 h-10 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center text-white font-bold flex-shrink-0">
          {currentUser.displayName.charAt(0).toUpperCase()}
        </div>
        <div className="flex-1">
          {/* Text Input */}
          <textarea
            value={content}
            onChange={(e) => setContent(e.target.value)}
            placeholder={`What's on your mind, ${currentUser.displayName}?`}
            className="w-full px-4 py-2 border border-gray-200 rounded-lg resize-none focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
            rows={3}
          />

          {/* Media Input */}
          {showMediaInput && (
            <div className="mt-3 space-y-2">
              {mediaType === 'image' && (
                <input
                  type="url"
                  value={imageUrl}
                  onChange={(e) => setImageUrl(e.target.value)}
                  placeholder="Enter image URL"
                  className="w-full px-4 py-2 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
                />
              )}
              {mediaType === 'video' && (
                <input
                  type="url"
                  value={videoUrl}
                  onChange={(e) => setVideoUrl(e.target.value)}
                  placeholder="Enter video URL"
                  className="w-full px-4 py-2 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
                />
              )}
            </div>
          )}

          {/* Action Buttons */}
          <div className="flex items-center justify-between mt-3 pt-3 border-t border-gray-200">
            <div className="flex items-center space-x-2">
              {/* Image Button */}
              <button
                onClick={() => {
                  setShowMediaInput(true);
                  setMediaType('image');
                }}
                className="flex items-center space-x-1 px-3 py-2 rounded-lg hover:bg-gray-100 transition-colors text-gray-700"
                title="Add Image"
              >
                <svg className="w-5 h-5 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
                <span className="text-sm">Photo</span>
              </button>

              {/* Video Button */}
              <button
                onClick={() => {
                  setShowMediaInput(true);
                  setMediaType('video');
                }}
                className="flex items-center space-x-1 px-3 py-2 rounded-lg hover:bg-gray-100 transition-colors text-gray-700"
                title="Add Video"
              >
                <svg className="w-5 h-5 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z" />
                </svg>
                <span className="text-sm">Video</span>
              </button>

              {/* Emoji Button */}
              <button
                className="flex items-center space-x-1 px-3 py-2 rounded-lg hover:bg-gray-100 transition-colors text-gray-700"
                title="Add Emoji"
                onClick={() => setContent(content + '😊')}
              >
                <svg className="w-5 h-5 text-yellow-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M14.828 14.828a4 4 0 01-5.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <span className="text-sm">Emoji</span>
              </button>
            </div>

            {/* Post Button */}
            <button
              onClick={handleSubmit}
              disabled={isLoading || (!content.trim() && !imageUrl && !videoUrl)}
              className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors font-medium"
            >
              {isLoading ? 'Posting...' : 'Post'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
