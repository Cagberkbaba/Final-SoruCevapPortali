using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace SoruCevapPortali.Models
{
    public class AppDbContext : IdentityDbContext<AppUser,AppRole,string>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Todo> Todos { get; set; }

        public DbSet<Category> Categories { get; set; } 
    }
}
