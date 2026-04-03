using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Powers;

public class LuckyCharmPower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner == base.Owner.Player && card.Type == CardType.Curse)
        {
            Flash();
            await CreatureCmd.GainBlock(base.Owner, Amount, ValueProp.Unpowered, null);
        }
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner == base.Owner.Player && card.Type == CardType.Curse)
        {
            Flash();
            await CreatureCmd.GainBlock(base.Owner, Amount, ValueProp.Unpowered, null);
        }
    }
}
