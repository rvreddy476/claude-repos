export enum PostType {
  Text = 0,
  Image = 1,
  Video = 2,
  Mixed = 3,
}

export interface Post {
  id: string;
  userId: string;
  userDisplayName: string;
  userAvatarUrl?: string;
  content?: string;
  imageUrl?: string;
  videoUrl?: string;
  type: PostType;
  createdAt: Date;
  updatedAt?: Date;
  likesCount: number;
  commentsCount: number;
  sharesCount: number;
  isLikedByCurrentUser: boolean;
}

export interface Comment {
  id: string;
  postId: string;
  userId: string;
  userDisplayName: string;
  userAvatarUrl?: string;
  content: string;
  createdAt: Date;
  updatedAt?: Date;
  likesCount: number;
  isLikedByCurrentUser: boolean;
}

export interface CreatePostDto {
  content?: string;
  imageUrl?: string;
  videoUrl?: string;
  type: PostType;
}

export interface CreateCommentDto {
  postId: string;
  content: string;
}
