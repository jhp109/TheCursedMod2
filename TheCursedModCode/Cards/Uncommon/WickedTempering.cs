using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 사악한 담금질(Wicked Tempering) - 업보 13. 의례 : 내 다음 업보 공격의 피해량이 50% (강화: 75%) 증가합니다.
/// </summary>
public sealed class WickedTempering() : RiteCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<KarmaTurn2Power>("KarmaPower", 13m),
        new PowerVar<MultiplyNextKarmaAttackPower>("BonusDamage", 50m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnBaseEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ApplyKarma(DynamicVars["KarmaPower"].IntValue);
    }

    protected override async Task OnRiteEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<MultiplyNextKarmaAttackPower>(
            Owner.Creature, DynamicVars["BonusDamage"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BonusDamage"].UpgradeValueBy(25m);
    }
}
