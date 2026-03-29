using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 뉘우침(Repentance) - 방어도를 6 얻습니다.
/// 이번 턴에 업보로 피해를 받을 예정이라면, 대신 13 얻습니다.
/// 강화 시 6→9 / 13→17
/// </summary>
public sealed class Repentance() : TheCursedModCard(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(6, ValueProp.Move),
        new BlockVar("KarmaBlock", 13, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override bool ShouldGlowGoldInternal => Owner?.Creature.HasPower<KarmaTurn1Power>() == true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner!.Creature, "Cast", Owner.Character.CastAnimDelay);
        bool hasKarma = Owner.Creature.HasPower<KarmaTurn1Power>();
        decimal blockAmount = hasKarma ? DynamicVars["KarmaBlock"].PreviewValue : DynamicVars.Block.PreviewValue;
        await CreatureCmd.GainBlock(Owner.Creature, blockAmount, ValueProp.Move, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["KarmaBlock"].UpgradeValueBy(4m);
    }
}
