using Models.Models;
using SocialNetwork.CheckingAccess;

namespace SocialNetwork.ViewModels
{
    public class PageAccess
    {
        public bool AccessSeeMyPhone { get; set; }
        public bool AccessChatInvites { get; set; }
        public bool AccessSeeMyGroups { get; set; }
        public bool AccessWriteToMe { get; set; }
        public bool AccessLeavePosts { get; set; }
        public bool AccessSeeMyPosts { get; set; }
        public bool AccessSeeMyFriends { get; set; }
    }
}
