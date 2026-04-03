using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using TheCursedMod.TheCursedModCode.Cards;

namespace TheCursedMod.TheCursedModCode.Patches;

[HarmonyPatch(typeof(ArchaicTooth), "TranscendenceUpgrades", MethodType.Getter)]
public static class ArchaicToothPatch
{
    [HarmonyPostfix]
    private static void AddTheCursedTranscendence(ref Dictionary<ModelId, CardModel> __result)
    {
        __result[ModelDb.Card<CleanUpWorkshop>().Id] = ModelDb.Card<SpringCleaning>();
    }
}
