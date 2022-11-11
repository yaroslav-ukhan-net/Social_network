using Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social_network.ViewModels
{
    public class UserGroupsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AvatarURL { get; set; }
        public virtual List<GroupsViewModel> Groups { get; set; }
    }
    public class GroupsViewModel
    {
        public int GroupId { get; set; }
        public int GroupAdminId { get; set; }
        public string GroupName { get; set; }
        public int GroupCountFollowers { get; set; }
        public DateTime GroupCreateDate { get; set; }
        public string GroupAvatarURL { get; set; }
        public string GroupNotes { get; set; }
    }
}
