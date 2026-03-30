using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace TheCursedMod.TheCursedModCode.Powers;

/// <summary>
/// 마도서 찢기 - 비용 2 이상의 공격 카드를 한 번 더 사용합니다.
/// 내 턴 시작 시, Amount만큼 업보를 얻습니다.
/// IsInstanced = true: 여러 장 사용해도 각각 독립적으로 작동합니다.
/// </summary>
public class TearGrimoirePower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool IsInstanced => true;

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner.Creature != base.Owner) return playCount;
        if (card.Type != CardType.Attack) return playCount;
        if (card.EnergyCost.GetResolved() < 2) return playCount;
        Flash();
        return playCount + 1;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player) return;
        Flash();
        if (base.Owner.HasPower<GracePower>())
            await PowerCmd.Apply<KarmaTurn3Power>(base.Owner, Amount, base.Owner, null);
        else
            await PowerCmd.Apply<KarmaTurn2Power>(base.Owner, Amount, base.Owner, null);
    }
}
