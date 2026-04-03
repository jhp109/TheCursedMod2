using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 망토 두르기(Wear Cloak) - 방어도를 6 얻습니다. 손에 사용불가 카드가 있다면, 버린 카드 더미에서 카드 1장을 골라 손으로 가져옵니다.
/// 강화 시 방어도 9.
/// </summary>
public sealed class WearCloak() : TheCursedModCard(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(6, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Unplayable),
    ];

    protected override bool ShouldGlowGoldInternal => HasUnplayableInHand();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        if (!HasUnplayableInHand()) return;

        var pile = PileType.Discard.GetPile(Owner);
        if (pile.Cards.Count == 0) return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var selected = (await CardSelectCmd.FromSimpleGrid(choiceContext, pile.Cards, Owner, prefs)).FirstOrDefault();
        if (selected != null)
            await CardPileCmd.Add(selected, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
