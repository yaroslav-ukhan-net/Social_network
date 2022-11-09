using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social_network.ViewModels
{
    public class UserAddFriendsViewModel
    {
        public int Id { get; set; }
        public int FriendNewId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AvatarURL { get; set; }
        public List<AddFriendsViewModel> AddFriendForModel { get; set; }
    }

    public class AddFriendsViewModel
    {
        public int UserId { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string AvatarURL { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAdded { get; set; }
    }
}
