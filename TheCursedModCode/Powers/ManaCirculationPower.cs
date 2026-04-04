using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace TheCursedMod.TheCursedModCode.Powers;

public class ManaCirculationPower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.ForEnergy(this)
    ];

    private int _lastTriggerRound = -1;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        StartPulsing();
        return Task.CompletedTask;
    }

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player == base.Owner.Player)
            StartPulsing();
        return Task.CompletedTask;
    }

    public async Task TriggerOnRiteEffect(PlayerChoiceContext choiceContext)
    {
        if (CombatState?.RoundNumber == _lastTriggerRound) return;
        _lastTriggerRound = CombatState?.RoundNumber ?? -1;
        StopPulsing();
        Flash();
        await PlayerCmd.GainEnergy(Amount, base.Owner.Player!);
    }
}
