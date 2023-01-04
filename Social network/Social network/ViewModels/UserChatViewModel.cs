using Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.ViewModels
{
    public class UserChatViewModel
    {
        public virtual List<ChatsViewModel> UserChatsList { get; set; } = new();
    }
    public class ChatsViewModel
    {
        public int Id { get; set; }
        public int MyUserId { get; set; }
        public bool IsGroup { get; set; }
        public string Name { get; set; }
        public string AvatarURL { get; set; }
        public int AdminId { get; set; }
        public Message LastMessage { get; set; } = new();
        public string jsFieldDateSms { get; set; }
        public virtual List<UserChat> UserChat { get; set; } = new();
        public virtual List<MessageViewModel> Messages { get; set; } = new();
    }
}
