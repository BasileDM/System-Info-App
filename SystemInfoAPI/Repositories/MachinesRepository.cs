using SystemInfoApi.Classes;

namespace SystemInfoApi.Repositories
{
    public class MachinesRepository(IConfiguration configuration) : Database(configuration)
    {
    }
}
