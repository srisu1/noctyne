using SQLite;

namespace MoodJournal.Models;

/// <summary>
/// Key-value store for application settings.
/// </summary>
[Table("AppSettings")]
public class AppSetting
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    /// <summary>
    /// Setting key name.
    /// </summary>
    [MaxLength(100), Unique]
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// Setting value (stored as string, parsed as needed).
    /// </summary>
    public string? Value { get; set; }
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Well known setting keys for type safe access.
/// </summary>
public static class SettingKeys
{
    public const string Theme = "Theme";
    public const string IsFirstLaunch = "IsFirstLaunch";
    public const string OnboardingCompleted = "OnboardingCompleted";
    public const string AvatarCompleted = "AvatarCompleted";
    public const string SecurityEnabled = "SecurityEnabled";
    public const string FontSize = "FontSize";
    public const string AccentColor = "AccentColor";
}