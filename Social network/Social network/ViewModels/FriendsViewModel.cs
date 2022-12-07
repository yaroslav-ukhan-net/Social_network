using Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social_network.ViewModels
{
    public class FriendsViewModel
    {
        public int Id { get; set; }
        public int FriendNewId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AvatarURL { get; set; }
        public  List<FriendsListViewModel> FriendsListForModel { get; set; }
    }

    public class FriendsListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AvatarURL { get; set; }
    }
}
