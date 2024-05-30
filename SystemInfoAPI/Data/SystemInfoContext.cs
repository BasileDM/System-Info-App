using Microsoft.EntityFrameworkCore;
using SystemInfoApi.Models;

namespace SystemInfoApi.Data {
    public class SystemInfoContext : DbContext {
        public SystemInfoContext(DbContextOptions<SystemInfoContext> options)
            : base(options) {
        }

        public DbSet<MachineModel> Client { get; set; }
        public DbSet<MachineModel> Client_Machine { get; set; }
        public DbSet<DriveModel> Client_Machine_Disque { get; set; }
        public DbSet<OsModel> Client_Machine_Disque_Os { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<ClientModel>().HasKey(c => c.Id);
            modelBuilder.Entity<MachineModel>().HasKey(m => m.Id);
            modelBuilder.Entity<DriveModel>().HasKey(d => d.Id);
            modelBuilder.Entity<OsModel>().HasKey(o => o.Id);

            modelBuilder.Entity<MachineModel>()
                    .HasMany(m => m.Drives)
                    .WithOne()
                    .HasForeignKey(d => d.Id);

            modelBuilder.Entity<DriveModel>()
                    .HasOne(d => d.Os)
                    .WithOne()
                    .HasForeignKey<OsModel>(o => o.Id);
        }
    }
}
