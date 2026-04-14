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

    /// <summary>
    /// 전투 외적으로 영구적인 보너스를 주는 의례 카드 타입 목록.
    /// 스톨링 방지를 위해 전투당 최대 1회만 생성됩니다.
    /// </summary>
    private static readonly HashSet<Type> PermanentBonusRiteCards = [typeof(AntiAging), typeof(MiracleAlchemy)];

    private readonly HashSet<Type> _generatedPermanentCards = [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite)
    ];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player != base.Owner.Player || base.AmountOnTurnStart < 1) return;

        Flash();

        var ritePool = RiteCard.GetRiteCardPool(player)
            .Where(c => !PermanentBonusRiteCards.Contains(c.GetType()) || !_generatedPermanentCards.Contains(c.GetType()))
            .ToList();

        for (int i = 0; i < Amount && ritePool.Count > 0; i++)
        {
            var randomRite = player.RunState.Rng.CombatCardGeneration.NextItem(ritePool)!;

            if (PermanentBonusRiteCards.Contains(randomRite.GetType()))
            {
                _generatedPermanentCards.Add(randomRite.GetType());
                ritePool.RemoveAll(c => PermanentBonusRiteCards.Contains(c.GetType()));
            }

            var riteCard = combatState.CreateCard(randomRite, player);
            await CardPileCmd.AddGeneratedCardToCombat(riteCard, PileType.Hand, addedByPlayer: true);
        }

        for (int i = 0; i < Amount; i++)
            await TheCursedModCard.GainRandomCurse(player, player, combatState, PileType.Hand, addedByPlayer: true);
    }
}
