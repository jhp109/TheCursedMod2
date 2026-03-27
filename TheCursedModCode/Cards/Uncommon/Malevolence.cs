using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 악의(Malevolence) - 무작위 의례 카드를 1장 손으로 가져옵니다. 이번 턴 동안 비용 없이 사용할 수 있습니다. 소멸.
/// (강화 시 비용 0)
/// RiteCard를 상속한 카드라면 자동으로 후보 풀에 포함됩니다.
/// </summary>
public sealed class Malevolence() : TheCursedModCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var card = CardFactory.GetDistinctForCombat(
            Owner, RiteCard.GetRiteCardPool(Owner), 1, Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();

        if (card != null)
        {
            card.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
