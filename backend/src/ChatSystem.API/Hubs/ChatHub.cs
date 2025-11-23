using ChatSystem.Application.DTOs;
using ChatSystem.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatSystem.API.Hubs;

public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    private readonly IChatRoomService _chatRoomService;

    public ChatHub(
        IMessageService messageService,
        IUserService userService,
        IChatRoomService chatRoomService)
    {
        _messageService = messageService;
        _userService = userService;
        _chatRoomService = chatRoomService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();

        if (!string.IsNullOrEmpty(userId))
        {
            await _userService.UpdateConnectionIdAsync(userId, Context.ConnectionId);
            await _userService.SetUserOnlineStatusAsync(userId, true);

            // Notify others that user is online
            await Clients.Others.SendAsync("UserOnline", userId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();

        if (!string.IsNullOrEmpty(userId))
        {
            await _userService.UpdateConnectionIdAsync(userId, null);
            await _userService.SetUserOnlineStatusAsync(userId, false);

            // Notify others that user is offline
            await Clients.Others.SendAsync("UserOffline", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        await Clients.Group(roomId).SendAsync("UserJoinedRoom", userId, roomId);
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        await Clients.Group(roomId).SendAsync("UserLeftRoom", userId, roomId);
    }

    public async Task SendMessage(SendMessageDto sendMessageDto)
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();

        if (string.IsNullOrEmpty(userId))
            return;

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return;

        var message = await _messageService.SendMessageAsync(
            sendMessageDto,
            userId,
            user.DisplayName);

        // Send message to all users in the room
        await Clients.Group(sendMessageDto.ChatRoomId).SendAsync("ReceiveMessage", message);
    }

    public async Task UserTyping(string roomId)
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        var user = await _userService.GetUserByIdAsync(userId ?? "");

        if (user != null)
        {
            await Clients.OthersInGroup(roomId).SendAsync("UserTyping", user.DisplayName, roomId);
        }
    }

    public async Task UserStoppedTyping(string roomId)
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        var user = await _userService.GetUserByIdAsync(userId ?? "");

        if (user != null)
        {
            await Clients.OthersInGroup(roomId).SendAsync("UserStoppedTyping", user.DisplayName, roomId);
        }
    }
}
