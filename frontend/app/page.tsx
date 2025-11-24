'use client';

import React, { useState, useEffect } from 'react';
import { LoginForm } from '@/components/LoginForm';
import { RegisterForm } from '@/components/RegisterForm';
import { UserList } from '@/components/UserList';
import { ChatPopup, DirectChatMessage } from '@/components/ChatPopup';
import { SidebarMenu } from '@/components/SidebarMenu';
import { PostCreator } from '@/components/PostCreator';
import { FeedCard } from '@/components/FeedCard';
import { User } from '@/types/chat';
import { Post, Comment, CreatePostDto, CreateCommentDto, PostType } from '@/types/feed';
import { api } from '@/lib/api';
import { chatHub } from '@/lib/signalr';
import { FeedHubConnection } from '@/lib/feedHub';

const feedHub = new FeedHubConnection();

interface OpenChat {
  user: User;
  isMinimized: boolean;
  messages: DirectChatMessage[];
}

interface PostWithComments {
  post: Post;
  comments: Comment[];
  commentsLoaded: number;
}

export default function Home() {
  const [currentUser, setCurrentUser] = useState<User | null>(null);
  const [users, setUsers] = useState<User[]>([]);
  const [showRegister, setShowRegister] = useState(false);
  const [openChats, setOpenChats] = useState<OpenChat[]>([]);

  // Feed state
  const [posts, setPosts] = useState<PostWithComments[]>([]);
  const [isLoadingPosts, setIsLoadingPosts] = useState(false);

  useEffect(() => {
    if (currentUser) {
      loadInitialData();
      setupSignalR();
      setupFeedHub();
    }
  }, [currentUser]);

  const loadInitialData = async () => {
    try {
      const [usersData, postsData] = await Promise.all([
        api.getOnlineUsers(),
        api.getPosts(currentUser?.id),
      ]);

      setUsers(usersData);

      const postsWithComments: PostWithComments[] = await Promise.all(
        postsData.map(async (post) => ({
          post,
          comments: await api.getPostComments(post.id, currentUser?.id, 0, 3),
          commentsLoaded: 3,
        }))
      );

      setPosts(postsWithComments);
    } catch (error) {
      console.error('Error loading data:', error);
    }
  };

  const setupSignalR = async () => {
    if (!currentUser) return;

    try {
      await chatHub.start(currentUser.id);

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

      chatHub.onReceiveDirectMessage(async (directMessage: any) => {
        console.log('✉️ Received direct message:', directMessage);
        const formattedMessage: DirectChatMessage = {
          id: directMessage.id,
          senderId: directMessage.senderId,
          senderName: directMessage.senderName,
          recipientId: directMessage.recipientId,
          content: directMessage.content,
          timestamp: new Date(directMessage.timestamp),
        };

        setOpenChats((prev) => {
          const existingChat = prev.find((chat) => chat.user.id === directMessage.senderId);

          if (existingChat) {
            return prev.map((chat) =>
              chat.user.id === directMessage.senderId
                ? { ...chat, messages: [...chat.messages, formattedMessage], isMinimized: false }
                : chat
            );
          } else {
            api.getUserById(directMessage.senderId).then((user) => {
              setOpenChats((prevChats) => {
                let updatedChats = [...prevChats];
                if (updatedChats.length >= 3) {
                  updatedChats = updatedChats.slice(-2);
                }
                return [...updatedChats, { user, isMinimized: false, messages: [formattedMessage] }];
              });
            });
            return prev;
          }
        });
      });

      chatHub.onDirectMessageSent((directMessage: any) => {
        const formattedMessage: DirectChatMessage = {
          id: directMessage.id,
          senderId: directMessage.senderId,
          senderName: directMessage.senderName,
          recipientId: directMessage.recipientId,
          content: directMessage.content,
          timestamp: new Date(directMessage.timestamp),
        };

        setOpenChats((prev) => {
          const updatedChats = prev.map((chat) =>
            chat.user.id === directMessage.recipientId
              ? { ...chat, messages: [...chat.messages, formattedMessage] }
              : chat
          );
          return updatedChats;
        });
      });
    } catch (error) {
      console.error('Error setting up SignalR:', error);
    }
  };

  const setupFeedHub = async () => {
    try {
      await feedHub.start();
      console.log('FeedHub connected');

      feedHub.onNewPost(async (newPost: Post) => {
        console.log('New post received:', newPost);
        const comments = await api.getPostComments(newPost.id, currentUser?.id, 0, 3);
        setPosts((prev) => [{ post: newPost, comments, commentsLoaded: 3 }, ...prev]);
      });

      feedHub.onNewComment((newComment: Comment) => {
        console.log('New comment received:', newComment);
        setPosts((prev) =>
          prev.map((p) =>
            p.post.id === newComment.postId
              ? {
                  ...p,
                  post: { ...p.post, commentsCount: p.post.commentsCount + 1 },
                  comments: [newComment, ...p.comments].slice(0, p.commentsLoaded),
                }
              : p
          )
        );
      });

      feedHub.onPostLikeUpdated((data) => {
        setPosts((prev) =>
          prev.map((p) =>
            p.post.id === data.postId
              ? {
                  ...p,
                  post: {
                    ...p.post,
                    likesCount: data.likesCount,
                    isLikedByCurrentUser: data.userId === currentUser?.id ? data.isLiked : p.post.isLikedByCurrentUser,
                  },
                }
              : p
          )
        );
      });

      feedHub.onCommentLikeUpdated((data) => {
        setPosts((prev) =>
          prev.map((p) => ({
            ...p,
            comments: p.comments.map((c) =>
              c.id === data.commentId
                ? {
                    ...c,
                    likesCount: data.likesCount,
                    isLikedByCurrentUser: data.userId === currentUser?.id ? data.isLiked : c.isLikedByCurrentUser,
                  }
                : c
            ),
          }))
        );
      });

      feedHub.onPostDeleted((postId) => {
        setPosts((prev) => prev.filter((p) => p.post.id !== postId));
      });
    } catch (error) {
      console.error('Error setting up FeedHub:', error);
    }
  };

  const handleLogin = async (username: string) => {
    try {
      const user = await api.getUserByUsername(username);
      if (!user) {
        throw new Error('User not found. Please sign up first.');
      }
      setCurrentUser(user);
    } catch (error: any) {
      console.error('Error logging in:', error);
      throw error;
    }
  };

  const handleRegister = async (username: string, displayName: string) => {
    try {
      const existingUser = await api.getUserByUsername(username);
      if (existingUser) {
        throw new Error('Username already taken. Please choose another.');
      }
      const user = await api.createUser({ username, displayName });
      setShowRegister(false);
      return user;
    } catch (error: any) {
      console.error('Error registering:', error);
      throw error;
    }
  };

  const handleCreatePost = async (createPostDto: CreatePostDto) => {
    if (!currentUser) return;
    try {
      await api.createPost(currentUser.id, createPostDto);
    } catch (error) {
      console.error('Error creating post:', error);
    }
  };

  const handleTogglePostLike = async (postId: string) => {
    if (!currentUser) return;
    try {
      await api.togglePostLike(postId, currentUser.id);
    } catch (error) {
      console.error('Error toggling post like:', error);
    }
  };

  const handleLoadMoreComments = async (postId: string) => {
    if (!currentUser) return;
    try {
      const postWithComments = posts.find((p) => p.post.id === postId);
      if (!postWithComments) return;

      const skip = postWithComments.commentsLoaded;
      const newComments = await api.getPostComments(postId, currentUser.id, skip, 3);

      setPosts((prev) =>
        prev.map((p) =>
          p.post.id === postId
            ? {
                ...p,
                comments: [...p.comments, ...newComments],
                commentsLoaded: p.commentsLoaded + 3,
              }
            : p
        )
      );
    } catch (error) {
      console.error('Error loading more comments:', error);
    }
  };

  const handleAddComment = async (postId: string, content: string) => {
    if (!currentUser) return;
    try {
      await api.createComment(currentUser.id, { postId, content });
    } catch (error) {
      console.error('Error adding comment:', error);
    }
  };

  const handleToggleCommentLike = async (commentId: string) => {
    if (!currentUser) return;
    try {
      await api.toggleCommentLike(commentId, currentUser.id);
    } catch (error) {
      console.error('Error toggling comment like:', error);
    }
  };

  const handleSharePost = (postId: string) => {
    console.log('Share post:', postId);
    alert('Share functionality coming soon!');
  };

  const handleUserClick = (user: User) => {
    const existingChat = openChats.find((chat) => chat.user.id === user.id);

    if (existingChat) {
      setOpenChats((prev) =>
        prev.map((chat) =>
          chat.user.id === user.id ? { ...chat, isMinimized: false } : chat
        )
      );
    } else {
      setOpenChats((prev) => {
        let updatedChats = [...prev];
        if (updatedChats.length >= 3) {
          updatedChats = updatedChats.slice(-2);
        }
        return [...updatedChats, { user, isMinimized: false, messages: [] }];
      });
    }
  };

  const handleSendDirectMessage = async (recipientUserId: string, content: string) => {
    try {
      await chatHub.sendDirectMessage(recipientUserId, content);
    } catch (error) {
      console.error('Error sending direct message:', error);
    }
  };

  const handleCloseChat = (userId: string) => {
    setOpenChats((prev) => prev.filter((chat) => chat.user.id !== userId));
  };

  const handleMinimizeChat = (userId: string) => {
    setOpenChats((prev) =>
      prev.map((chat) =>
        chat.user.id === userId ? { ...chat, isMinimized: !chat.isMinimized } : chat
      )
    );
  };

  if (!currentUser) {
    if (showRegister) {
      return (
        <RegisterForm
          onRegister={handleRegister}
          onSwitchToLogin={() => setShowRegister(false)}
        />
      );
    }
    return (
      <LoginForm
        onLogin={handleLogin}
        onSwitchToRegister={() => setShowRegister(true)}
      />
    );
  }

  return (
    <div className="h-screen flex bg-gray-100">
      {/* Left Sidebar - Menu (2 cols) */}
      <div className="w-64 flex-shrink-0">
        <SidebarMenu currentUser={currentUser} />
      </div>

      {/* Center - Feed (6 cols equivalent) */}
      <div className="flex-1 overflow-y-auto px-4 py-6">
        <div className="max-w-2xl mx-auto">
          <PostCreator currentUser={currentUser} onCreatePost={handleCreatePost} />

          {posts.map(({ post, comments }) => (
            <FeedCard
              key={post.id}
              post={post}
              currentUserId={currentUser.id}
              comments={comments}
              onToggleLike={() => handleTogglePostLike(post.id)}
              onLoadMoreComments={() => handleLoadMoreComments(post.id)}
              onAddComment={(content) => handleAddComment(post.id, content)}
              onToggleCommentLike={handleToggleCommentLike}
              onShare={() => handleSharePost(post.id)}
            />
          ))}

          {posts.length === 0 && !isLoadingPosts && (
            <div className="text-center py-12 text-gray-500">
              <p>No posts yet. Create the first one!</p>
            </div>
          )}
        </div>
      </div>

      {/* Right Sidebar - Users */}
      <div className="w-80 flex-shrink-0">
        <UserList
          users={users}
          currentUserId={currentUser.id}
          onUserClick={handleUserClick}
        />
      </div>

      {/* Chat Popups */}
      {openChats.map((chat, index) => (
        <div
          key={chat.user.id}
          style={{ right: `${20 + index * 340}px` }}
        >
          <ChatPopup
            user={chat.user}
            currentUserId={currentUser.id}
            messages={chat.messages}
            onClose={() => handleCloseChat(chat.user.id)}
            onMinimize={() => handleMinimizeChat(chat.user.id)}
            onSendMessage={handleSendDirectMessage}
            isMinimized={chat.isMinimized}
          />
        </div>
      ))}
    </div>
  );
}
