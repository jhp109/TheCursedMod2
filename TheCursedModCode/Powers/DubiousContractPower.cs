using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheCursedMod.TheCursedModCode.Cards;

namespace TheCursedMod.TheCursedModCode.Powers;

public class DubiousContractPower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite)
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player || base.AmountOnTurnStart < 1) return;

        Flash();

        var ritePool = RiteCard.GetRiteCardPool(player);
        for (int i = 0; i < Amount && ritePool.Count > 0; i++)
        {
            var randomRite = player.RunState.Rng.Niche.NextItem(ritePool)!;
            var riteCard = base.CombatState!.CreateCard(randomRite, player);
            await CardPileCmd.AddGeneratedCardToCombat(riteCard, PileType.Hand, addedByPlayer: true);
        }

        var curseCandidates = ModelDb.CardPool<CurseCardPool>()
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedByModifiers)
            .ToList();

        for (int i = 0; i < Amount && curseCandidates.Count > 0; i++)
        {
            var randomCurse = player.RunState.Rng.Niche.NextItem(curseCandidates)!;
            var curseCard = base.CombatState!.CreateCard(randomCurse, player);
            await CardPileCmd.AddGeneratedCardToCombat(curseCard, PileType.Hand, addedByPlayer: true);
        }
    }
}
