# Quick Start Guide

## Fastest Way to Run the Chat System

### Using Docker Compose (5 minutes)

1. **Prerequisites**: Install Docker Desktop
   - Download from: https://www.docker.com/products/docker-desktop

2. **Clone and Run**:
   ```bash
   git clone <your-repo-url>
   cd chat-system
   docker-compose up --build
   ```

3. **Access the Application**:
   - Open browser: http://localhost:3000
   - Enter username and display name
   - Start chatting!

### Default Chat Rooms

The system doesn't create default rooms. To test:

1. **Create a chat room** using the API:
   ```bash
   curl -X POST "http://localhost:5000/api/chatrooms?createdBy=user1" \
     -H "Content-Type: application/json" \
     -d '{
       "name": "General",
       "description": "General discussion room",
       "isPrivate": false
     }'
   ```

2. **Or use Swagger UI**:
   - Navigate to: http://localhost:5000/swagger
   - Use the interactive API documentation to create rooms

### Testing Multiple Users

1. Open multiple browser windows/tabs
2. Login with different usernames in each
3. Join the same chat room
4. See real-time messaging in action!

### Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                         Frontend                             │
│  (Next.js + React + TailwindCSS + SignalR Client)           │
│  Port: 3000                                                  │
└───────────────────────┬─────────────────────────────────────┘
                        │ HTTP/WebSocket
                        │
┌───────────────────────▼─────────────────────────────────────┐
│                    Backend API                               │
│              (.NET 8 + SignalR Hub)                         │
│              Port: 5000                                      │
│                                                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │   Domain     │  │ Application  │  │     API      │     │
│  │  (Entities)  │─▶│  (Services)  │─▶│ (Controllers)│     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│                           │                                  │
│                    ┌──────▼──────┐                          │
│                    │Infrastructure│                          │
│                    └──────┬───────┘                          │
└───────────────────────────┼─────────────────────────────────┘
                            │
            ┌───────────────┼───────────────┐
            │               │               │
    ┌───────▼─────┐  ┌─────▼─────┐  ┌─────▼─────┐
    │  MongoDB    │  │   Redis   │  │  SignalR  │
    │ Port: 27017 │  │Port: 6379 │  │ WebSocket │
    └─────────────┘  └───────────┘  └───────────┘
```

### Key Features to Test

1. **Real-time Messaging**: Send messages and see them appear instantly
2. **Typing Indicators**: Start typing and others see "... is typing"
3. **User Presence**: See who's online in the right sidebar
4. **Multiple Rooms**: Join different rooms for different conversations
5. **Message History**: Messages persist in MongoDB

### Stopping the Application

```bash
docker-compose down
```

To also remove volumes (database data):
```bash
docker-compose down -v
```

### Troubleshooting

**Can't access localhost:3000?**
- Check if containers are running: `docker ps`
- Check logs: `docker-compose logs frontend`

**SignalR not connecting?**
- Check backend logs: `docker-compose logs backend`
- Verify MongoDB and Redis are running: `docker-compose ps`

**Need to reset data?**
- Stop: `docker-compose down -v`
- Start fresh: `docker-compose up --build`

### Next Steps

- Read the full [README.md](README.md) for detailed documentation
- Explore the API with Swagger UI at http://localhost:5000/swagger
- Customize the UI in `frontend/components/`
- Add new features by following Clean Architecture patterns
