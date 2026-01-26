using SQLite;

namespace MoodJournal.Models;

[Table("AvatarConfiguration")]
public class AvatarConfiguration
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [NotNull]
    public string Gender { get; set; } = "female";
    
    [NotNull]
    public int BaseIndex { get; set; } = 1;
    
    [NotNull]
    public int EyesIndex { get; set; } = 1;
    
    [NotNull]
    public int NoseIndex { get; set; } = 1;
    
    [NotNull]
    public int MouthIndex { get; set; } = 1;
    
    public string? HairStyle { get; set; }
    public string? HairColor { get; set; }
    public string? ClothesStyle { get; set; }
    public string? ClothesColor { get; set; }
    public string? FacialHairStyle { get; set; }
    public string? GlassesStyle { get; set; }
    public string? HeadwearStyle { get; set; }
    public string? HeadwearColor { get; set; }
    public string? NeckwearStyle { get; set; }
    public string? NeckwearColor { get; set; }
    public string? ExtrasStyle { get; set; }
    public string? ExtrasColor { get; set; }
    
    [NotNull]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [NotNull]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
