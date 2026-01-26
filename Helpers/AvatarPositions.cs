namespace MoodJournal.Helpers;

/// <summary>
/// Avatar layer positions - calibrated from working HTML
/// </summary>
public static class AvatarPositions
{
    public record LayerPosition(int X, int Y, double Scale);

    // DEFAULT LAYER POSITIONS 
    public static readonly Dictionary<string, LayerPosition> DefaultPositions = new()
    {
        ["base"] = new LayerPosition(0, 0, 0.30),
        ["eyes"] = new LayerPosition(0, -31, 0.30),
        ["nose"] = new LayerPosition(0, -15, 0.30),
        ["mouth"] = new LayerPosition(-2, 68, 0.30),
        ["clothes"] = new LayerPosition(0, 210, 0.30),
        ["facialHair"] = new LayerPosition(0, 34, 0.30),
        ["glasses"] = new LayerPosition(1, -25, 0.30),
        ["headwear"] = new LayerPosition(0, -100, 0.30),
        ["neckwear"] = new LayerPosition(0, 120, 0.30),
        ["extras"] = new LayerPosition(0, 50, 0.30)
    };

    #region HAIR POSITIONS (from working HTML)

    // Female hair positions 
    public static readonly Dictionary<string, LayerPosition> HairPositions_Female = new()
    {
        ["bangs-short"] = new LayerPosition(-3, -76, 0.30),
        ["bangs"] = new LayerPosition(-2, -22, 0.30),
        ["medium-curl"] = new LayerPosition(-8, -35, 0.30),
        ["to-the-side"] = new LayerPosition(-9, -19, 0.30),
        ["granny-hair"] = new LayerPosition(-1, -137, 0.30),
        ["anime-hair"] = new LayerPosition(-17, -30, 0.30),
        ["side-pony"] = new LayerPosition(-3, -40, 0.30),
        ["pigtails"] = new LayerPosition(-9, -119, 0.30),
        ["bun"] = new LayerPosition(-20, -148, 0.30),
        ["boys-cut"] = new LayerPosition(-4, -140, 0.30)
    };

    // Male hair positions 
    public static readonly Dictionary<string, LayerPosition> HairPositions_Male = new()
    {
        ["curly-short"] = new LayerPosition(2, -167, 0.30),
        ["straight-short"] = new LayerPosition(4, -138, 0.30),
        ["ceo-hair"] = new LayerPosition(7, -143, 0.30),
        ["flat-hair"] = new LayerPosition(-3, -143, 0.30),
        ["90s-hair"] = new LayerPosition(-1, -137, 0.30),
        ["bangs"] = new LayerPosition(-2, -159, 0.30),  // Different from female!
        ["pointed-pony"] = new LayerPosition(-1, -200, 0.30),
        ["spiky-hair"] = new LayerPosition(9, -170, 0.30),
        ["dad-hair"] = new LayerPosition(1, -145, 0.30),
        ["edgy-hair"] = new LayerPosition(-2, -145, 0.30)
    };

    #endregion

    #region CLOTHES POSITIONS 

    public static readonly Dictionary<string, LayerPosition> ClothesPositions_Female = new()
    {
        ["off-shoulder"] = new LayerPosition(-7, 214, 0.29),
        ["night-dress"] = new LayerPosition(-4, 214, 0.32),
        ["sweater"] = new LayerPosition(-4, 214, 0.32),
        ["c-neck"] = new LayerPosition(-4, 214, 0.32),
        ["tank-top"] = new LayerPosition(6, 214, 0.30),
        ["v-neck-sweater"] = new LayerPosition(-4, 214, 0.32)
    };

    public static readonly Dictionary<string, LayerPosition> ClothesPositions_Male = new()
    {
        ["uniform"] = new LayerPosition(1, 230, 0.39),
        ["button-up-shirt"] = new LayerPosition(1, 230, 0.39),
        ["sweater"] = new LayerPosition(-3, 239, 0.40),
        ["c-neck"] = new LayerPosition(-3, 239, 0.40),
        ["v-neck"] = new LayerPosition(-3, 239, 0.40),
        ["tank-top"] = new LayerPosition(7, 212, 0.32)
    };

    #endregion

    #region ACCESSORY POSITIONS 

    public static readonly Dictionary<string, LayerPosition> FacialHairPositions = new()
    {
        ["style1"] = new LayerPosition(-3, 31, 0.30),
        ["style2"] = new LayerPosition(-7, 122, 0.30),
        ["style3"] = new LayerPosition(4, 70, 0.30),
        ["style4"] = new LayerPosition(3, 105, 0.30),
        ["style5"] = new LayerPosition(1, 120, 0.30),
        ["style6"] = new LayerPosition(-3, 100, 0.30),
        ["style7"] = new LayerPosition(-2, 118, 0.30),
        ["style8"] = new LayerPosition(-2, -13, 0.30),
        ["style9"] = new LayerPosition(0, 40, 0.30),
        ["style10"] = new LayerPosition(0, 18, 0.30),
        ["style11"] = new LayerPosition(-2, 35, 0.30),
        ["style12"] = new LayerPosition(-4, 109, 0.30),
        ["style13"] = new LayerPosition(-1, 33, 0.30),
        ["style14"] = new LayerPosition(-1, 111, 0.30),
        ["style15"] = new LayerPosition(-1, 6, 0.29),
        ["style16"] = new LayerPosition(-2, 86, 0.64),
        ["style17"] = new LayerPosition(-2, 86, 0.34),
        ["style18"] = new LayerPosition(-2, 97, 0.30),
        ["style19"] = new LayerPosition(-2, 29, 0.30),
        ["style20"] = new LayerPosition(-2, 44, 0.30)
    };

    public static readonly Dictionary<string, LayerPosition> GlassesPositions = new()
    {
        ["default"] = new LayerPosition(1, -25, 0.30)
    };

    public static readonly Dictionary<string, LayerPosition> HeadwearPositions = new()
    {
        ["hairband1"] = new LayerPosition(-8, -150, 0.30),
        ["hairband2"] = new LayerPosition(-8, -150, 0.30),
        ["basketball-cap"] = new LayerPosition(0, -215, 0.30),
        ["french-cap"] = new LayerPosition(-24, -228, 0.30),
        ["hat"] = new LayerPosition(33, -225, 0.30),
        ["winter-cap"] = new LayerPosition(-3, -254, 0.30),
        ["magician-hat"] = new LayerPosition(-3, -254, 0.30),
        ["sideways-baseball-cap"] = new LayerPosition(77, -204, 0.29)
    };

    public static readonly Dictionary<string, LayerPosition> NeckwearPositions = new()
    {
        ["boy-tie"] = new LayerPosition(0, 160, 0.11),
        ["straight-tie"] = new LayerPosition(0, 241, 0.11)
    };

    public static readonly Dictionary<string, LayerPosition> ExtrasPositions = new()
    {
        ["side-bow"] = new LayerPosition(-141, -200, 0.30),
        ["lady-hat"] = new LayerPosition(0, -193, 0.30),
        ["flower"] = new LayerPosition(-170, -66, 0.30)
    };

    #endregion

    #region HELPER METHODS

    public static LayerPosition GetHairPosition(string gender, string style)
    {
        var positions = gender == "male" ? HairPositions_Male : HairPositions_Female;
        return positions.GetValueOrDefault(style, DefaultPositions["clothes"]);
    }

    public static LayerPosition GetClothesPosition(string gender, string style)
    {
        var positions = gender == "male" ? ClothesPositions_Male : ClothesPositions_Female;
        return positions.GetValueOrDefault(style, DefaultPositions["clothes"]);
    }

    public static LayerPosition GetFacialHairPosition(string style)
    {
        return FacialHairPositions.GetValueOrDefault(style, DefaultPositions["facialHair"]);
    }

    public static LayerPosition GetGlassesPosition(string style)
    {
        return GlassesPositions.GetValueOrDefault(style, DefaultPositions["glasses"]);
    }

    public static LayerPosition GetHeadwearPosition(string style)
    {
        return HeadwearPositions.GetValueOrDefault(style, DefaultPositions["headwear"]);
    }

    public static LayerPosition GetNeckwearPosition(string style)
    {
        return NeckwearPositions.GetValueOrDefault(style, DefaultPositions["neckwear"]);
    }

    public static LayerPosition GetExtrasPosition(string style)
    {
        return ExtrasPositions.GetValueOrDefault(style, DefaultPositions["extras"]);
    }

    #endregion
}