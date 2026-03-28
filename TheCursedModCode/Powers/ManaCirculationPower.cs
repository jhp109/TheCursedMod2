using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.ForEnergy(this)
    ];

    private bool _triggeredThisTurn;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner == base.Owner.Player && card.Type == CardType.Curse && !_triggeredThisTurn)
        {
            _triggeredThisTurn = true;
            Flash();
            await PlayerCmd.GainEnergy(Amount, base.Owner.Player!);
        }
    }

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player == base.Owner.Player)
            _triggeredThisTurn = false;
        return Task.CompletedTask;
    }
}
