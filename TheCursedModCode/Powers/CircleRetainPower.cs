using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheCursedMod.TheCursedModCode.Cards;

namespace TheCursedMod.TheCursedModCode.Powers;

/// <summary>
/// 마법진 보존 - 이번 턴 종료 시 손에 있는 마법진 카드를 버리지 않습니다.
/// 다음 턴 시작 시 모든 파일의 마법진에서 Retain 키워드를 제거하고 파워가 소멸합니다.
/// </summary>
public class CircleRetainPower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    private int _appliedRound = -1;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _appliedRound = CombatState?.RoundNumber ?? -1;
        Flash();

        var hand = Owner.Player?.PlayerCombatState?.Hand;
        if (hand != null)
            foreach (var card in hand.Cards.OfType<CircleCard>())
                card.AddKeyword(CardKeyword.Retain);

        return Task.CompletedTask;
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        // 적용된 턴에 드로우된 Circle 카드에도 Retain 추가
        if (card.Owner == Owner.Player && card is CircleCard && CombatState?.RoundNumber == _appliedRound)
            card.AddKeyword(CardKeyword.Retain);

        return Task.CompletedTask;
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player != Owner.Player) return;

        // 적용된 턴 시작 시에는 아무것도 하지 않음
        if (combatState.RoundNumber == _appliedRound) return;

        // 다음 턴 시작 시: 모든 파일(손/버림패/덱)의 마법진에서 Retain 제거하고 파워 소멸
        Flash();
        var pcs = Owner.Player?.PlayerCombatState;
        if (pcs != null)
            foreach (var card in pcs.AllCards.OfType<CircleCard>())
                card.RemoveKeyword(CardKeyword.Retain);

        await PowerCmd.Remove(this);
    }
}
