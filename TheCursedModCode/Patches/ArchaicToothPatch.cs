using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using TheCursedMod.TheCursedModCode.Cards;

namespace TheCursedMod.TheCursedModCode.Patches;

[HarmonyPatch(typeof(ArchaicTooth), "GetTranscendenceStarterCard")]
public static class ArchaicToothStarterPatch
{
    [HarmonyPrefix]
    private static bool FindTheCursedStarterCard(Player player, ref CardModel? __result)
    {
        if (player.Character is not Character.TheCursedMod) return true;
        __result = player.Deck.Cards.FirstOrDefault(c => c.Id == ModelDb.Card<CleanUpWorkshop>().Id);
        return __result == null;
    }
}

[HarmonyPatch(typeof(ArchaicTooth), "GetTranscendenceTransformedCard")]
public static class ArchaicToothTransformPatch
{
    [HarmonyPrefix]
    private static bool TransformTheCursedCard(CardModel starterCard, ref CardModel? __result)
    {
        if (starterCard is not CleanUpWorkshop) return true;
        var canonicalCard = ModelDb.Card<SpringCleaning>();
        var card = starterCard.Owner.RunState.CreateCard(canonicalCard, starterCard.Owner);
        if (starterCard.IsUpgraded)
            CardCmd.Upgrade(card);
        if (starterCard.Enchantment != null)
        {
            var enchantment = (EnchantmentModel)starterCard.Enchantment.MutableClone();
            CardCmd.Enchant(enchantment, card, (decimal)enchantment.Amount);
        }
        __result = card;
        return false;
    }
}
