using Microsoft.EntityFrameworkCore;

namespace JoyFix.Data
{
    public class DynamicDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public DynamicDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ContextDB CreateDbContext(string? overrideConnectionString = null)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ContextDB>();

            var connectionString = overrideConnectionString
                ?? _configuration.GetConnectionString(DatabaseService.CurrentConnection)
                ?? _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Brak dostępnego connection stringa.");
            }

            optionsBuilder.UseNpgsql(connectionString);
            return new ContextDB(optionsBuilder.Options);
        }
    }
}
