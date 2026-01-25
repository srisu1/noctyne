using SQLite;

namespace MoodJournal.Services.Interfaces;

/// <summary>
/// Interface for database operations.
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Initializes the database and creates tables if they don't exist.
    /// </summary>
    Task InitializeAsync();
    
    /// <summary>
    /// Gets the SQLite connection.
    /// </summary>
    Task<SQLiteAsyncConnection> GetConnectionAsync();
    
    // Settings helpers
    Task<string?> GetSettingAsync(string key);
    Task SetSettingAsync(string key, string value);
    Task<bool> GetBoolSettingAsync(string key, bool defaultValue = false);
    Task SetBoolSettingAsync(string key, bool value);
}