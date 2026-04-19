using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using TheCursedMod.TheCursedModCode.Extensions;
using TheCursedCharacter = TheCursedMod.TheCursedModCode.Character.TheCursedMod;

namespace TheCursedMod.TheCursedModCode.Patches;

[HarmonyPatch(typeof(CharacterModel), "IconOutlineTexturePath", MethodType.Getter)]
public static class IconOutlineTexturePatch
{
    [HarmonyPrefix]
    private static bool CustomOutline(CharacterModel __instance, ref string __result)
    {
        if (__instance is not TheCursedCharacter)
            return true;
        __result = $"res://{TheCursedModMainFile.ModId}/images/ui/top_panel/character_icon_the_cursed_outline.png";
        return false;
    }
}
