namespace Adhihtan.Models;

public sealed class PersistedAppState
{
    public int SchemaVersion { get; set; } = 1;
    public AppSettings Settings { get; set; } = new();
    public ActivePlan? Plan { get; set; }
    public Dictionary<string, int> CountingProgress { get; set; } = [];
    public List<CountHistoryEntry> CountHistory { get; set; } = [];
}

public sealed class AppSettings
{
    public string Audio { get; set; } = "none";
    public bool Alarm { get; set; }
    public bool IsDarkMode { get; set; }
    public string CounterStyle { get; set; } = "default";
    public string FontEncoding { get; set; } = "unicode";
    public string BackgroundStyle { get; set; } = "rain";
    public bool KeepAwake { get; set; } = true;
    public bool ConfirmReset { get; set; } = true;
    public bool HighContrast { get; set; }
}

public sealed class ActivePlan
{
    public int Category { get; set; } = 1;
    public int Level { get; set; } = 1;
    public int SpellId { get; set; } = 1;
    public int Multiplier { get; set; } = 108;
    public int CustomRoundCount { get; set; } = 1;
    public List<ScheduleLevel> Datasource { get; set; } = [];
}

public sealed class CountHistoryEntry
{
    public string Date { get; set; } = string.Empty;
    public string SpellName { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Target { get; set; }
    public DateTimeOffset CompletedAt { get; set; }
}

public sealed class PlanSetupRequest
{
    public int Category { get; set; } = 1;
    public int Level { get; set; } = 1;
    public int SpellId { get; set; } = 1;
    public int Multiplier { get; set; } = 108;
    public int CustomRoundCount { get; set; } = 1;
    public int CustomTouchCount { get; set; } = 1;
}

public sealed class BackupEnvelope
{
    public int Version { get; set; } = 1;
    public DateTimeOffset ExportedAt { get; set; } = DateTimeOffset.UtcNow;
    public PersistedAppState State { get; set; } = new();
}

public sealed record SelectOption<T>(string Label, T Value, string? Icon = null);
