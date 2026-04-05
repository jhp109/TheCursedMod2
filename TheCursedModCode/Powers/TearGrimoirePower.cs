using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace TheCursedMod.TheCursedModCode.Powers;

/// <summary>
/// 마도서 찢기 - 비용 2 이상의 공격 카드를 Amount번 더 사용합니다.
/// 업보는 KarmaEveryTurnPower가 별도로 처리합니다.
/// </summary>
public class TearGrimoirePower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2)];

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner.Creature != base.Owner) return playCount;
        if (card.Type != CardType.Attack) return playCount;
        if (card.EnergyCost.GetResolved() < 2) return playCount;
        Flash();
        return playCount + Amount;
    }
}
