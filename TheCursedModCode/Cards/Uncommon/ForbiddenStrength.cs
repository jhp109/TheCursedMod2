using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 금단의 힘(Forbidden Strength) - 힘을 1 얻습니다. 이 수치는 손에 있는 마법진 카드 하나당 1 증가합니다.
/// 업보 10. 소멸. 강화 시 힘의 기본 수치 1 → 2.
/// </summary>
public sealed class ForbiddenStrength() : TheCursedModCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<KarmaTurn2Power>("KarmaPower", 10m),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedStrength").WithMultiplier((CardModel card, Creature? _) =>
            (card.IsUpgraded ? 2 : 1) + PileType.Hand.GetPile(card.Owner).Cards.OfType<CircleCard>().Count())
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle),
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int circleCount = PileType.Hand.GetPile(Owner).Cards.OfType<CircleCard>().Count();
        int strengthAmount = (IsUpgraded ? 2 : 1) + circleCount;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, strengthAmount, Owner.Creature, this);
        await ApplyKarma(DynamicVars["KarmaPower"].IntValue);
    }
}
