using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 금단의 형상(Forbidden Form) - 의례로 저주 카드를 소멸시킬 때 마다,
/// 업보를 부여하는 내 다음 공격의 피해량이 50% (강화: 75%) 증가합니다.
/// </summary>
public sealed class ForbiddenForm() : TheCursedModCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ForbiddenFormPower>("BonusPercent", 50m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ForbiddenFormPower>(Owner.Creature, IsUpgraded ? 50 : 25, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BonusPercent"].UpgradeValueBy(25m);
    }
}
