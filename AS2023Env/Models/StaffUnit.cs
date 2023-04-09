namespace AS2023Env.Models;

public class StaffUnit
{
    public string Id { get; set; }
    public string PositionId { get; set; }
    public string EmployeeId { get; set; }
    public bool Active => string.IsNullOrEmpty(EmployeeId);
}