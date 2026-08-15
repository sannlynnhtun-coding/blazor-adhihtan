using System.Text.Json.Serialization;

namespace Adhihtan.Models;

public sealed class RecoveredContent
{
    public int SchemaVersion { get; set; }
    public SourceMetadata Source { get; set; } = new();
    public List<CategoryDefinition> Categories { get; set; } = [];
    public Dictionary<string, List<ScheduleLevel>> Schedules { get; set; } = [];
    public Dictionary<string, string> LocalizedSpellNames { get; set; } = [];
}

public sealed class SourceMetadata
{
    public string PackageName { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
    public int HermesBytecodeVersion { get; set; }
}

public sealed class CategoryDefinition
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
    public bool IsActive { get; set; }
    public string Duration { get; set; } = string.Empty;
    public List<LevelOption>? Level { get; set; }
    public string Format { get; set; } = string.Empty;
    public string HowToPray { get; set; } = string.Empty;
    public string Benefit { get; set; } = string.Empty;
    public DetailTabs? DetailTabs { get; set; }
}

public sealed class LevelOption
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
}

public sealed class DetailTabs
{
    public List<InformationItem> Benefits { get; set; } = [];
    public List<InformationItem> Instructions { get; set; } = [];
}

public sealed class InformationItem
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Caution { get; set; }
}

public sealed class ScheduleLevel
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
    public List<SpellEntry> Datasources { get; set; } = [];
}

public sealed class SpellEntry
{
    [JsonPropertyName("no")]
    public int Number { get; set; }

    public string EnDay { get; set; } = string.Empty;
    public string MmDay { get; set; } = string.Empty;
    public string SpellName { get; set; } = string.Empty;
    public int SpellId { get; set; }
    public string SpellCount { get; set; } = string.Empty;
    public int SpellEngCount { get; set; }

    [JsonPropertyName("isVagitable")]
    public bool IsVegetarian { get; set; }

    public string? Date { get; set; }
    public bool IsDone { get; set; }
}
