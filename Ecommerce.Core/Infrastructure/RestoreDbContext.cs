using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Core.Infrastructure
{
    public class RestoreDbContext : DbContext
    {
        private readonly string _connectionString;

        public RestoreDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        // Your DbSet properties here
    }

}
