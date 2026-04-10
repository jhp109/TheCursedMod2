using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 축지법(Celerity) - 의례 : 손에 있는 마법진 카드 하나당 민첩을 2 얻습니다. 소멸.
/// 비용 1. 강화 시 비용 0.
/// </summary>
public sealed class Celerity() : RiteCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0m),
        new CalculationExtraVar(2m),
        new CalculatedVar("CalculatedDexterity").WithMultiplier(static (card, _) =>
            PileType.Hand.GetPile(card.Owner).Cards.Count(c => c is CircleCard))
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle),
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    protected override Task OnRiteEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int dexterityAmount = (int)((CalculatedVar)DynamicVars["CalculatedDexterity"]).Calculate(null);
        return PowerCmd.Apply<DexterityPower>(Owner.Creature, dexterityAmount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
