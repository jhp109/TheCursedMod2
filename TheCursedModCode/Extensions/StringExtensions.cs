namespace TheCursedMod.TheCursedModCode.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    public static string ImagePath(this string path)
    {
        return Path.Join(TheCursedModMainFile.ModId, "images", path);
    }
    
    public static string CardImagePath(this string path)
    {
        return Path.Join(TheCursedModMainFile.ModId, "images", "card_portraits", path);
    }
    public static string BigCardImagePath(this string path)
    {
        return Path.Join(TheCursedModMainFile.ModId, "images", "card_portraits", "big", path);
    }

    public static string PowerImagePath(this string path)
    {
        return Path.Join(TheCursedModMainFile.ModId, "images", "powers", path);
    }

    public static string BigPowerImagePath(this string path)
    {
        return Path.Join(TheCursedModMainFile.ModId, "images", "powers", "big", path);
    }

    public static string RelicImagePath(this string path)
    {
        return Path.Join(TheCursedModMainFile.ModId, "images", "relics", path);
    }

    public static string BigRelicImagePath(this string path)
    {
        return Path.Join(TheCursedModMainFile.ModId, "images", "relics", "big", path);
    }

    public static string PotionImagePath(this string path)
    {
        return Path.Join(TheCursedModMainFile.ModId, "images", "potions", path);
    }

    public static string CharacterUiPath(this string path)
    {
        return Path.Join(TheCursedModMainFile.ModId, "images", "charui", path);
    }

    public static string TopPanelUiPath(this string path)
    {
        return Path.Join(TheCursedModMainFile.ModId, "images", "ui", "top_panel", path);
    }

    public static string ScenePath(this string path)
    {
        return Path.Join(TheCursedModMainFile.ModId, "scenes", path);
    }
}