using SQLite;

namespace MoodJournal.Models;

/// <summary>
/// User profile model - links to avatar configuration
/// Simple profile with name and reference to avatar
/// </summary>
[Table("UserProfile")]
public class UserProfile
{
    [PrimaryKey, AutoIncrement]
    [Column("Id")]
    public int Id { get; set; }
    
    [Column("Name"), NotNull]
    public string Name { get; set; } = string.Empty;
    
  
    // Foreign key to AvatarConfiguration table
    // Links to the user's complete avatar customization

    [Column("AvatarConfigId")]
    public int? AvatarConfigId { get; set; }
    

    //Complete avatar SVG markup for display
    //Generated from AvatarConfiguration
    //Used for profile buttons throughout the app
   
    [Column("AvatarSvg"), NotNull]
    public string AvatarSvg { get; set; } = string.Empty;
    
    [Column("CreatedAt"), NotNull]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("UpdatedAt"), NotNull]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}