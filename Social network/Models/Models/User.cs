
using Microsoft.AspNetCore.Identity;
using Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class User 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AvatarURL { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public   string  PhoneNumber { get; set; }
        public string Notes { get; set; }
        public virtual List<Post> Posts { get; set; }
        public virtual List<Friend> Friend_ones { get; set; }
        public virtual List<Friend> Friend_twos { get; set; }
        public virtual List<UserGroup> UserGroup { get; set; }
    }
}
