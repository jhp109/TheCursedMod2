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
/// 비용 1. 강화 시 민첩 3.
/// </summary>
public sealed class Celerity() : RiteCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<DexterityPower>("Dexterity", 2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle),
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    protected override Task OnRiteEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int circleCount = PileType.Hand.GetPile(Owner).Cards.OfType<CircleCard>().Count();
        return PowerCmd.Apply<DexterityPower>(Owner.Creature, circleCount * DynamicVars["Dexterity"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Dexterity"].UpgradeValueBy(1m);
    }
}
