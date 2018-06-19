using Microsoft.EntityFrameworkCore;
 
namespace TheWall.Models
{
    public class TheWallContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public TheWallContext(DbContextOptions<TheWallContext> options) : base(options) { }
        
        public DbSet<Users> users { get; set; }
        public DbSet<Posts> posts { get; set; }
        public DbSet<Comments> comments { get; set; }
    }
}
