using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 공방 청소(Clean up Workshop) - 의례 : 방어도를 3 얻고 카드를 1장 뽑습니다. (강화 시 방어도 4, 카드 2장)
/// </summary>
public sealed class CleanUpWorkshop() : RiteCard(0, CardType.Skill, CardRarity.Basic, TargetType.None)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3, ValueProp.Move), new CardsVar(1)];

    protected override async Task OnRiteEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await CommonActions.Draw(this, choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
