﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social_network.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PostSenderName { get; set; }
        public string PostSenderAvatar { get; set; }
        public string Text { get; set; }
        public DateTime PostTime { get; set; }
    }
}
