using SQLite;

namespace MoodJournal.Models;

/// <summary>
/// Represents a daily journal entry with mood, tags, and rich text content
/// Constraint: Only ONE entry allowed per day
/// </summary>
public class JournalEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

  
    //Optional title for the entry (max 100 characters)
    [MaxLength(100)]
    public string? Title { get; set; }


    //Rich text HTML content (from Quill editor or similar)
    public string Content { get; set; } = string.Empty;


    //Entry date (YYYY-MM-DD format, ONE entry per day)
    [Indexed(Name = "IX_EntryDate", Unique = true)]
    public string EntryDate { get; set; } = string.Empty;


    //Primary mood (REQUIRED) - one of: happy, sad, excited, anxious, calm, angry, tired, grateful
    public string PrimaryMood { get; set; } = string.Empty;

 
    //Secondary moods (up to 2) stored as comma-separated string
    //Example: "calm,grateful" or empty string
    public string SecondaryMoods { get; set; } = string.Empty;

 
    //Word count (calculated from content)
    public int WordCount { get; set; }


    // Character count (calculated from content, excluding HTML tags)
    public int CharacterCount { get; set; }


    //When the entry was created (system timestamp)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    //When the entry was last modified (system timestamp)
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    #region Helper Methods


    //gets secondary moods as a list
    [Ignore]
    public List<string> SecondaryMoodsList
    {
        get => string.IsNullOrWhiteSpace(SecondaryMoods)
            ? new List<string>()
            : SecondaryMoods.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        set => SecondaryMoods = string.Join(",", value);
    }


    //Gets all moods (primary + secondary)
    [Ignore]
    public List<string> AllMoods
    {
        get
        {
            var moods = new List<string> { PrimaryMood };
            moods.AddRange(SecondaryMoodsList);
            return moods;
        }
    }

    #endregion
}