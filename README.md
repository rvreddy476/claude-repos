# Chat System POC

A real-time chat system built with .NET 8, SignalR, MongoDB, Redis, Next.js, and React following Clean Architecture principles.

## Architecture

### Backend (.NET 8)
- **Clean Architecture** with four layers:
  - **Domain Layer**: Entities and Interfaces
  - **Application Layer**: Services, DTOs, and Business Logic
  - **Infrastructure Layer**: MongoDB and Redis implementations
  - **API Layer**: REST Controllers and SignalR Hub

### Frontend (Next.js + React)
- **Next.js 14** with TypeScript
- **React Components** for UI
- **TailwindCSS** for styling
- **SignalR Client** for real-time communication

### Technologies
- **.NET 8** - Backend framework
- **SignalR** - Real-time WebSocket communication
- **MongoDB** - Message and data persistence
- **Redis** - Caching layer
- **Next.js** - Frontend framework
- **React** - UI components
- **TypeScript** - Type safety
- **TailwindCSS** - Styling
- **Docker & Docker Compose** - Containerization

## Features

- ✅ Real-time messaging with SignalR
- ✅ Multiple chat rooms
- ✅ User presence (online/offline status)
- ✅ Typing indicators
- ✅ Message history
- ✅ Clean Architecture
- ✅ Redis caching
- ✅ MongoDB persistence
- ✅ Responsive UI design
- ✅ Docker support

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker & Docker Compose](https://www.docker.com/)
- [MongoDB](https://www.mongodb.com/) (if running locally)
- [Redis](https://redis.io/) (if running locally)

## Project Structure

```
chat-system/
├── backend/
│   ├── src/
│   │   ├── ChatSystem.Domain/          # Entities and Interfaces
│   │   │   ├── Entities/
│   │   │   │   ├── User.cs
│   │   │   │   ├── ChatRoom.cs
│   │   │   │   └── Message.cs
│   │   │   └── Interfaces/
│   │   │       ├── IUserRepository.cs
│   │   │       ├── IChatRoomRepository.cs
│   │   │       ├── IMessageRepository.cs
│   │   │       └── ICacheService.cs
│   │   ├── ChatSystem.Application/     # Services and DTOs
│   │   │   ├── Services/
│   │   │   │   ├── UserService.cs
│   │   │   │   ├── ChatRoomService.cs
│   │   │   │   └── MessageService.cs
│   │   │   └── DTOs/
│   │   ├── ChatSystem.Infrastructure/  # MongoDB and Redis
│   │   │   ├── Repositories/
│   │   │   ├── Caching/
│   │   │   └── Persistence/
│   │   └── ChatSystem.API/             # Controllers and SignalR Hub
│   │       ├── Controllers/
│   │       ├── Hubs/
│   │       │   └── ChatHub.cs
│   │       └── Program.cs
│   ├── ChatSystem.sln
│   └── Dockerfile
├── frontend/
│   ├── app/
│   │   ├── page.tsx                    # Main chat page
│   │   ├── layout.tsx
│   │   └── globals.css
│   ├── components/
│   │   ├── ChatMessage.tsx
│   │   ├── ChatWindow.tsx
│   │   ├── ChatRoomList.tsx
│   │   ├── MessageInput.tsx
│   │   ├── UserList.tsx
│   │   └── LoginForm.tsx
│   ├── lib/
│   │   ├── signalr.ts                  # SignalR connection
│   │   └── api.ts                      # REST API calls
│   ├── types/
│   │   └── chat.ts
│   ├── package.json
│   └── Dockerfile
├── docker-compose.yml
└── README.md
```

## Getting Started

### Option 1: Using Docker Compose (Recommended)

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd chat-system
   ```

2. **Run with Docker Compose**
   ```bash
   docker-compose up --build
   ```

3. **Access the application**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger

### Option 2: Running Locally

#### Backend Setup

1. **Navigate to backend directory**
   ```bash
   cd backend
   ```

2. **Start MongoDB and Redis (using Docker)**
   ```bash
   docker run -d -p 27017:27017 --name mongodb mongo:7.0
   docker run -d -p 6379:6379 --name redis redis:7-alpine
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Run the API**
   ```bash
   cd src/ChatSystem.API
   dotnet run
   ```

   The API will be available at http://localhost:5000

#### Frontend Setup

1. **Navigate to frontend directory**
   ```bash
   cd frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Run the development server**
   ```bash
   npm run dev
   ```

   The frontend will be available at http://localhost:3000

## API Endpoints

### Users
- `GET /api/users` - Get all users
- `GET /api/users/online` - Get online users
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/username/{username}` - Get user by username
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Chat Rooms
- `GET /api/chatrooms` - Get all chat rooms
- `GET /api/chatrooms/{id}` - Get chat room by ID
- `GET /api/chatrooms/user/{userId}` - Get user's chat rooms
- `POST /api/chatrooms` - Create new chat room
- `POST /api/chatrooms/{roomId}/participants/{userId}` - Add participant
- `DELETE /api/chatrooms/{roomId}/participants/{userId}` - Remove participant

### Messages
- `GET /api/messages/{id}` - Get message by ID
- `GET /api/messages/room/{chatRoomId}` - Get room messages
- `PUT /api/messages/{id}` - Update message
- `DELETE /api/messages/{id}` - Delete message

### SignalR Hub
- **Hub URL**: `/chatHub`
- **Methods**:
  - `JoinRoom(roomId)` - Join a chat room
  - `LeaveRoom(roomId)` - Leave a chat room
  - `SendMessage(sendMessageDto)` - Send a message
  - `UserTyping(roomId)` - Notify typing status
  - `UserStoppedTyping(roomId)` - Notify stopped typing

## Configuration

### Backend (appsettings.json)
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "ChatSystemDb"
  },
  "RedisSettings": {
    "ConnectionString": "localhost:6379",
    "DefaultExpirationMinutes": 30
  }
}
```

### Frontend (lib/api.ts & lib/signalr.ts)
- API Base URL: `http://localhost:5000/api`
- SignalR Hub URL: `http://localhost:5000/chatHub`

## Clean Architecture Layers

### 1. Domain Layer
- Contains entities (User, ChatRoom, Message)
- Defines repository interfaces
- No dependencies on other layers

### 2. Application Layer
- Business logic and use cases
- Service implementations
- DTOs for data transfer
- Depends only on Domain layer

### 3. Infrastructure Layer
- MongoDB repository implementations
- Redis caching service
- External service integrations
- Depends on Domain layer

### 4. API/Presentation Layer
- REST API controllers
- SignalR hub
- Dependency injection configuration
- Depends on Application and Infrastructure layers

## Usage

1. **Login**: Enter a username and display name
2. **Select Chat Room**: Click on a room from the left sidebar
3. **Send Message**: Type in the input field and press Enter or click Send
4. **View Online Users**: See online users in the right sidebar
5. **Real-time Updates**: Messages appear instantly via SignalR

## Development

### Adding a New Feature

1. **Domain Layer**: Add entities and interfaces if needed
2. **Application Layer**: Create DTOs and services
3. **Infrastructure Layer**: Implement repository/service interfaces
4. **API Layer**: Add controllers and/or SignalR methods
5. **Frontend**: Create React components and integrate with API

### Building for Production

#### Backend
```bash
cd backend
dotnet publish -c Release -o ./publish
```

#### Frontend
```bash
cd frontend
npm run build
```

## Testing

### Backend
```bash
cd backend
dotnet test
```

### Frontend
```bash
cd frontend
npm test
```

## Troubleshooting

### MongoDB Connection Issues
- Ensure MongoDB is running: `docker ps | grep mongo`
- Check connection string in appsettings.json

### Redis Connection Issues
- Ensure Redis is running: `docker ps | grep redis`
- Check connection string in appsettings.json

### SignalR Connection Issues
- Check CORS settings in Program.cs
- Verify frontend is using correct hub URL
- Check browser console for WebSocket errors

### Port Already in Use
```bash
# Kill process on port 5000 (backend)
lsof -ti:5000 | xargs kill -9

# Kill process on port 3000 (frontend)
lsof -ti:3000 | xargs kill -9
```

## Future Enhancements

- [ ] User authentication with JWT
- [ ] File sharing
- [ ] Message reactions
- [ ] Private messaging
- [ ] Message search
- [ ] User profiles with avatars
- [ ] Push notifications
- [ ] Message encryption
- [ ] Video/Audio calls
- [ ] Unit and integration tests

## License

This is a POC (Proof of Concept) project for demonstration purposes.

## Contributing

This is a proof of concept project. Feel free to fork and modify as needed.

## Support

For issues or questions, please create an issue in the repository.
