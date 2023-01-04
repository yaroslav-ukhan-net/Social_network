using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Models.Models;
using Services;
using Social_network.Data;
using SocialNetwork.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Hubs
{
    public class ChatHub : Hub
    {
        private readonly MessageService _MessageService;
        private readonly UserService _UserService;

        public ChatHub(MessageService messageService, UserService userService)
        {
            _MessageService = messageService;
            _UserService = userService;
        }
        public async Task SendMessage(string groupName, string user, string message) //+
        {
            if (message != "")
            {
                Message newmes = new()
                {
                    Date = System.DateTime.Now,
                    Text = message,
                    IdSender = _UserService.GetUserById(Convert.ToInt32(user)).Id,
                    WithAnswer = false,
                    IsVisible = true,
                    ChatId = Convert.ToInt32(groupName),
                    Edited = false
                };
                _MessageService.CreateMessage(newmes);
                var userSender = _UserService.GetUserById(newmes.IdSender);

                ChatsViewModel chatsViewModel = new();
                chatsViewModel.AvatarURL = userSender.AvatarURL;
                chatsViewModel.Name =  userSender.Name + " " + userSender.Surname;
                chatsViewModel.Id = newmes.ChatId;
                chatsViewModel.LastMessage = new();
                chatsViewModel.LastMessage.Edited = newmes.Edited;
                chatsViewModel.LastMessage.Date = newmes.Date;
                chatsViewModel.LastMessage.Text = newmes.Text;
                chatsViewModel.LastMessage.Id = newmes.Id;
                await Clients.Group(groupName).SendAsync("ReceiveMessage", chatsViewModel);
            }
        }
        public async Task RemoveMes(string idmes, string groupName) //+
        {
            await Clients.Group(groupName).SendAsync("ReceiveRemove");
        }

            public async Task JoinRoom(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }
    }
}
