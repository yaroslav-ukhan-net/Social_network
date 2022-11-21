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
            optionsBuilder.UseSqlServer(_options.Value.DefaultConnectionString);
            optionsBuilder.UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Friend>().HasKey(x => new { x.Friend_oneId, x.Friend_twoId });

            builder.Entity<Friend>().HasKey(f => new { f.Friend_oneId, f.Friend_twoId });

            builder.Entity<Friend>()
              .HasOne(f => f.Friend_one)
              .WithMany(mu => mu.Friend_ones)
              .HasForeignKey(f => f.Friend_oneId);

            builder.Entity<Friend>()
                .HasOne(f => f.Friend_two)
                .WithMany(mu => mu.Friend_twos)
                .HasForeignKey(f => f.Friend_twoId);

            builder.Entity<UserGroup>()
                .HasKey(bc => new { bc.UserId, bc.GroupId });
            builder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGroup)
                .HasForeignKey(ug => ug.UserId);
            builder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.UserGroup)
                .HasForeignKey(ug => ug.GroupId);


        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<User> Moderators { get; set; }
        
    }
}
