using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 고대신의 부름(Call of the Ancient) - 이번 턴에, 다음에 사용하는 의례 카드의 의례 효과가 두 번 발동합니다.
/// (강화 시 다음에 사용하는 의례 카드 2장의 의례 효과가 두 번 발동합니다. 턴 종료 시 제거되지 않음)
/// </summary>
public sealed class CallOfTheAncient() : TheCursedModCard(1, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<CallOfTheAncientPower>(1m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<CallOfTheAncientPower>(
            Owner!.Creature, DynamicVars["CallOfTheAncientPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CallOfTheAncientPower"].UpgradeValueBy(1m);
    }
}
