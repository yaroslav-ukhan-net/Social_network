using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_SocialNetwork.EF
{
    public class SocialNetworkContext : DbContext
    {
        private readonly IOptions<RepositoryOptions> _options;
        public SocialNetworkContext(IOptions<RepositoryOptions> options)
        {
            _options = options;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_options.Value.DefaultConnectionString, b=> b.MigrationsAssembly("SocialNetwork"));
            optionsBuilder.UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Friend>().HasKey(x => new { x.FirstFriendId, x.SecondFriendId });
            builder.Entity<Friend>()
              .HasOne(f => f.FirstFriend)
              .WithMany(mu => mu.FirstFriends)
              .HasForeignKey(f => f.FirstFriendId);
            builder.Entity<Friend>()
                .HasOne(f => f.SecondFriend)
                .WithMany(mu => mu.SecondFriends)
                .HasForeignKey(f => f.SecondFriendId);


            builder.Entity<UserGroup>().HasKey(bc => new { bc.UserId, bc.GroupId });
            builder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGroup)
                .HasForeignKey(ug => ug.UserId);
            builder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.UserGroup)
                .HasForeignKey(ug => ug.GroupId);


            builder.Entity<UserChat>().HasKey(bc => new { bc.UserId, bc.ChatId });
            builder.Entity<UserChat>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserChat)
                .HasForeignKey(ug => ug.UserId);
            builder.Entity<UserChat>()
                .HasOne(ug => ug.Chat)
                .WithMany(g => g.UserChat)
                .HasForeignKey(ug => ug.ChatId);

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
