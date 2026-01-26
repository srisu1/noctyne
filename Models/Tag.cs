using SQLite;

namespace MoodJournal.Models;

/// <summary>
/// Represents a tag that can be attached to journal entries
/// Tags are user-created labels for organizing entries
/// </summary>
public class Tag
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

  
    //tag name (e.g., "work", "personal", "goals")
    //Stored in lowercase for consistency
    [Indexed(Name = "IX_TagName", Unique = true)]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;


    //Number of times this tag has been used
    //Updated automatically when entries are created/deleted
    public int UsageCount { get; set; }


    //When this tag was first created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

 
    //When this tag was last used
    public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Junction table for many-to-many relationship between entries and tags
/// One entry can have multiple tags, one tag can be on multiple entries
/// </summary>
public class EntryTag
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

 
    //Foreign key to JournalEntry
    [Indexed(Name = "IX_EntryId")]
    public int EntryId { get; set; }


    //Foreign key to Tag
    [Indexed(Name = "IX_TagId")]
    public int TagId { get; set; }


    // When this tag was added to the entry
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}