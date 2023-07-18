using RestApiClient;

namespace TeamGatherer.Shared.DTOs;

public record StaffUnitDto
{
    public string Id { get; set; }
    public Position Position { get; set; }
    public string Status { get; set; }
}