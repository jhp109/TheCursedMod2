using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Enchantments;
using TheCursedMod.TheCursedModCode.Cards;

namespace TheCursedMod.TheCursedModCode.Potions;

public sealed class CirclesInABottlePotion : TheCursedModPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment<Steady>().Append(
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle)
    );

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var circleCardPool = base.Owner.Character.CardPool
            .GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c is CircleCard)
            .ToList();

        var cards = CardFactory.GetDistinctForCombat(
            base.Owner,
            circleCardPool,
            3,
            base.Owner.RunState.Rng.CombatCardGeneration).ToList();

        foreach (var card in cards)
        {
            CardCmd.Upgrade(card);
            CardCmd.Enchant<Steady>(card, 1m);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
        }
    }
}
