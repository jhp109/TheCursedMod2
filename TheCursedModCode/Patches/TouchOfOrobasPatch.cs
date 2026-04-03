using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using TheCursedMod.TheCursedModCode.Relics;

namespace TheCursedMod.TheCursedModCode.Patches;

[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.GetUpgradedStarterRelic))]
public static class TouchOfOrobasPatch
{
    [HarmonyPostfix]
    private static void AddBlackMagicUpgrade(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic.Id == ModelDb.Relic<BlackMagic101Relic>().Id)
            __result = ModelDb.Relic<BlackMagicAdvancedRelic>().ToMutable();
    }
}
