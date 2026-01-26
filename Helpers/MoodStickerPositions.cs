using System.Collections.Generic;

namespace MoodJournal.Helpers;

/// <summary>
/// Mood sticker layer positions - for HEAD ONLY stickers
/// These positions are for generating personalized mood stickers that combine user's avatar base with mood-specific expressions.
/// </summary>
public static class MoodStickerPositions
{

    //Represents X, Y position and scale for a mood sticker layer
    public record LayerPosition(int X, int Y, double Scale);

    #region BASE POSITIONS


    
    public static readonly LayerPosition JustHeadBasePosition = new(0, 0, 0.75);

    #endregion

    #region EXPRESSION POSITIONS (Gender-Specific)


    // Expression positions for FEMALE mood stickers
    public static class Female
    {
        public static readonly LayerPosition Eyes = new(0, -20, 0.30);
        public static readonly LayerPosition Nose = new(0, -6, 0.10);      // ADJUSTED: was 0.20, now 0.12 (smaller)
        public static readonly LayerPosition Mouth = new(1, 38, 0.12);     // ADJUSTED: was 0.20, now 0.12 (smaller)
    }


    // Expression positions for MALE mood stickers
    public static class Male
    {
        public static readonly LayerPosition Eyes = new(0, -26, 0.30);
        public static readonly LayerPosition Nose = new(0, -12, 0.10);     // ADJUSTED: was 0.20, now 0.12 (smaller)
        public static readonly LayerPosition Mouth = new(1, 38, 0.12);     // ADJUSTED: was 0.21, now 0.13 (smaller)
    }

    #endregion

    #region HAIR POSITIONS

 
    // Female hair positions for mood stickers
    public static readonly Dictionary<string, LayerPosition> HairPositions_Female = new()
    {
        ["medium-curl"] = new LayerPosition(-8, -15, 0.66),
        ["bangs"] = new LayerPosition(1, -11, 0.61),
        ["to-the-side"] = new LayerPosition(-5, -11, 0.60),
        ["boys-cut"] = new LayerPosition(-3, -80, 0.71),
        ["granny-hair"] = new LayerPosition(-1, -75, 0.67),
        ["anime-hair"] = new LayerPosition(-9, -18, 0.66),
        ["bangs-short"] = new LayerPosition(-1, -42, 0.61),
        ["side-pony"] = new LayerPosition(-1, -24, 0.59),
        ["pigtails"] = new LayerPosition(-6, -74, 0.80),
        ["bun"] = new LayerPosition(-12, -81, 0.62)
    };

   
    // Male hair positions for mood stickers
    public static readonly Dictionary<string, LayerPosition> HairPositions_Male = new()
    {
        ["curly-short"] = new LayerPosition(1, -90, 0.65),
        ["straight-short"] = new LayerPosition(4, -86, 0.63),
        ["ceo-hair"] = new LayerPosition(7, -88, 0.63),
        ["flat-hair"] = new LayerPosition(0, -78, 0.55),
        ["90s-hair"] = new LayerPosition(0, -78, 0.55),
        ["bangs"] = new LayerPosition(0, -91, 0.54),
        ["pointed-pony"] = new LayerPosition(1, -117, 0.54),
        ["spiky-hair"] = new LayerPosition(5, -94, 0.54),
        ["dad-hair"] = new LayerPosition(-2, -80, 0.53),
        ["edgy-hair"] = new LayerPosition(-2, -84, 0.38)
    };

    #endregion

    #region FACIAL HAIR POSITIONS (Male Only)


    //Facial hair positions for male mood stickers
    public static readonly Dictionary<string, LayerPosition> FacialHairPositions = new()
    {
        ["style1"] = new LayerPosition(0, 15, 0.20),
        ["style2"] = new LayerPosition(-5, 73, 0.20),
        ["style3"] = new LayerPosition(1, 40, 0.28),
        ["style4"] = new LayerPosition(1, 58, 0.40),
        ["style5"] = new LayerPosition(1, 68, 0.40),
        ["style6"] = new LayerPosition(0, 61, 0.43),
        ["style7"] = new LayerPosition(0, 66, 0.20),
        ["style8"] = new LayerPosition(0, -4, 0.51),
        ["style9"] = new LayerPosition(0, 27, 0.23),
        ["style10"] = new LayerPosition(0, 11, 0.23),
        ["style11"] = new LayerPosition(0, 19, 0.52),
        ["style12"] = new LayerPosition(-3, 62, 0.48),
        ["style13"] = new LayerPosition(-1, 20, 0.20),
        ["style14"] = new LayerPosition(1, 65, 0.20),
        ["style15"] = new LayerPosition(-2, 5, 0.49),
        ["style16"] = new LayerPosition(5, 45, 0.52),
        ["style17"] = new LayerPosition(-2, 43, 0.36),
        ["style18"] = new LayerPosition(-2, 57, 0.20),
        ["style19"] = new LayerPosition(-2, 13, 0.20),
        ["style20"] = new LayerPosition(-1, 18, 0.24)
    };

    #endregion

    #region HELPER METHODS


    // Gets expression positions based on gender

    public static (LayerPosition Eyes, LayerPosition Nose, LayerPosition Mouth) GetExpressionPositions(string gender)
    {
        return gender?.ToLower() == "male"
            ? (Male.Eyes, Male.Nose, Male.Mouth)
            : (Female.Eyes, Female.Nose, Female.Mouth);
    }
    
    // Gets hair position for mood sticker based on gender and style
    public static LayerPosition GetHairPosition(string gender, string style)
    {
        var positions = gender?.ToLower() == "male" 
            ? HairPositions_Male 
            : HairPositions_Female;
        
        // Return position if found, otherwise use default centered position
        return positions.GetValueOrDefault(style, new LayerPosition(0, -60, 0.60));
    }


    // Gets facial hair position for mood sticker (male only)
    public static LayerPosition? GetFacialHairPosition(string? style)
    {
        if (string.IsNullOrEmpty(style))
            return null;
        
        return FacialHairPositions.GetValueOrDefault(style);
    }

    #endregion
}