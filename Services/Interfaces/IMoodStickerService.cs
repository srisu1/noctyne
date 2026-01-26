namespace MoodJournal.Services.Interfaces;

/// <summary>
/// Service interface for generating personalized mood stickers
/// Combines user's avatar configuration with mood-specific expressions
/// </summary>
public interface IMoodStickerService
{
  
    //Generates an SVG mood sticker for a given mood
    //Combines user's justheadbase, hair, facial hair with mood expressions
    Task<string?> GenerateMoodStickerSvgAsync(string mood, int width = 80, int height = 80);


    //Generates mood sticker data URL (base64 encoded) for use in img src

    Task<string?> GenerateMoodStickerDataUrlAsync(string mood, int width = 80, int height = 80);
    
    //Generates mood stickers for all 8 moods
    //Useful for preloading/caching stickers
    Task<Dictionary<string, string>> GenerateAllMoodStickersAsync(int width = 80, int height = 80);


    //Checks if user has a configured avatar for mood sticker generation
    Task<bool> HasConfiguredAvatarAsync();
}