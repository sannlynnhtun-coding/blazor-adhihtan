using System.Net.Http.Json;
using System.Text.Json;
using Adhihtan.Models;

namespace Adhihtan.Services;

public sealed class AppStateService(HttpClient httpClient, BrowserStorage storage)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private bool _initialized;

    public event Action? Changed;

    public RecoveredContent Content { get; private set; } = new();
    public PersistedAppState State { get; private set; } = new();
    public bool IsInitialized => _initialized;

    public static IReadOnlyList<SelectOption<int>> Multipliers { get; } =
    [
        new("၁၀၈ လုံး", 108), new("၈၁ လုံး", 81), new("၅၄ လုံး", 54),
        new("၂၇ လုံး", 27), new("၁၀ လုံး", 10), new("၉ လုံး", 9)
    ];

    public static IReadOnlyList<SelectOption<int>> CustomSpells { get; } =
    [
        new("အရဟံ", 1), new("သတ္ထာဒေဝမနုဿာနံ", 2), new("ဗုဒ္ဓေါ", 3),
        new("ဘဂဝါ", 4), new("သမ္မာသမ္ဗုဒ္ဓေါ", 5), new("သုဂတော", 6),
        new("လောကဝိဒူ", 7), new("အနုတ္တရောပုရိသဓမ္မသာရထိ", 8),
        new("ဝိဇ္ဇာစရဏသမ္ပန္နော", 9)
    ];

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        Content = await httpClient.GetFromJsonAsync<RecoveredContent>("data/recovered-content.json", JsonOptions)
            ?? throw new InvalidOperationException("Recovered application content couldn't be loaded.");

        State = await storage.LoadAsync() ?? new PersistedAppState();
        NormalizeState();
        EnsureCustomTodayEntry();
        await storage.SetFontEncodingAsync(State.Settings.FontEncoding);
        _initialized = true;
        await storage.SaveAsync(State);
        Changed?.Invoke();
        await storage.RegisterPwaAsync();
    }

    public CategoryDefinition? GetCategory(int categoryId) =>
        Content.Categories.FirstOrDefault(category => category.Value == categoryId);

    public IReadOnlyList<ScheduleLevel> GetSchedule(int categoryId) =>
        Content.Schedules.TryGetValue(categoryId.ToString(), out var levels) ? levels : [];

    public IReadOnlyList<SelectOption<int>> GetSpellOptions(int categoryId, int level)
    {
        if (categoryId == 5)
        {
            return CustomSpells;
        }

        var entries = GetSchedule(categoryId)
            .FirstOrDefault(item => item.Value == level)?.Datasources ?? [];

        return entries
            .GroupBy(entry => entry.SpellId)
            .Select(group => group.First())
            .Select(entry => new SelectOption<int>(entry.SpellName, entry.SpellId))
            .ToList();
    }

    public SpellEntry? GetTodayEntry()
    {
        EnsureCustomTodayEntry();
        var today = TodayKey;
        return State.Plan?.Datasource
            .SelectMany(level => level.Datasources)
            .FirstOrDefault(entry => entry.Date == today);
    }

    public int TodayCount => State.CountingProgress.GetValueOrDefault(TodayKey);
    public int TodayTarget => GetTodayEntry() is { } entry && State.Plan is { } plan
        ? entry.SpellEngCount * plan.Multiplier
        : 0;

    public double TodayProgress => TodayTarget == 0
        ? 0
        : Math.Clamp((double)TodayCount / TodayTarget, 0, 1);

    public async Task ConfigurePlanAsync(PlanSetupRequest request)
    {
        ActivePlan plan;
        if (request.Category == 5)
        {
            var spell = CustomSpells.FirstOrDefault(option => option.Value == request.SpellId) ?? CustomSpells[0];
            plan = new ActivePlan
            {
                Category = 5,
                Level = 1,
                SpellId = spell.Value,
                Multiplier = Math.Max(request.CustomTouchCount, 1),
                CustomRoundCount = Math.Max(request.CustomRoundCount, 1),
                Datasource =
                [
                    new ScheduleLevel
                    {
                        Label = "စိတ်ကြိုက်",
                        Value = 1,
                        Datasources = [CreateCustomEntry(spell.Label, spell.Value, request.CustomRoundCount)]
                    }
                ]
            };
        }
        else
        {
            var source = GetSchedule(request.Category);
            var selectedLevel = source.FirstOrDefault(item => item.Value == request.Level) ?? source.First();
            var selectedSpell = selectedLevel.Datasources.FirstOrDefault(item => item.SpellId == request.SpellId)
                ?? selectedLevel.Datasources.First();
            var start = DateTime.Today.AddDays(-(selectedSpell.Number - 1));
            var cloned = Clone(source);

            foreach (var entry in cloned.SelectMany(level => level.Datasources))
            {
                entry.Date = start.AddDays(entry.Number - 1).ToString("yyyy-MM-dd");
                entry.IsDone = entry.Number < selectedSpell.Number;
            }

            plan = new ActivePlan
            {
                Category = request.Category,
                Level = request.Level,
                SpellId = selectedSpell.SpellId,
                Multiplier = Math.Max(request.Multiplier, 1),
                Datasource = cloned
            };
        }

        State.Plan = plan;
        State.CountingProgress[TodayKey] = 0;
        State.CountHistory.RemoveAll(entry => entry.Date == TodayKey);
        await SaveAndNotifyAsync();
    }

    public async Task IncrementAsync()
    {
        var entry = GetTodayEntry();
        var target = TodayTarget;
        if (entry is null || entry.IsDone || target <= 0 || TodayCount >= target)
        {
            return;
        }

        var next = TodayCount + 1;
        State.CountingProgress[TodayKey] = next;
        var completed = next >= target;
        entry.IsDone = completed;
        UpdateHistory(entry, next, target, completed);
        await SaveAndNotifyAsync();
        await storage.FeedbackAsync(State.Settings.Audio, completed, State.Settings.Alarm);
    }

    public async Task UndoAsync()
    {
        if (TodayCount <= 0 || GetTodayEntry() is not { } entry)
        {
            return;
        }

        State.CountingProgress[TodayKey] = TodayCount - 1;
        entry.IsDone = false;
        State.CountHistory.RemoveAll(item => item.Date == TodayKey);
        await SaveAndNotifyAsync();
    }

    public async Task ResetTodayAsync()
    {
        State.CountingProgress[TodayKey] = 0;
        if (GetTodayEntry() is { } entry)
        {
            entry.IsDone = false;
        }

        State.CountHistory.RemoveAll(item => item.Date == TodayKey);
        await SaveAndNotifyAsync();
    }

    public async Task UpdateSettingsAsync(Action<AppSettings> update)
    {
        update(State.Settings);
        NormalizeState();
        await SaveAndNotifyAsync();
        await storage.SetFontEncodingAsync(State.Settings.FontEncoding);
        await storage.SetWakeLockAsync(State.Settings.KeepAwake && State.Plan is not null);
    }

    public async Task ExportBackupAsync() =>
        await storage.DownloadBackupAsync(new BackupEnvelope { State = State });

    public async Task<(bool Success, string Message)> RestoreBackupAsync(string json)
    {
        try
        {
            var backup = JsonSerializer.Deserialize<BackupEnvelope>(json, JsonOptions);
            if (backup?.Version != 1 || backup.State is null)
            {
                return (false, "ဒေတာမိတ္တူဖိုင်ပုံစံ မမှန်ပါ။ မှန်ကန်သော အဓိဋ္ဌာန် ဒေတာမိတ္တူဖိုင်ကို ရွေးပါ။");
            }

            State = backup.State;
            NormalizeState();
            EnsureCustomTodayEntry();
            await SaveAndNotifyAsync();
            return (true, "ဒေတာမိတ္တူကို ပြန်လည်ထည့်သွင်းပြီးပါပြီ။");
        }
        catch (JsonException)
        {
            return (false, "ဒေတာမိတ္တူ JSON ဖိုင်ကို ဖတ်၍မရပါ။ ဖိုင်မှန်ကန်ကြောင်း စစ်ပြီး ထပ်မံရွေးပါ။");
        }
    }

    public async Task ShareTodayAsync()
    {
        var entry = GetTodayEntry();
        if (entry is null)
        {
            return;
        }

        var category = GetCategory(State.Plan?.Category ?? 0)?.Label ?? "အဓိဋ္ဌာန်";
        var text = $"{category}\n{entry.SpellName}\n{ToMyanmarNumber(TodayCount)} / {ToMyanmarNumber(TodayTarget)} ပတ်";
        await storage.ShareAsync("အဓိဋ္ဌာန်", text);
    }

    public static string ToMyanmarNumber(int value) => ToMyanmarNumber(value.ToString());

    public static string ToMyanmarNumber(string value)
    {
        const string latin = "0123456789";
        const string myanmar = "၀၁၂၃၄၅၆၇၈၉";
        return new string(value.Select(character =>
        {
            var index = latin.IndexOf(character);
            return index >= 0 ? myanmar[index] : character;
        }).ToArray());
    }

    private static string TodayKey => DateTime.Today.ToString("yyyy-MM-dd");

    private static List<ScheduleLevel> Clone(IReadOnlyList<ScheduleLevel> source) =>
        JsonSerializer.Deserialize<List<ScheduleLevel>>(JsonSerializer.Serialize(source, JsonOptions), JsonOptions) ?? [];

    private static SpellEntry CreateCustomEntry(string spellName, int spellId, int rounds) => new()
    {
        Number = 1,
        EnDay = DateTime.Today.DayOfWeek.ToString().ToLowerInvariant(),
        MmDay = "ယနေ့",
        SpellName = spellName,
        SpellId = spellId,
        SpellCount = ToMyanmarNumber(Math.Max(rounds, 1)),
        SpellEngCount = Math.Max(rounds, 1),
        Date = TodayKey
    };

    private void EnsureCustomTodayEntry()
    {
        if (State.Plan is not { Category: 5 } plan || plan.Datasource.Count == 0 || GetEntryWithoutEnsuring() is not null)
        {
            return;
        }

        var spell = CustomSpells.FirstOrDefault(option => option.Value == plan.SpellId) ?? CustomSpells[0];
        plan.Datasource[0].Datasources.Add(CreateCustomEntry(spell.Label, spell.Value, plan.CustomRoundCount));
        State.CountingProgress.TryAdd(TodayKey, 0);
    }

    private SpellEntry? GetEntryWithoutEnsuring() => State.Plan?.Datasource
        .SelectMany(level => level.Datasources)
        .FirstOrDefault(entry => entry.Date == TodayKey);

    private void UpdateHistory(SpellEntry entry, int count, int target, bool completed)
    {
        State.CountHistory.RemoveAll(item => item.Date == TodayKey);
        if (completed)
        {
            State.CountHistory.Add(new CountHistoryEntry
            {
                Date = TodayKey,
                SpellName = entry.SpellName,
                Count = count,
                Target = target,
                CompletedAt = DateTimeOffset.UtcNow
            });
        }
    }

    private void NormalizeState()
    {
        State.Settings ??= new AppSettings();
        State.CountingProgress ??= [];
        State.CountHistory ??= [];
        State.Settings.Audio = State.Settings.Audio is "none" or "sound" or "vibrate" or "sound_vibrate"
            ? State.Settings.Audio
            : "none";
        State.Settings.CounterStyle = string.IsNullOrWhiteSpace(State.Settings.CounterStyle) ? "default" : State.Settings.CounterStyle;
        State.Settings.BackgroundStyle = string.IsNullOrWhiteSpace(State.Settings.BackgroundStyle) ? "rain" : State.Settings.BackgroundStyle;
    }

    private async Task SaveAndNotifyAsync()
    {
        await storage.SaveAsync(State);
        Changed?.Invoke();
    }
}
