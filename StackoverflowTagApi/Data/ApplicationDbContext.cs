using Microsoft.EntityFrameworkCore;
using StackoverflowTagApi.Models;

namespace StackoverflowTagApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Tag> Tags { get; set; }
    }
}
