using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 사악한 수호(Wicked Ward) - 의례 : 다른 모든 플레이어들에게 방어도를 22 줍니다.
/// 멀티플레이어 전용. 비용 2, Uncommon, Skill. 강화 시 방어도 28.
/// </summary>
public sealed class WickedWard() : RiteCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(22, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
    ];

    protected override async Task OnRiteEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var allies = CombatState!.Allies.Where(c => c != Owner.Creature).ToList();
        foreach (var ally in allies)
            await CreatureCmd.GainBlock(ally, DynamicVars.Block.IntValue, ValueProp.Move, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(6m);
    }
}
