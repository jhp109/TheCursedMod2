using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using TheCursedMod.TheCursedModCode.Cards;

namespace TheCursedMod.TheCursedModCode.Patches;


[HarmonyPatch(typeof(DustyTome), nameof(DustyTome.SetupForPlayer))]
class DustyTomePatch
{
    [HarmonyPrefix]
    static bool AncientPowerPatch(DustyTome __instance, Player player)
    {
        if (player.Character is Character.TheCursedMod)
        {
            __instance.AncientCard = ModelDb.Card<ArchdemonSword>().Id;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(DustyTome), nameof(DustyTome.AfterObtained))]
class DustyTomeForceEvaluationPatch
{
    [HarmonyPrefix]
    static void ForceEvaluation(DustyTome __instance)
    {
        if (__instance.Owner.Character is Character.TheCursedMod)
        {
            __instance.SetupForPlayer((__instance.Owner));
        }
    }
}