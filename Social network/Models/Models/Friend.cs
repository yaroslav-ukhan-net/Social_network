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
        public virtual User Friend_one { get; set; }
        public  int Friend_oneId { get; set; }
        public virtual User Friend_two { get; set; }
        public  int Friend_twoId { get; set; }
        public int Status { get; set; }
    }
}
