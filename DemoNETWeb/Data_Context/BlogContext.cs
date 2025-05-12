using System;
using System.Collections.Generic;
using System.Data.Entity;
using DemoNETWeb.Models;
using System.Linq;
using System.Web;

namespace DemoNETWeb.Data_Context
{
    public class BlogContext : DbContext
    {
        public BlogContext() : base("name=BlogConnection")
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<ContentSection> ContentSections { get; set; }

    }
}