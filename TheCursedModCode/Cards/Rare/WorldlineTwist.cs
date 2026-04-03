using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 세계선 비틀기(Worldline Twist) - 이번 턴에 마법진의 효과가 발동한 횟수당 에너지 소모가 1 감소합니다.
/// 이번 턴에 업보로 인한 피해를 받지 않습니다. 비용 4, Rare, Skill. 강화 시 비용 3.
/// 마법진 효과 발동 시 OnCircleTrigger가 호출되어 이번 턴 비용이 1 감소합니다.
/// </summary>
public sealed class WorldlineTwist() : TheCursedModCard(4, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle),
        EnergyHoverTip,
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<WorldlineTwistPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    /// <summary>
    /// 마법진 효과가 발동될 때마다 CircleCard.TriggerEffect에서 호출됩니다.
    /// </summary>
    public void OnCircleTrigger()
    {
        EnergyCost.AddThisTurn(-1);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
