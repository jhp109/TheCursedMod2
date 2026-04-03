using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace TheCursedMod.TheCursedModCode.Patches;

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class DeathAnimPatch
{
    [HarmonyPostfix]
    private static void SwapToCorpseSprite(NCreature __instance)
    {
        if (__instance.Entity?.Player?.Character is not Character.TheCursedMod)
            return;

        var sprite = __instance.Visuals?.FindChild("Sprite") as Sprite2D;
        if (sprite == null)
            return;

        var corpseTexture = GD.Load<Texture2D>("res://TheCursedMod/images/thecursed/TheCursed_corpse.png");
        if (corpseTexture != null)
            sprite.Texture = corpseTexture;
    }
}
