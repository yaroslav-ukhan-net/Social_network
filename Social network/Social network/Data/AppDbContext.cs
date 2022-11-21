using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Models;
using System.ComponentModel.DataAnnotations;

namespace Social_network.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
    }
    public class AppUser : IdentityUser
    {
        public AppUser(string userName) : base(userName)
        {
        }

        public int AppUserId { get; set; }
    }
}
