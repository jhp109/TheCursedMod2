using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 뼈에서 살을(Flesh from Bone) - 방어도를 7 얻습니다. 의례 : 판금을 5 얻습니다.
/// (강화 시 방어도 10, 판금 7)
/// </summary>
public sealed class FleshFromBone() : RiteCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.FromPower<PlatingPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(7, ValueProp.Move),
        new PowerVar<PlatingPower>(5m)
    ];

    protected override async Task OnBaseEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    protected override async Task OnRiteEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<PlatingPower>(Owner.Creature, DynamicVars["PlatingPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["PlatingPower"].UpgradeValueBy(2m);
    }
}
