using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Services;
using Social_network.Data;
using SocialNetwork.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ChatService _ChatService;
        private readonly MessageService _MessageService;
        private readonly UserService _UserService;
        private readonly FriendService _FriendService;
        private readonly UserManager<AppUser> _UserManager;

        public ChatController(ChatService chatService, MessageService messageService, UserService userService, FriendService friendService , UserManager<AppUser> userManager)
        {
            _ChatService = chatService;
            _MessageService = messageService;
            _UserService = userService;
            _FriendService = friendService;
            _UserManager = userManager;
        }
        public IActionResult AllChats()
        {
            int userId = _UserManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                userId = _UserManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();

            var chatsGroup = _ChatService.GetChats(chat => (chat.UserChat.Any(u => u.UserId == userId) && chat.Messages.Count >= 1) ||
                (( chat.UserChat.Any(u => u.UserId == userId) && chat.IsGroup)) ).ToList();

            var model = new UserChatViewModel();
            model.UserChatsList = new();

            foreach (var ch in chatsGroup)
            {
                ChatsViewModel chatsViewModel = new();
                chatsViewModel.Id = ch.Id;
                chatsViewModel.IsGroup = ch.IsGroup;
                if (!(ch.IsGroup && ch.Messages.Count==0))
                {
                    chatsViewModel.LastMessage = ch.Messages.Last();
                    if (chatsViewModel.LastMessage.Text.Length > 30) chatsViewModel.LastMessage.Text = chatsViewModel.LastMessage.Text.Substring(0, 30) + "...";

                    int idLastMessageSender = chatsViewModel.LastMessage.IdSender;
                    var userLastMessage = _UserService.GetUserById(idLastMessageSender);
                }
                if (ch.IsGroup) chatsViewModel.Name = ch.Name; //group chat

                else //private chat
                {
                    if (ch.UserChat[0].UserId == userId)
                    {
                        chatsViewModel.Name = ch.UserChat[1].User.Name + " " + ch.UserChat[1].User.Surname;
                        chatsViewModel.AvatarURL = ch.UserChat[1].User.AvatarURL;
                    }

                    else
                    {
                        chatsViewModel.Name = ch.UserChat[0].User.Name + " " + ch.UserChat[0].User.Surname;
                        chatsViewModel.AvatarURL = ch.UserChat[0].User.AvatarURL;
                    }
                }

                model.UserChatsList.Add(chatsViewModel);
            }

            return View(model);
        }

        public IActionResult CreateOrChooseChat(int foreignUserId)
        {
            int myUserId = _UserManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
               myUserId = _UserManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();

            int idChat;

            var chatsGroup = _ChatService.GetChats(chat => chat.UserChat.Any(u => u.UserId == myUserId) && !chat.IsGroup);
            var chatsGroupList = chatsGroup.Where(chat => chat.UserChat.Any(u => u.UserId == foreignUserId)).ToList();
            if (chatsGroupList.Count == 0)
            {
                Chat chat = new()
                {
                    IsGroup = false,

                };
                chat.UserChat.Add(new UserChat() { Chat = chat, UserId = myUserId });
                chat.UserChat.Add(new UserChat() { Chat = chat, UserId = foreignUserId });
                _ChatService.CreateChat(chat);

                var newchatsGroup = _ChatService.GetChats(chat => chat.UserChat.Any(u => u.UserId == myUserId) && !chat.IsGroup);
                var newchatsGroupList = newchatsGroup.Where(chat => chat.UserChat.Any(u => u.UserId == foreignUserId)).ToList();
                idChat = newchatsGroupList[0].Id;
            }
            else
            {
                idChat = chatsGroupList[0].Id;
            }

            return RedirectToAction("ChatWithUser", new { id = idChat });
        }
        public IActionResult ChatWithUser(int id)
        {
            int userId = _UserManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                userId = _UserManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();

            var ch = _ChatService.GetChatById(id);
            if(ch == null) return NotFound();

            ChatsViewModel chatsViewModel = new();
            chatsViewModel.Id = ch.Id;
            chatsViewModel.IsGroup = ch.IsGroup;
            chatsViewModel.MyUserId = userId;

            if (chatsViewModel.IsGroup) chatsViewModel.Name = ch.Name; //group chat

            else //private chat
            {
                if (ch.UserChat[0].UserId == userId)
                {
                    chatsViewModel.Name = ch.UserChat[1].User.Name + " " + ch.UserChat[1].User.Surname;
                    chatsViewModel.AvatarURL = ch.UserChat[1].User.AvatarURL;
                }

                else
                {
                    chatsViewModel.Name = ch.UserChat[0].User.Name + " " + ch.UserChat[0].User.Surname;
                    chatsViewModel.AvatarURL = ch.UserChat[0].User.AvatarURL;
                }
            }

            foreach (var m in ch.Messages)
            {
                chatsViewModel.Messages.Add(new()
                {
                    AnswerText = m.AnswerText,
                    Date = m.Date,
                    Edited = m.Edited,
                    Id = m.Id,
                    IdSender = m.IdSender,
                    IsVisible = m.IsVisible,
                    Text = m.Text,
                    WithAnswer = m.WithAnswer,
                    ChatId = m.ChatId,
                    MessageSenderAvatar = _UserService.GetUserById(m.IdSender).AvatarURL,
                    MessageSenderName = _UserService.GetUserById(m.IdSender).Name + " " + _UserService.GetUserById(m.IdSender).Surname,
                    EmailSender = _UserService.GetUserById(m.IdSender).Email
                });
            }

            return View(chatsViewModel);
        }
        [HttpGet]
        public IActionResult CreateChatGroup()
        {
            int id = _UserManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                id = _UserManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            ViewData["Action"] = "CreateChatGroup";
            var model = new ChatsViewModel();
           
            model.AvatarURL = "https://i.imgur.com/iBcCrRu.jpg";

            var userFriends1Half = _FriendService.GetAllFriendsQuerible(c =>
               ((c.Status == (int)StatusFriendship.areFriends) &&
               (c.FirstFriendId == id))).ToList();
            foreach (var f in userFriends1Half)
            {
                model.UserChat.Add(new UserChat()
                {
                    User = f.SecondFriend,
                     UserId = f.SecondFriendId
                }) ;
            }


            var userFriends_2Half = _FriendService.GetAllFriendsQuerible(c =>
               ((c.Status == (int)StatusFriendship.areFriends) &&
               (c.SecondFriendId == id))).ToList();
            foreach (var f in userFriends_2Half)
            {
                model.UserChat.Add(new UserChat()
                {
                    User = f.FirstFriend,
                    UserId = f.FirstFriendId
                });
            }


            return View("EditChatGroup", model);
        }
        [HttpPost]
        public IActionResult CreateChatGroup(ChatsViewModel chatsViewModel)
        {
            int id = _UserManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                id = _UserManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            chatsViewModel.UserChat.Add(new UserChat() { UserId = id, ConsistInGroupChat = true });
            Chat newgroup = new()
            {

                AdminId = id,
                AvatarURL = chatsViewModel.AvatarURL,
                Name = chatsViewModel.Name,
                IsGroup = true,
                UserChat = chatsViewModel.UserChat
            };
            _ChatService.CreateChat(newgroup);

            return RedirectToAction("AllChats");
        }
        public IActionResult AllParticipants(int idChat)
        {
            int id = _UserManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                id = _UserManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();

            var chat = _ChatService.GetChatById(idChat);
            if (chat.UserChat.Any(c => c.UserId == id))
            {
                var model = new ChatsViewModel();
                foreach (var c in chat.UserChat)
                {
                    model.UserChat.Add(c);
                    model.AdminId = c.Chat.AdminId;
                    model.IsGroup = c.Chat.IsGroup;
                } 
                return View(model);
            }
            else return Forbid();

        }
        
    }
}
