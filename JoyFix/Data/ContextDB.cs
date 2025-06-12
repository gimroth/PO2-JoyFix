using Microsoft.EntityFrameworkCore;

namespace JoyFix.Data
{
    public class ContextDB : DbContext
    {
        public ContextDB(DbContextOptions<ContextDB> options) : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<RepairRequest> RepairRequests { get; set; }
        public DbSet<Repair> Repairs { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<TechnicianSpecialization> TechnicianSpecializations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Devices)
                .WithOne(d => d.Customer)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.RepairRequests)
                .WithOne(rr => rr.Customer)
                .HasForeignKey(rr => rr.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Device>()
                .HasMany(d => d.RepairRequests)
                .WithOne(rr => rr.Device)
                .HasForeignKey(rr => rr.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RepairRequest>()
                .HasOne(rr => rr.Repair)
                .WithOne(r => r.RepairRequest)
                .HasForeignKey<Repair>(r => r.RepairRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Repair>()
                .HasOne(r => r.Technician)
                .WithMany(t => t.Repairs)
                .HasForeignKey(r => r.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TechnicianSpecialization>()
                .HasKey(ts => new { ts.TechnicianId, ts.SpecializationId });

            modelBuilder.Entity<TechnicianSpecialization>()
                .HasOne(ts => ts.Technician)
                .WithMany(t => t.Specializations)
                .HasForeignKey(ts => ts.TechnicianId);

            modelBuilder.Entity<TechnicianSpecialization>()
                .HasOne(ts => ts.Specialization)
                .WithMany(s => s.Technicians)
                .HasForeignKey(ts => ts.SpecializationId);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Device>()
                .HasIndex(d => d.SerialNumber)
                .IsUnique();

            modelBuilder.Entity<RepairRequest>()
                .HasIndex(rr => rr.Status);

            modelBuilder.Entity<Technician>()
                .HasIndex(t => t.Email)
                .IsUnique();

        }
    }
}