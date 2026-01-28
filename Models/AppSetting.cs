using SQLite;

namespace MoodJournal.Models;

/// <summary>
/// Application settings model for database storage
/// </summary>
[Table("AppSettings")]
public class AppSetting
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [Indexed(Unique = true), NotNull]
    public string SettingKey { get; set; } = string.Empty;
    
    public string? SettingValue { get; set; }
    
    [NotNull]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}