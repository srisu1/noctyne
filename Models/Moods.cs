namespace MoodJournal.Models;

/// <summary>
/// Available moods for journal entries
/// Each mood has an associated emoji and label
/// </summary>
public static class Moods
{
    // Mood constants
    public const string Happy = "happy";
    public const string Sad = "sad";
    public const string Excited = "excited";
    public const string Anxious = "anxious";
    public const string Calm = "calm";
    public const string Angry = "angry";
    public const string Tired = "tired";
    public const string Grateful = "grateful";

   
    //Mood data: emoji and display label
    public record MoodInfo(string Emoji, string Label);

 
    // All available moods with their emoji and label
    public static readonly Dictionary<string, MoodInfo> All = new()
    {
        [Happy] = new MoodInfo("😊", "Happy"),
        [Sad] = new MoodInfo("😢", "Sad"),
        [Excited] = new MoodInfo("🤩", "Excited"),
        [Anxious] = new MoodInfo("😰", "Anxious"),
        [Calm] = new MoodInfo("😌", "Calm"),
        [Angry] = new MoodInfo("😠", "Angry"),
        [Tired] = new MoodInfo("😴", "Tired"),
        [Grateful] = new MoodInfo("🙏", "Grateful")
    };

 
    // Gets list of all mood keys

    public static List<string> GetAllMoodKeys() => All.Keys.ToList();

    
    // Gets mood info (emoji + label) for a given mood
    public static MoodInfo? GetMoodInfo(string? mood)
    {
        if (string.IsNullOrEmpty(mood))
            return null;

        return All.GetValueOrDefault(mood);
    }
    
    
    //Gets emoji for a mood
    public static string GetEmoji(string? mood)
    {
        return GetMoodInfo(mood)?.Emoji ?? "❓";
    }

 
    //Gets display label for a mood
  
    public static string GetLabel(string? mood)
    {
        return GetMoodInfo(mood)?.Label ?? "Unknown";
    }


    //Validates if a mood key is valid
    public static bool IsValid(string? mood)
    {
        return !string.IsNullOrEmpty(mood) && All.ContainsKey(mood);
    }
}