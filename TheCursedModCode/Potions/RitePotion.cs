using System.Linq;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheCursedMod.TheCursedModCode.Cards;

namespace TheCursedMod.TheCursedModCode.Potions;

public sealed class RitePotion : TheCursedModPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(Keywords.Rite),
        HoverTipFactory.FromCard<Dregs>(false)
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var riteCards = CardFactory.GetDistinctForCombat(
            base.Owner,
            RiteCard.GetRiteCardPool(base.Owner),
            3,
            base.Owner.RunState.Rng.CombatCardGeneration).ToList();

        var selected = await CardSelectCmd.FromChooseACardScreen(
            choiceContext, riteCards, base.Owner, canSkip: true);

        if (selected != null)
        {
            selected.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Hand, addedByPlayer: true);
        }

        await Dregs.CreateAndAddToHand(base.Owner, 1);
    }
}
