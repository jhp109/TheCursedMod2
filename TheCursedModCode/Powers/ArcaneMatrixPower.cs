using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using TheCursedMod.TheCursedModCode.Cards;

namespace TheCursedMod.TheCursedModCode.Powers;

/// <summary>
/// 비전 행렬 - 마법진 카드를 뽑을 때 마다, 에너지를 1 얻고 카드를 1장 뽑습니다.
/// AfterCardDrawn에서 CircleCard를 감지하여 발동합니다.
/// </summary>
public class ArcaneMatrixPower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle),
        HoverTipFactory.ForEnergy(this)
    ];
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != Owner.Player) return;
        if (card is not CircleCard) return;
        Flash();
        await PlayerCmd.GainEnergy(Amount, Owner.Player);
        await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
    }
}
