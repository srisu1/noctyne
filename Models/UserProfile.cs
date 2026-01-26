using SQLite;

namespace MoodJournal.Models;

/// <summary>
/// User profile - one per app
/// </summary>
[Table("UserProfile")]
public class UserProfile
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [NotNull]
    public string Name { get; set; } = string.Empty;
    
    public int? AvatarConfigId { get; set; }
    
    [NotNull]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [NotNull]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}