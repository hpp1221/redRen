using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Model.Entities;

namespace Web.Model.Context
{
    public class MyContext:DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) 
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Message> Message { get; set; }
        public DbSet<Reply> Reply { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Topic> Topic { get; set; }
        public DbSet<TopicCollect> TopicCollect { get; set; }
        public DbSet<User> User { get; set; }
    }
}
