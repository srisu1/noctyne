using MoodJournal.Models;
using MoodJournal.Services.Interfaces;
using MoodJournal.Helpers;
using System.Text;

namespace MoodJournal.Services;

/// <summary>
/// Service for generating personalized mood stickers
/// Strategy: Build sticker at 400x400 using EXACT PDF positions, then scale entire result
/// </summary>
public class MoodStickerService : IMoodStickerService
{
    private readonly IDatabaseService _databaseService;

    public MoodStickerService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<string?> GenerateMoodStickerSvgAsync(string mood, int width = 80, int height = 80)
    {
        try
        {
            var avatarConfig = await _databaseService.GetUserAvatarAsync();
            if (avatarConfig == null)
            {
                System.Diagnostics.Debug.WriteLine("No avatar configuration found");
                return null;
            }

            if (!Moods.IsValid(mood))
            {
                System.Diagnostics.Debug.WriteLine($"Invalid mood: {mood}");
                return null;
            }

            var svg = BuildMoodStickerSvg(avatarConfig, mood, width, height);
            return svg;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GenerateMoodStickerSvgAsync Error: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> GenerateMoodStickerDataUrlAsync(string mood, int width = 80, int height = 80)
    {
        try
        {
            var svg = await GenerateMoodStickerSvgAsync(mood, width, height);
            if (svg == null)
                return null;

            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(svg));
            return $"data:image/svg+xml;base64,{base64}";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GenerateMoodStickerDataUrlAsync Error: {ex.Message}");
            return null;
        }
    }

    public async Task<Dictionary<string, string>> GenerateAllMoodStickersAsync(int width = 80, int height = 80)
    {
        try
        {
            var stickers = new Dictionary<string, string>();
            foreach (var moodKey in Moods.GetAllMoodKeys())
            {
                var svg = await GenerateMoodStickerSvgAsync(moodKey, width, height);
                if (svg != null)
                {
                    stickers[moodKey] = svg;
                }
            }
            return stickers;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GenerateAllMoodStickersAsync Error: {ex.Message}");
            return new Dictionary<string, string>();
        }
    }

    public async Task<bool> HasConfiguredAvatarAsync()
    {
        try
        {
            var avatarConfig = await _databaseService.GetUserAvatarAsync();
            return avatarConfig != null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"HasConfiguredAvatarAsync Error: {ex.Message}");
            return false;
        }
    }

    private string BuildMoodStickerSvg(AvatarConfiguration avatar, string mood, int width, int height)
    {
        var sb = new StringBuilder();
        
        // Build sticker at 400x400, browser scales entire thing to width x height
        sb.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 400 400\" width=\"{width}\" height=\"{height}\">");
        sb.AppendLine("  <g transform=\"translate(200, 200)\">");

        var expressions = MoodStickerPositions.GetExpressionPositions(avatar.Gender);

        // Layer 1: Just Head Base (X:0, Y:0, Scale:50%)
        AddLayer(sb, 
            $"/assets/avatar/{avatar.Gender}/justheadbase/justheadbase{avatar.BaseIndex.ToString().PadLeft(2, '0')}.png",
            MoodStickerPositions.JustHeadBasePosition);

        // Layer 2: Expression Eyes
        AddLayer(sb, $"/assets/avatar/expressions/{mood}/eyes.png", expressions.Eyes);

        // Layer 3: Expression Nose
        AddLayer(sb, $"/assets/avatar/expressions/{mood}/nose.png", expressions.Nose);

        // Layer 4: Expression Mouth
        AddLayer(sb, $"/assets/avatar/expressions/{mood}/mouth.png", expressions.Mouth);

        // Layer 5: Facial Hair (male only)
        if (avatar.Gender == "male" && !string.IsNullOrEmpty(avatar.FacialHairStyle) && avatar.FacialHairStyle != "none")
        {
            var facialHairPos = MoodStickerPositions.GetFacialHairPosition(avatar.FacialHairStyle);
            if (facialHairPos != null)
            {
                AddLayer(sb, $"/assets/avatar/male/facialhair/{avatar.FacialHairStyle}.png", facialHairPos);
            }
        }

        // Layer 6: Hair
        if (!string.IsNullOrEmpty(avatar.HairStyle) && !string.IsNullOrEmpty(avatar.HairColor))
        {
            var hairPos = MoodStickerPositions.GetHairPosition(avatar.Gender, avatar.HairStyle);
            AddLayer(sb, $"/assets/avatar/{avatar.Gender}/hair/{avatar.HairStyle}-{avatar.HairColor}.png", hairPos);
        }

        sb.AppendLine("  </g>");
        sb.AppendLine("</svg>");

        return sb.ToString();
    }

    private void AddLayer(StringBuilder sb, string imagePath, MoodStickerPositions.LayerPosition pos)
    {
        // All images are 400x400, positioned at (-200,-200) to center them
        // Then apply YOUR EXACT PDF transform: translate(X, Y) scale(Scale)
        sb.AppendLine($"    <image href=\"{imagePath}\" x=\"-200\" y=\"-200\" width=\"400\" height=\"400\" transform=\"translate({pos.X}, {pos.Y}) scale({pos.Scale})\" />");
    }
}