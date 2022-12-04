using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Group
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string AvatarURL { get; set; }
        public string Notes { get; set; }
        public bool IsClose { get; set; }
        public virtual List<Post> Posts { get; set; } 
        public virtual List<UserGroup> UserGroup { get; set; }
        public int CountFollowers 
        {
            get {return UserGroup.Where(w => w.GroupId == Id && w.ConsistInGroup).Count(); }
        }
    }
}
