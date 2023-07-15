using System.Text.Json.Serialization;

namespace AS2023Env.Models;

public class StaffUnit
{
    public string Id { get; set; }
    public string PositionId { get; set; }
    public string EmployeeId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StaffUnitStatus Status { get; set; }
    public DateTime? CloseTime { get; set; } = null;

    public void SetEmployee(string employeeId)
    {
        if (employeeId == null)
        {
            EmployeeId = null;
            Status = StaffUnitStatus.Opened;
            CloseTime = null;
        }
        else
        {
            EmployeeId = employeeId;
            Status = StaffUnitStatus.Pending;
            CloseTime = DateTime.Now + Constants.PendingStaffUnitDelay;
        }
    }

    public void SetClosed()
    {
        Status = StaffUnitStatus.Closed;
    }
}