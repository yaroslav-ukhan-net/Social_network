using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Friend
    {
        public virtual User FirstFriend { get; set; }
        public  int FirstFriendId { get; set; }
        public virtual User SecondFriend { get; set; }
        public  int SecondFriendId { get; set; }
        public int Status { get; set; }
    }
    public enum StatusFriendship
    {
        notFriends,
        requestToFriendship,
        areFriends
    }
}
