﻿using Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social_network.ViewModels
{
    public class GroupsViewModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int GroupAdminId { get; set; }
        public string GroupName { get; set; }
        public bool IsClose { get; set; }
        public int GroupCountFollowers { get; set; }
        public DateTime GroupCreateDate { get; set; }
        public string GroupAvatarURL { get; set; }
        public string GroupNotes { get; set; }
        public Post NewPost { get; set; }
        public List<User> Users { get; set; }
        public List<PostViewModel> Posts { get; set; }
    }
}