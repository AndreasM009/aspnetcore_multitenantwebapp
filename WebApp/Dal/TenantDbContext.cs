using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Dal
{
    public class TenantDbContext : DbContext
    {
        #region c'tor

        public TenantDbContext()
        {

        }

        public TenantDbContext(DbContextOptions<TenantDbContext> options)
            : base(options)
        {

        }

        #endregion

        #region DbSets

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }

        #endregion

        #region Model

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var tenant = modelBuilder.Entity<Tenant>().ToTable("Tenant");
            tenant.HasKey(t => t.Id);
            tenant.Property(t => t.TenantId);
            tenant.Property(t => t.StateMarker);
            tenant.Property(t => t.Name);
            tenant.Property(t => t.AdminConsented);
            tenant.Property(t => t.Issuer);

            var user = modelBuilder.Entity<User>().ToTable("User");
            user.HasKey(u => u.PrincipalName);
            user.HasIndex(u => u.TenantId);

            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}
