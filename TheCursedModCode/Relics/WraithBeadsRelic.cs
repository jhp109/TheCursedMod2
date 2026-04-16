using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheCursedMod.TheCursedModCode.Character;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Relics;

[Pool(typeof(TheCursedModRelicPool))]
public sealed class WraithBeadsRelic : TheCursedModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2),
        new CardsVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma),
    ];

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner)
            return;

        if (Owner.Creature.CombatState == null
            || Owner.Creature.CombatState.RoundNumber <= 1)
            return;

        if (!KarmaTurn1Power.WasKarmaHitLastTurn(Owner.Creature.CombatState, Owner))
            return;

        Flash();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner)
            return count;

        if (!KarmaTurn1Power.WasKarmaHitLastTurn(player.Creature?.CombatState, player))
            return count;

        return count + DynamicVars.Cards.IntValue;
    }
}
