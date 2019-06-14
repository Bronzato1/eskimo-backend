using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        { }

        public DbSet<PostItem> PostItems { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
 