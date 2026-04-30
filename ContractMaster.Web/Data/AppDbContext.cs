using ContractMaster.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ContractMaster.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Client Configuration
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.ClientId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ContactDetails).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Region).IsRequired().HasMaxLength(50);

                // Seed data
                entity.HasData(
                    new Client { ClientId = 1, Name = "ABC Corporation", ContactDetails = "admin@abccorp.com", Region = "Gauteng" },
                    new Client { ClientId = 2, Name = "XYZ Enterprises", ContactDetails = "info@xyz.co.za", Region = "Western Cape" }
                );
            });

            // Contract Configuration
            modelBuilder.Entity<Contract>(entity =>
            {
                entity.HasKey(e => e.ContractId);
                entity.Property(e => e.ServiceLevel).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SignedAgreementPath).HasMaxLength(500);

                entity.HasOne(c => c.Client)
                    .WithMany(cl => cl.Contracts)
                    .HasForeignKey(c => c.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Seed data
                entity.HasData(
                    new Contract
                    {
                        ContractId = 1,
                        ClientId = 1,
                        StartDate = new DateTime(2025, 1, 1),
                        EndDate = new DateTime(2025, 12, 31),
                        Status = ContractStatus.Active,
                        ServiceLevel = "Premium"
                    }
                );
            });

            // ServiceRequest Configuration
            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.HasKey(e => e.ServiceRequestId);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CostUSD).HasPrecision(18, 2);
                entity.Property(e => e.CostZAR).HasPrecision(18, 2);

                entity.HasOne(sr => sr.Contract)
                    .WithMany(c => c.ServiceRequests)
                    .HasForeignKey(sr => sr.ContractId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}