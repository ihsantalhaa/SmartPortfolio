using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartPortfolio.Models;

namespace SmartPortfolio.Data
{
    public class ApplicationDbContext : IdentityDbContext<IUser, IRole, int>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<IUser> Users { get; set; }
        public DbSet<IRole> Roles { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Order> Orders { get; set; }

    }
}
