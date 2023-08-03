using TeamGatherer.Shared;

namespace TeamGatherer.Server;

public class ServiceOptions
{
    public required string ConnectionString { get; set; }

    public required StaffConfig StaffConfig { get; set; }
}