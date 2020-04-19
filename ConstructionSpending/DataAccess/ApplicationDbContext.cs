using ConstructionSpending.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructionSpending.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Response> Responses { get; set; }

        public DbSet<ResponseVip> ResponseVips { get; set; }
        public DbSet<Time> Times { get; set; }
        public DbSet<Spending> Spendings { get; set; }
        public DbSet<Occupancy> Occupancies { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
