namespace MoodJournal.Helpers
{
    public static class AvatarAssets
    {
        // Example: base layers
        public static string GetBasePath(int index) => $"/images/avatar/base/base{index}.png";
        public static string GetEyesPath(int index) => $"/images/avatar/eyes/eyes{index}.png";
        public static string GetNosePath(int index) => $"/images/avatar/nose/nose{index}.png";
        public static string GetMouthPath(int index) => $"/images/avatar/mouth/mouth{index}.png";

        // Styles for things like hair, clothes, etc.
        public static string GetHairPath(string? style, string? color)
            => style != null ? $"/images/avatar/hair/{style}_{color ?? "default"}.png" : string.Empty;

        public static string GetClothesPath(string? style, string? color)
            => style != null ? $"/images/avatar/clothes/{style}_{color ?? "default"}.png" : string.Empty;

        public static string GetFacialHairPath(string? style)
            => style != null ? $"/images/avatar/facialHair/{style}.png" : string.Empty;

        public static string GetGlassesPath(string? style)
            => style != null ? $"/images/avatar/glasses/{style}.png" : string.Empty;

        public static string GetHeadwearPath(string? style, string? color)
            => style != null ? $"/images/avatar/headwear/{style}_{color ?? "default"}.png" : string.Empty;

        public static string GetNeckwearPath(string? style, string? color)
            => style != null ? $"/images/avatar/neckwear/{style}_{color ?? "default"}.png" : string.Empty;

        public static string GetExtrasPath(string? style, string? color)
            => style != null ? $"/images/avatar/extras/{style}_{color ?? "default"}.png" : string.Empty;
    }
}