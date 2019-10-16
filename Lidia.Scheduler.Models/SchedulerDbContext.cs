using Lidia.Framework.SaaS.Models.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidia.Scheduler.Models
{
    public class SchedulerDbContext : DbContext
    {
        public SchedulerDbContext() : base("name=MAIN") { }

        public static SchedulerDbContext Create()
        {
            return new SchedulerDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region [ Identity ]

            modelBuilder.Entity<LidiaUserLogin>().Map(c =>
            {
                c.ToTable("UserLogins");
                c.Properties(p => new
                {
                    p.UserId,
                    p.LoginProvider,
                    p.ProviderKey
                });
            }).HasKey(p => new { p.LoginProvider, p.ProviderKey, p.UserId });

            // Mapping for ApiRole
            modelBuilder.Entity<LidiaRole>().Map(c =>
            {
                c.ToTable("Roles");
                c.Property(p => p.Id).HasColumnName("RoleId");
                c.Properties(p => new
                {
                    p.Name
                });
            }).HasKey(p => p.Id);
            modelBuilder.Entity<LidiaRole>().HasMany(c => c.Users).WithRequired().HasForeignKey(c => c.RoleId);

            modelBuilder.Entity<LidiaUser>().Map(c =>
            {
                c.ToTable("Users");
                c.Property(p => p.Id).HasColumnName("UserId");
                c.Properties(p => new
                {
                    p.AccessFailedCount,
                    p.Email,
                    p.EmailConfirmed,
                    p.PasswordHash,
                    p.PhoneNumber,
                    p.PhoneNumberConfirmed,
                    p.MobileNumber,
                    p.MobileNumberConfirmed,
                    p.TwoFactorEnabled,
                    p.SecurityStamp,
                    p.LockoutEnabled,
                    p.LockoutEndDateUtc,
                    p.UserName,
                    p.Firstname,
                    p.Lastname,
                    p.Gender,
                    p.Birthdate
                });
            }).HasKey(c => c.Id);
            modelBuilder.Entity<LidiaUser>().HasMany(c => c.Logins).WithOptional().HasForeignKey(c => c.UserId);
            modelBuilder.Entity<LidiaUser>().HasMany(c => c.Claims).WithOptional().HasForeignKey(c => c.UserId);
            modelBuilder.Entity<LidiaUser>().HasMany(c => c.Roles).WithRequired().HasForeignKey(c => c.UserId);

            modelBuilder.Entity<LidiaUserRole>().Map(c =>
            {
                c.ToTable("UserRoles");
                c.Properties(p => new
                {
                    p.UserId,
                    p.RoleId,
                    p.TenantId
                });
            })
            .HasKey(c => new { c.UserId, c.RoleId });

            modelBuilder.Entity<LidiaUserClaim>().Map(c =>
            {
                c.ToTable("UserClaims");
                c.Property(p => p.Id).HasColumnName("UserClaimId");
                c.Properties(p => new
                {
                    p.UserId,
                    p.ClaimValue,
                    p.ClaimType
                });
            }).HasKey(c => c.Id);

            #endregion
        }

        public DbSet<JobCollection> Collections { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<JobLogEntry> JobLogEntries { get; set; }
        

    }
}
