using Microsoft.EntityFrameworkCore;
using SystemInfoApi.Models;

namespace SystemInfoApi.Data {
    public class SystemInfoContext : DbContext {
        public SystemInfoContext(DbContextOptions<SystemInfoContext> options)
            : base(options) {
        }

        public DbSet<MachineModel> Machines { get; set; }
        public DbSet<DriveModel> Drives { get; set; }
        public DbSet<OsModel> Os { get; set; }
    }
}
