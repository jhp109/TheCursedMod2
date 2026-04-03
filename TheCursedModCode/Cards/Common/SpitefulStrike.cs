using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 원한 맺힌 타격(Spiteful Strike) - 피해를 12 줍니다.
/// 손에 사용불가 카드가 있다면, 에너지를 2 얻습니다. (강화 시 피해 16)
/// </summary>
public sealed class SpitefulStrike() : TheCursedModCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Unplayable),
        EnergyHoverTip
    ];
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(12, ValueProp.Move),
        new EnergyVar(2),
    ];
    
    protected override bool ShouldGlowGoldInternal => HasUnplayableInHand();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        if (HasUnplayableInHand())
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
