using Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social_network.ViewModels
{
    public class GroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AvatarURL { get; set; }
        public List<GroupsViewModel> Groups { get; set; }
    }
    public class GroupsViewModel
    {
        public int GroupId { get; set; }
        public string GroupAdminName { get; set; }
        public string GroupAdminId { get; set; }
        public string GroupAdminUsername { get; set; }
        public bool UserIsModerator { get; set; }
        public bool MyUserConsistInGroup { get; set; }
        public string GroupName { get; set; }
        public bool IsClose { get; set; }
        public int GroupCountFollowers { get; set; }
        public DateTime GroupCreateDate { get; set; }
        public string GroupAvatarURL { get; set; }
        public string GroupNotes { get; set; }
        public int GroupCountFollowersRequests { get; set; }
        public Post NewPost { get; set; }
        public List<User> Users { get; set; }
        public List<ModeratorsViewModel> Moderators { get; set; }
        public List<PostViewModel> Posts { get; set; }
    }
}
