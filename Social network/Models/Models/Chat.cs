using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public bool IsGroup { get; set; }
        public string Name { get; set; }
        public int AdminId { get; set; }
        public string AvatarURL { get; set; }
        public virtual List<Message> Messages { get; set; } = new List<Message>();
        public virtual List<UserChat> UserChat { get; set; } = new();
    }
}
