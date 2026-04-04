using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheCursedMod.TheCursedModCode.Powers;

/// <summary>
/// 영혼 공물 - 매 턴 처음으로 의례로 저주를 소멸시킬 때, 힘을 1 얻습니다.
/// RiteCard.OnPlay의 저주 소멸 블록에서 TriggerOnRiteCurse가 호출됩니다.
/// </summary>
public class SpiritOfferingPower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    private int _lastTriggerRound = -1;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        StartPulsing();
        return Task.CompletedTask;
    }

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player == Owner.Player)
            StartPulsing();
        return Task.CompletedTask;
    }

    public async Task TriggerOnRiteEffect(PlayerChoiceContext choiceContext)
    {
        if (CombatState?.RoundNumber == _lastTriggerRound) return;
        _lastTriggerRound = CombatState?.RoundNumber ?? -1;
        StopPulsing();
        Flash();
        await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
    }
}
