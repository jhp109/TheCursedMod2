using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheCursedMod.TheCursedModCode.Character;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Relics;

[Pool(typeof(TheCursedModRelicPool))]
public sealed class WraithBeadsRelic : TheCursedModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner)
            return;

        if (Owner.Creature.CombatState == null
            || Owner.Creature.CombatState.RoundNumber <= 1)
            return;

        if (!KarmaTurn1Power.WasKarmaHitLastTurn(Owner.Creature.CombatState))
            return;

        Flash();
        await PlayerCmd.GainEnergy(1, Owner);
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner)
            return count;

        if (!KarmaTurn1Power.WasKarmaHitLastTurn(player.Creature?.CombatState))
            return count;

        return count + 3;
    }
}
