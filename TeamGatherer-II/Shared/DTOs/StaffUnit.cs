namespace TeamGatherer.Shared.DTOs;

public record StaffUnit
{
    [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Id { get; set; }

    [Newtonsoft.Json.JsonProperty("positionId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string PositionId { get; set; }

    [Newtonsoft.Json.JsonProperty("employeeId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string EmployeeId { get; set; }

    [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Status { get; set; }

    [Newtonsoft.Json.JsonProperty("closeTime", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.DateTimeOffset? CloseTime { get; set; }
}

public enum StaffUnitStatus
{
    Opened,
    Closed, 
    Pending
}