using Microsoft.EntityFrameworkCore;
using DBPractice.Models;

namespace DBPractice.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        // Define DbSets (Tables)
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
    }
}