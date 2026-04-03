using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 공방 대청소(Spring Cleaning) - 방어도를 6 얻습니다. 의례 : 에너지를 1 얻고 카드를 2장 뽑습니다.
/// 비용 0, Ancient, Skill. 강화 시 방어도 8, 카드 3장 뽑음.
/// </summary>
public sealed class SpringCleaning() : RiteCard(0, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        EnergyHoverTip
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(6, ValueProp.Move),
        new EnergyVar(1),
        new CardsVar(2)
    ];

    protected override async Task OnBaseEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    protected override async Task OnRiteEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        await CommonActions.Draw(this, choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
