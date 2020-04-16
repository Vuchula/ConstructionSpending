using System;
using Microsoft.EntityFrameworkCore;

namespace ConstructionSpending.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
