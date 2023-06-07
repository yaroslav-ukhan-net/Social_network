using Models;

namespace SocialNetwork.ViewModels
{
    public class SettingsVievModel
    {
        public int Id { get; set; }

        public int ChatInvites { get; set; }
        public int SeeMyGroups { get; set; }
        public int WriteToMe { get; set; }
        public int LeavePosts { get; set; }
        public int SeeMyPosts { get; set; }
        public int SeeMyFriends { get; set; }
        public int SeeMyPhone { get; set; }

        public virtual User User { get; set; }
        public int UserId { get; set; }
    }
}
