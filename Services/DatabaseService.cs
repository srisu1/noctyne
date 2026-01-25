using SQLite;
using MoodJournal.Models;
using MoodJournal.Services.Interfaces;

namespace MoodJournal.Services;

/// <summary>
/// SQLite database service handling all data persistence.
/// </summary>
public class DatabaseService : IDatabaseService
{
    private SQLiteAsyncConnection? _connection;
    private readonly string _dbPath;
    private bool _initialized = false;
    
    /// <summary>
    /// Initializes a new instance of the DatabaseService.
    /// </summary>
    public DatabaseService()
    {
        // Store database in app data directory
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "moodjournal.db3");
    }
    
    /// <summary>
    /// Gets or creates the database connection.
    /// </summary>
    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection != null)
            return _connection;
        
        _connection = new SQLiteAsyncConnection(
            _dbPath,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache
        );
        
        if (!_initialized)
        {
            await InitializeAsync();
        }
        
        return _connection;
    }
    
    /// <summary>
    /// Initializes database tables and seeds default data.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_initialized)
            return;
        
        var db = _connection ?? new SQLiteAsyncConnection(_dbPath);
        _connection = db;
        
        // Create AppSettings table
        await db.CreateTableAsync<AppSetting>();
        
        // Seed default settings
        await SeedDefaultSettingsAsync(db);
        
        _initialized = true;
    }
    
    /// <summary>
    /// Seeds the default settings if they don't exist.
    /// </summary>
    private async Task SeedDefaultSettingsAsync(SQLiteAsyncConnection db)
    {
        var count = await db.Table<AppSetting>().CountAsync();
        if (count > 0)
            return;
        
        var settings = new List<AppSetting>
        {
            new() { Key = SettingKeys.Theme, Value = "light" },
            new() { Key = SettingKeys.IsFirstLaunch, Value = "true" },
            new() { Key = SettingKeys.OnboardingCompleted, Value = "false" },
            new() { Key = SettingKeys.AvatarCompleted, Value = "false" },
            new() { Key = SettingKeys.SecurityEnabled, Value = "false" },
            new() { Key = SettingKeys.FontSize, Value = "medium" },
            new() { Key = SettingKeys.AccentColor, Value = "#F490AF" },
        };
        
        await db.InsertAllAsync(settings);
    }
    
    #region Settings Helpers
    
    /// <summary>
    /// Gets a setting value by key.
    /// </summary>
    public async Task<string?> GetSettingAsync(string key)
    {
        var db = await GetConnectionAsync();
        var setting = await db.Table<AppSetting>()
            .Where(s => s.Key == key)
            .FirstOrDefaultAsync();
        return setting?.Value;
    }
    
    /// <summary>
    /// Sets a setting value. Creates if doesn't exist, updates if it does.
    /// </summary>
    public async Task SetSettingAsync(string key, string value)
    {
        var db = await GetConnectionAsync();
        var setting = await db.Table<AppSetting>()
            .Where(s => s.Key == key)
            .FirstOrDefaultAsync();
        
        if (setting != null)
        {
            setting.Value = value;
            setting.UpdatedAt = DateTime.UtcNow;
            await db.UpdateAsync(setting);
        }
        else
        {
            await db.InsertAsync(new AppSetting
            {
                Key = key,
                Value = value
            });
        }
    }
    
    /// <summary>
    /// Gets a boolean setting value.
    /// </summary>
    public async Task<bool> GetBoolSettingAsync(string key, bool defaultValue = false)
    {
        var value = await GetSettingAsync(key);
        if (string.IsNullOrEmpty(value))
            return defaultValue;
        
        return value.Equals("true", StringComparison.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Sets a boolean setting value.
    /// </summary>
    public async Task SetBoolSettingAsync(string key, bool value)
    {
        await SetSettingAsync(key, value ? "true" : "false");
    }
    
    #endregion
}