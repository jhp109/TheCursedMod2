using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using TheCursedMod.TheCursedModCode.Cards;
using TheCursedMod.TheCursedModCode.Character;

namespace TheCursedMod.TheCursedModCode.Relics;

[Pool(typeof(TheCursedModRelicPool))]
public sealed class SpiritFuelRelic : TheCursedModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != base.Owner)
            return count;

        if (!RiteCard.WasRiteEffectTriggeredLastTurn(player.Creature?.CombatState))
            return count;

        Flash();
        return count + 1;
    }
}
