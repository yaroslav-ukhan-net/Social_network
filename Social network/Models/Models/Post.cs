using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? GroupId { get; set; }
        public string Text { get; set; }
        public DateTime PostTime { get; set; }
    }
}
