using System.Text.Json;
using System.Text.Json.Serialization;
using AS2023Env.Data;
using AS2023Env.Models;

namespace AS2023Env;

public static class Constants
{
    public static int InitialEmployeePerPositionMin { get; private set; }
    public static int InitialEmployeePerPositionMax { get; private set; }
    public static int MaximumActiveStaffUnits { get; private set; }
    public static TimeSpan FireEmployeeDelay { get; private set; }
    public static TimeSpan PendingStaffUnitDelay { get; private set; }

    public static readonly Guid MockGuid = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public const string MockPosition = "teamlead";

    private static bool? _adminMode = null;

    public static bool IsAdmin {
        get
        {
            _adminMode ??= Environment.GetEnvironmentVariable("AS23_ADMIN_MODE") == "1";
            return _adminMode.Value || true;
        }
    }    

    public static void Load()
    {
        string dataRaw = Environment.GetEnvironmentVariable("AS23_CONFIG") ??
                      throw new Exception("No AS23_CONFIG env variable set");

        var data = JsonSerializer.Deserialize<ConfigData>(dataRaw);

        InitialEmployeePerPositionMin = data.InitialEmployeePerPositionMin;
        InitialEmployeePerPositionMax = data.InitialEmployeePerPositionMax;
        MaximumActiveStaffUnits = data.MaximumActiveStaffUnits;
        FireEmployeeDelay = TimeSpan.FromSeconds(data.FireEmployeeDelay);
        PendingStaffUnitDelay = TimeSpan.FromSeconds(data.PendingStaffUnitDelay);
    }

    public static async Task<(Employee employee, StaffUnit staffUnit)> FireEmployee(IStorage<StaffUnit> staffUnitStorage, IStorage<Employee> employeeStorage)
    {
        List<StaffUnit> activeStaffUnits = await staffUnitStorage.GetList(u => u.Status == StaffUnitStatus.Opened);
        if (activeStaffUnits.Count >= MaximumActiveStaffUnits)
        {
            return (null, null);
        }

        List<Employee> employees = await employeeStorage.GetList();
        if (employees.Count == 0)
        {
            return (null, null);
        }

        Employee pickToFire = null;
        if (IsAdmin)
        {
            List<Employee> mockEmployee = await employeeStorage.GetList((e) => e.Id == MockGuid.ToString());
            if (mockEmployee.Count >= 1)
            {
                pickToFire = mockEmployee.First();
            }
        }

        if (pickToFire == null)
        {
            var rng = new Random();
            int index = rng.Next(0, employees.Count);

            pickToFire = employees[index];
        }

        StaffUnit staffUnit = (await staffUnitStorage.GetList(u => u.EmployeeId == pickToFire.Id)).FirstOrDefault();
        if (staffUnit?.Status == StaffUnitStatus.Pending)
        {
            return (null, null);
        }
        if (staffUnit != null)
        {
            staffUnit.SetEmployee(null);
            await staffUnitStorage.Update(staffUnit);
        }
        await employeeStorage.Delete(pickToFire.Id);
        return (pickToFire, staffUnit);
    }
}

public class ConfigData
{
    [JsonPropertyName("empPerPosMin")]
    public int InitialEmployeePerPositionMin { get; set; }
    [JsonPropertyName("empPerPosMax")]
    public int InitialEmployeePerPositionMax { get; set; }
    [JsonPropertyName("activeUnitsMax")]
    public int MaximumActiveStaffUnits { get; set; }
    [JsonPropertyName("fireDelaySec")]
    public int FireEmployeeDelay { get; set; }
    [JsonPropertyName("pendingDelaySec")]
    public int PendingStaffUnitDelay { get; set; }
}