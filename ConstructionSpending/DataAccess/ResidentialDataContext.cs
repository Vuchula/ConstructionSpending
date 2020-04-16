using ConstructionSpending.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace ConstructionSpending.DataAccess
{
    public class ResidentialDataContext : DbContext
    {
        public ResidentialDataContext(DbContextOptions<ResidentialDataContext> options) : base(options) { }

        public DbSet<Time> Times { get; set; }
        public DbSet<Spending> Spendings { get; set; }
        public DbSet<Occupancy> Occupancies { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Response> Responses { get; set; }

    }
}
