using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 액땜 부적(Lucky Charm) - 저주 카드가 소멸될 때 마다, 방어도를 3 얻습니다. (강화 시 방어도 4)
/// </summary>
public sealed class LuckyCharm() : TheCursedModCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<LuckyCharmPower>(3m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<LuckyCharmPower>(Owner!.Creature, DynamicVars["LuckyCharmPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["LuckyCharmPower"].UpgradeValueBy(1m);
    }
}
