using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 축지법(Celerity) - 의례 : 민첩을 1 얻습니다. 이 수치는 손에 있는 마법진 카드 하나당 1 증가합니다. 소멸.
/// 강화 시 기본 민첩 1 → 2.
/// </summary>
public sealed class Celerity() : RiteCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedDexterity").WithMultiplier((CardModel card, Creature? _) =>
            (card.IsUpgraded ? 2 : 1) + PileType.Hand.GetPile(card.Owner).Cards.OfType<CircleCard>().Count())
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle),
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    protected override Task OnRiteEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int circleCount = PileType.Hand.GetPile(Owner).Cards.OfType<CircleCard>().Count();
        int dexAmount = (IsUpgraded ? 2 : 1) + circleCount;
        return PowerCmd.Apply<DexterityPower>(Owner.Creature, dexAmount, Owner.Creature, this);
    }
}
