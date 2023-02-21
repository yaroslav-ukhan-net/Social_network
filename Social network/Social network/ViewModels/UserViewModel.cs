using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social_network.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AvatarURL { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Notes { get; set; }
        public Post NewPost { get; set; }
        public List<PostViewModel> Posts { get; set; }
        public string Email { get; set; }
        public int friendshipStatus { get; set; }

    }
    public enum friendshipStatusEnum
    {
        notFriends,
        requestForUser,
        areFriends,
        requestFromUser
    }
}
