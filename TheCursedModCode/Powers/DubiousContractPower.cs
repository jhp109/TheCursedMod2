using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheCursedMod.TheCursedModCode.Cards;

namespace TheCursedMod.TheCursedModCode.Powers;

public class DubiousContractPower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.FromCard<Dregs>(false)
    ];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player == base.Owner.Player && base.AmountOnTurnStart >= 1)
        {
            Flash();

            var ritePool = RiteCard.GetRiteCardPool(player);
            if (ritePool.Count > 0)
            {
                var randomRite = player.RunState.Rng.Niche.NextItem(ritePool)!;
                var riteCard = combatState.CreateCard(randomRite, player);
                await CardPileCmd.AddGeneratedCardToCombat(riteCard, PileType.Hand, addedByPlayer: true);
            }

            await Dregs.CreateAndAddToHand(player, 1);
        }
    }
}
