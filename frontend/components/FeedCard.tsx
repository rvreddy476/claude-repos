'use client';

import React, { useState } from 'react';
import { Post, Comment } from '@/types/feed';
import { CommentSection } from './CommentSection';
import { formatDistanceToNow } from 'date-fns';

interface FeedCardProps {
  post: Post;
  currentUserId: string;
  comments: Comment[];
  onToggleLike: () => Promise<void>;
  onLoadMoreComments: () => Promise<void>;
  onAddComment: (content: string) => Promise<void>;
  onToggleCommentLike: (commentId: string) => Promise<void>;
  onShare: () => void;
}

export const FeedCard: React.FC<FeedCardProps> = ({
  post,
  currentUserId,
  comments,
  onToggleLike,
  onLoadMoreComments,
  onAddComment,
  onToggleCommentLike,
  onShare,
}) => {
  const [showComments, setShowComments] = useState(false);

  return (
    <div className="bg-white rounded-lg shadow mb-4">
      {/* Post Header */}
      <div className="p-4">
        <div className="flex items-center space-x-3">
          <div className="w-10 h-10 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center text-white font-bold">
            {post.userDisplayName.charAt(0).toUpperCase()}
          </div>
          <div className="flex-1">
            <div className="font-semibold text-gray-900">{post.userDisplayName}</div>
            <div className="text-xs text-gray-500">
              {formatDistanceToNow(new Date(post.createdAt), { addSuffix: true })}
            </div>
          </div>
        </div>

        {/* Post Content */}
        {post.content && (
          <p className="mt-3 text-gray-800 whitespace-pre-wrap">{post.content}</p>
        )}
      </div>

      {/* Post Media */}
      {post.imageUrl && (
        <div className="w-full">
          <img
            src={post.imageUrl}
            alt="Post"
            className="w-full object-cover max-h-96"
          />
        </div>
      )}

      {post.videoUrl && (
        <div className="w-full">
          <video
            src={post.videoUrl}
            controls
            className="w-full max-h-96"
          />
        </div>
      )}

      {/* Post Stats */}
      <div className="px-4 py-2 border-t border-gray-200">
        <div className="flex items-center justify-between text-sm text-gray-600">
          <span>{post.likesCount} {post.likesCount === 1 ? 'like' : 'likes'}</span>
          <div className="flex items-center space-x-3">
            <span>{post.commentsCount} {post.commentsCount === 1 ? 'comment' : 'comments'}</span>
            <span>{post.sharesCount} {post.sharesCount === 1 ? 'share' : 'shares'}</span>
          </div>
        </div>
      </div>

      {/* Action Buttons */}
      <div className="px-4 py-2 border-t border-gray-200">
        <div className="flex items-center justify-around">
          {/* Like Button */}
          <button
            onClick={onToggleLike}
            className={`flex items-center space-x-2 px-4 py-2 rounded-lg hover:bg-gray-100 transition-colors ${
              post.isLikedByCurrentUser ? 'text-blue-600' : 'text-gray-700'
            }`}
          >
            <svg
              className="w-5 h-5"
              fill={post.isLikedByCurrentUser ? 'currentColor' : 'none'}
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M14 10h4.764a2 2 0 011.789 2.894l-3.5 7A2 2 0 0115.263 21h-4.017c-.163 0-.326-.02-.485-.06L7 20m7-10V5a2 2 0 00-2-2h-.095c-.5 0-.905.405-.905.905 0 .714-.211 1.412-.608 2.006L7 11v9m7-10h-2M7 20H5a2 2 0 01-2-2v-6a2 2 0 012-2h2.5"
              />
            </svg>
            <span className="font-medium">Like</span>
          </button>

          {/* Comment Button */}
          <button
            onClick={() => setShowComments(!showComments)}
            className="flex items-center space-x-2 px-4 py-2 rounded-lg hover:bg-gray-100 transition-colors text-gray-700"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"
              />
            </svg>
            <span className="font-medium">Comment</span>
          </button>

          {/* Share Button */}
          <button
            onClick={onShare}
            className="flex items-center space-x-2 px-4 py-2 rounded-lg hover:bg-gray-100 transition-colors text-gray-700"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z"
              />
            </svg>
            <span className="font-medium">Share</span>
          </button>
        </div>
      </div>

      {/* Comment Section */}
      {showComments && (
        <div className="px-4 py-3 border-t border-gray-200 bg-gray-50">
          <CommentSection
            postId={post.id}
            comments={comments}
            totalCommentsCount={post.commentsCount}
            currentUserId={currentUserId}
            onLoadMore={onLoadMoreComments}
            onAddComment={onAddComment}
            onToggleLike={onToggleCommentLike}
          />
        </div>
      )}
    </div>
  );
};
