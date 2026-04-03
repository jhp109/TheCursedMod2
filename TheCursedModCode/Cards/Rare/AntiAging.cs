using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheCursedMod.TheCursedModCode;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 회춘(AntiAging) - 방어도를 4 얻습니다. 의례 : 최대 체력이 4 증가합니다. 소멸. (강화 시 방어도 5, 최대 체력 5)
/// </summary>
public sealed class AntiAging() : RiteCard(1, CardType.Skill, CardRarity.Rare, TargetType.None) {
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(4, ValueProp.Move),
        new MaxHpVar(4)
    ];

    protected override async Task OnBaseEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    protected override async Task OnRiteEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
        DynamicVars.MaxHp.UpgradeValueBy(1m);
    }
}
