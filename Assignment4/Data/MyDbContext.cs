using Assignment4.Models;

using System.Data.Entity;

namespace Assignment4.Data
{
    public class MyDbContext: DbContext
    {
        public MyDbContext() : base("Article")
        {
        }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}