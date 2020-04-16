using ConstructionSpending.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructionSpending.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Response> Responses { get; set; }

        public DbSet<ResponseVip> ResponseVips { get; set; }
    }
}
