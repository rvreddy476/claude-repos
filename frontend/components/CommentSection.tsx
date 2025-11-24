'use client';

import React, { useState } from 'react';
import { Comment } from '@/types/feed';
import { formatDistanceToNow } from 'date-fns';

interface CommentSectionProps {
  postId: string;
  comments: Comment[];
  totalCommentsCount: number;
  currentUserId: string;
  onLoadMore: () => Promise<void>;
  onAddComment: (content: string) => Promise<void>;
  onToggleLike: (commentId: string) => Promise<void>;
}

export const CommentSection: React.FC<CommentSectionProps> = ({
  postId,
  comments,
  totalCommentsCount,
  currentUserId,
  onLoadMore,
  onAddComment,
  onToggleLike,
}) => {
  const [newComment, setNewComment] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isLoadingMore, setIsLoadingMore] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newComment.trim() || isSubmitting) return;

    setIsSubmitting(true);
    try {
      await onAddComment(newComment.trim());
      setNewComment('');
    } catch (error) {
      console.error('Failed to add comment:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleLoadMore = async () => {
    setIsLoadingMore(true);
    try {
      await onLoadMore();
    } catch (error) {
      console.error('Failed to load more comments:', error);
    } finally {
      setIsLoadingMore(false);
    }
  };

  return (
    <div className="space-y-3">
      {/* Comment Input */}
      <form onSubmit={handleSubmit} className="flex items-start space-x-2">
        <input
          type="text"
          value={newComment}
          onChange={(e) => setNewComment(e.target.value)}
          placeholder="Write a comment..."
          className="flex-1 px-3 py-2 bg-gray-100 rounded-full text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
          disabled={isSubmitting}
        />
        <button
          type="submit"
          disabled={!newComment.trim() || isSubmitting}
          className="px-4 py-2 bg-blue-600 text-white rounded-full text-sm font-medium hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors"
        >
          {isSubmitting ? 'Posting...' : 'Post'}
        </button>
      </form>

      {/* Comments List */}
      {comments.length > 0 && (
        <div className="space-y-3">
          {comments.map((comment) => (
            <div key={comment.id} className="flex items-start space-x-2">
              {/* User Avatar */}
              <div className="w-8 h-8 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center text-white font-bold text-xs flex-shrink-0">
                {comment.userDisplayName.charAt(0).toUpperCase()}
              </div>

              {/* Comment Content */}
              <div className="flex-1">
                <div className="bg-gray-100 rounded-2xl px-3 py-2">
                  <div className="font-semibold text-sm text-gray-900">{comment.userDisplayName}</div>
                  <p className="text-sm text-gray-800 mt-0.5">{comment.content}</p>
                </div>

                {/* Comment Actions */}
                <div className="flex items-center space-x-4 mt-1 px-3">
                  <button
                    onClick={() => onToggleLike(comment.id)}
                    className={`text-xs font-medium ${
                      comment.isLikedByCurrentUser ? 'text-blue-600' : 'text-gray-500 hover:text-blue-600'
                    } transition-colors`}
                  >
                    Like {comment.likesCount > 0 && `(${comment.likesCount})`}
                  </button>
                  <span className="text-xs text-gray-500">
                    {formatDistanceToNow(new Date(comment.createdAt), { addSuffix: true })}
                  </span>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Load More Button */}
      {comments.length < totalCommentsCount && (
        <button
          onClick={handleLoadMore}
          disabled={isLoadingMore}
          className="text-sm font-medium text-gray-600 hover:text-gray-900 transition-colors disabled:text-gray-400"
        >
          {isLoadingMore ? 'Loading...' : `Load more comments (${totalCommentsCount - comments.length} remaining)`}
        </button>
      )}
    </div>
  );
};
