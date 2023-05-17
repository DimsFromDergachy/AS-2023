using System.Text.Json;
using System.Text.Json.Serialization;

namespace AS2023Env;

public static class Constants
{
    public static int InitialEmployeePerPositionMin { get; private set; }
    public static int InitialEmployeePerPositionMax { get; private set; }
    public static int MaximumActiveStaffUnits { get; private set; }
    public static TimeSpan FireEmployeeDelay { get; private set; }
    public static TimeSpan PendingStaffUnitDelay { get; private set; }

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