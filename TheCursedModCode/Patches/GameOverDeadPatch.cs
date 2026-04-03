using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;
using TheCursedMod.TheCursedModCode.Nodes;

namespace TheCursedMod.TheCursedModCode.Patches;

/// <summary>
/// When the game over screen appears while the player died outside combat
/// (at the merchant or rest site), the base game calls PlayAnimation("die") or
/// SpineAnimation.SetAnimation("die") on our non-Spine character visuals.
/// Neither of these do anything for our Sprite2D-based scenes, so we patch
/// AfterOverlayOpened (the public entry point) to swap the texture ourselves.
/// </summary>
[HarmonyPatch(typeof(NGameOverScreen), nameof(NGameOverScreen.AfterOverlayOpened))]
public static class GameOverDeadPatch
{
    [HarmonyPostfix]
    private static void SwapCorpseTexture(NGameOverScreen __instance)
    {
        var creatureContainer = __instance.GetNodeOrNull<Control>("%CreatureContainer");
        if (creatureContainer == null) return;

        var corpseTexture = GD.Load<Texture2D>("res://TheCursedMod/images/thecursed/TheCursed_corpse.png");
        if (corpseTexture == null) return;

        foreach (var child in creatureContainer.GetChildren())
        {
            // Merchant case: SNMerchantCharacter was reparented here with PlayAnimation("die") (which did nothing)
            if (child is SNMerchantCharacter merchantChar)
            {
                var textureRect = merchantChar.GetNodeOrNull<TextureRect>("SpineSprite");
                if (textureRect != null)
                    textureRect.Texture = corpseTexture;
                continue;
            }

            // Rest site / other non-combat case: a fresh NCreatureVisuals was instantiated
            // and SpineAnimation.SetAnimation("die") was called (which is a no-op for us)
            if (child is SNCreatureVisuals creatureVisuals)
            {
                var sprite = creatureVisuals.FindChild("Sprite") as Sprite2D;
                if (sprite != null)
                    sprite.Texture = corpseTexture;
            }
        }
    }
}
