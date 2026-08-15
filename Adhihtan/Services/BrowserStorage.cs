using System.Text.Json;
using Adhihtan.Models;
using Microsoft.JSInterop;

namespace Adhihtan.Services;

public sealed class BrowserStorage(IJSRuntime jsRuntime)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public async Task<PersistedAppState?> LoadAsync()
    {
        var json = await jsRuntime.InvokeAsync<string?>("adhihtanApp.loadState");
        return string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<PersistedAppState>(json, JsonOptions);
    }

    public ValueTask SaveAsync(PersistedAppState state) =>
        jsRuntime.InvokeVoidAsync("adhihtanApp.saveState", JsonSerializer.Serialize(state, JsonOptions));

    public ValueTask DownloadBackupAsync(BackupEnvelope backup) =>
        jsRuntime.InvokeVoidAsync(
            "adhihtanApp.downloadJson",
            $"adhihtan-backup-{DateTime.Today:yyyy-MM-dd}.json",
            JsonSerializer.Serialize(backup, JsonOptions));

    public ValueTask FeedbackAsync(string audioMode, bool completed = false, bool alarm = false) =>
        jsRuntime.InvokeVoidAsync("adhihtanApp.feedback", audioMode, completed, alarm);

    public ValueTask ShareAsync(string title, string text) =>
        jsRuntime.InvokeVoidAsync("adhihtanApp.share", title, text);

    public ValueTask SetWakeLockAsync(bool enabled) =>
        jsRuntime.InvokeVoidAsync("adhihtanApp.setWakeLock", enabled);

    public ValueTask RegisterPwaAsync() =>
        jsRuntime.InvokeVoidAsync("adhihtanApp.registerPwa");

    public ValueTask SetFontEncodingAsync(string encoding) =>
        jsRuntime.InvokeVoidAsync("adhihtanApp.setFontEncoding", encoding);
}
