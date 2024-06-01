using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Entities.EntityFramework.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FileKey> FileKeys { get; set; }
        public DbSet<UserFriend> UserFriends { get; set; }
        public DbSet<SharedFile> SharedFiles { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
    }
}
