using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace WebApp.Dal
{
    /// <summary>
    /// A simple DBContextProvider that creates a DB connection on top of Sqlite
    /// </summary>
    public class SqliteTenantDbContextProvider : ITenantDbContextProvider
    {
        private DbContextOptions<TenantDbContext> _options;
        private readonly object _syncRoot = new object();

        async Task<TenantDbContext> ITenantDbContextProvider.Get()
        {
            if (null == _options)
            {
                lock (_syncRoot)
                {
                    if (null == _options)
                    {
                        var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
                        var connectionString = connectionStringBuilder.ToString();
                        var connection = new SqliteConnection(connectionString);
                        var builder = new DbContextOptionsBuilder<TenantDbContext>();
                        builder.UseSqlite(connection);
                        _options = builder.Options;
                    }
                }
            }

            var ctx = new TenantDbContext(_options);
            await ctx.Database.OpenConnectionAsync();
            await ctx.Database.EnsureCreatedAsync();
            return ctx;
        }
    }
}
