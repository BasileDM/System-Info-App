using Microsoft.EntityFrameworkCore;
using SystemInfoApi.Models;

namespace SystemInfoApi.Data {
    public class SystemInfoContext : DbContext {
        public SystemInfoContext(DbContextOptions<SystemInfoContext> options)
            : base(options) {
        }

        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<MachineModel> Machines { get; set; }
        public DbSet<DriveModel> Drives { get; set; }
        public DbSet<OsModel> OsSystems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<CustomerModel>().HasKey(c => c.Id);
            modelBuilder.Entity<MachineModel>().HasKey(m => m.Id);
            modelBuilder.Entity<DriveModel>().HasKey(d => d.Id);
            modelBuilder.Entity<OsModel>().HasKey(o => o.Id);

            modelBuilder.Entity<CustomerModel>()
                .HasMany(c => c.Machines)
                .WithOne()
                .HasForeignKey(m => m.CustomerId);

            modelBuilder.Entity<MachineModel>()
                .HasMany(m => m.Drives)
                .WithOne()
                .HasForeignKey(d => d.MachineId);

            modelBuilder.Entity<DriveModel>()
                .HasOne(d => d.Os)
                .WithOne()
                .HasForeignKey<OsModel>(o => o.DriveId);
        }
    }
}
